#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;
using Unity.AI.Navigation;
using System.Collections.Generic;

[CustomEditor(typeof(BidirectionalLinkGenerator))]
public class BidirectionalLinkGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        var generator = (BidirectionalLinkGenerator)target;
        
        EditorGUILayout.Space(10);
        
        if (GUILayout.Button("Generate Bidirectional Links", GUILayout.Height(30)))
        {
            GenerateLinks(generator);
        }
        
        if (GUILayout.Button("Clear Generated Links", GUILayout.Height(25)))
        {
            ClearLinks(generator);
        }
        
        EditorGUILayout.Space(5);
        
        if (GUILayout.Button("[Debug] Visualize Edges (5초)", GUILayout.Height(25)))
        {
            VisualizeEdges(generator);
        }
    }

    void VisualizeEdges(BidirectionalLinkGenerator generator)
    {
        var edges = FindNavMeshEdges(generator);
        Debug.Log($"발견된 엣지: {edges.Count}개");
        
        foreach (var edge in edges)
        {
            Debug.DrawRay(edge, Vector3.up * 2f, Color.red, 5f);
        }
    }

    void GenerateLinks(BidirectionalLinkGenerator generator)
    {
        ClearLinks(generator);
        
        Debug.Log("🔍 엣지 스캔 시작...");
        var edges = FindNavMeshEdges(generator);
        Debug.Log($"📍 발견된 엣지: {edges.Count}개");
        
        if (edges.Count < 2)
        {
            Debug.LogWarning("엣지가 2개 미만이라 링크를 만들 수 없어요.");
            return;
        }
        
        // 높이별로 엣지 분류
        var upperEdges = new List<Vector3>();
        var lowerEdges = new List<Vector3>();
        
        float avgHeight = 0f;
        foreach (var e in edges) avgHeight += e.y;
        avgHeight /= edges.Count;
        
        foreach (var edge in edges)
        {
            if (edge.y > avgHeight)
                upperEdges.Add(edge);
            else
                lowerEdges.Add(edge);
        }
        
        Debug.Log($"📍 상단 엣지: {upperEdges.Count}개, 하단 엣지: {lowerEdges.Count}개");
        
        // 상단-하단 엣지끼리 연결 시도
        foreach (var upper in upperEdges)
        {
            foreach (var lower in lowerEdges)
            {
                TryCreateLink(generator, upper, lower);
            }
        }
        
        Debug.Log($"✅ 생성된 링크: {generator.generatedLinks.Count}개");
    }

    List<Vector3> FindNavMeshEdges(BidirectionalLinkGenerator generator)
    {
        var edges = new List<Vector3>();
        var center = generator.transform.position;
        var step = generator.scanStep;
        var radius = generator.scanRadius;
        
        for (float x = -radius; x <= radius; x += step)
        {
            for (float z = -radius; z <= radius; z += step)
            {
                Vector3 rayStart = center + new Vector3(x, 50f, z);
                
                // 여러 높이의 NavMesh를 찾기 위해 RaycastAll 사용
                RaycastHit[] hits = Physics.RaycastAll(rayStart, Vector3.down, 100f);
                
                foreach (var hit in hits)
                {
                    if (IsEdgePoint(hit.point))
                    {
                        // 중복 제거 (가까운 점 무시)
                        bool tooClose = false;
                        foreach (var existing in edges)
                        {
                            if (Vector3.Distance(existing, hit.point) < step * 0.5f)
                            {
                                tooClose = true;
                                break;
                            }
                        }
                        
                        if (!tooClose)
                            edges.Add(hit.point);
                    }
                }
            }
        }
        
        return edges;
    }

    bool IsEdgePoint(Vector3 point)
    {
        // NavMesh 위인지 확인
        if (!NavMesh.SamplePosition(point, out NavMeshHit navHit, 1f, NavMesh.AllAreas))
            return false;
        
        // 실제 NavMesh 위치 사용
        point = navHit.position;
        
        // 주변 체크해서 경계인지 판단
        int offCount = 0;
        Vector3[] dirs = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };
        
        foreach (var dir in dirs)
        {
            Vector3 checkPoint = point + dir * 0.5f;
            if (!NavMesh.SamplePosition(checkPoint, out _, 0.3f, NavMesh.AllAreas))
            {
                offCount++;
            }
        }
        
        // 1~3방향이 NavMesh 밖이면 경계
        return offCount >= 1 && offCount < 4;
    }

    void TryCreateLink(BidirectionalLinkGenerator generator, Vector3 a, Vector3 b)
    {
        float dist = Vector3.Distance(a, b);
        float horizontalDist = Vector3.Distance(
            new Vector3(a.x, 0, a.z), 
            new Vector3(b.x, 0, b.z)
        );
        float heightDiff = Mathf.Abs(a.y - b.y);
        
        // 수평 거리 체크 (위/아래 연결이므로 수평 거리는 짧아야 함)
        if (horizontalDist > generator.maxJumpDistance) return;
        
        // 높이 차이가 있어야 의미 있음
        if (heightDiff < 0.5f) return;
        if (heightDiff > generator.maxHeightDiff) return;
        
        // 이미 NavMesh로 연결되어 있는지 체크
        NavMeshPath path = new NavMeshPath();
        if (NavMesh.CalculatePath(a, b, NavMesh.AllAreas, path))
        {
            if (path.status == NavMeshPathStatus.PathComplete)
            {
                // 경로 길이가 직선거리의 2배 이내면 이미 잘 연결됨
                float pathLength = CalculatePathLength(path);
                if (pathLength < dist * 2f)
                    return;
            }
        }
        
        // 링크 생성
        GameObject linkObj = new GameObject($"Link_{generator.generatedLinks.Count}");
        linkObj.transform.SetParent(generator.transform);
        linkObj.transform.position = (a + b) / 2f;
        
        var link = linkObj.AddComponent<NavMeshLink>();
        link.startPoint = linkObj.transform.InverseTransformPoint(a);
        link.endPoint = linkObj.transform.InverseTransformPoint(b);
        link.width = generator.linkWidth;
        link.bidirectional = true;
        
        generator.generatedLinks.Add(link);
        
        Undo.RegisterCreatedObjectUndo(linkObj, "Create NavMesh Link");
    }

    float CalculatePathLength(NavMeshPath path)
    {
        float length = 0f;
        for (int i = 1; i < path.corners.Length; i++)
        {
            length += Vector3.Distance(path.corners[i - 1], path.corners[i]);
        }
        return length;
    }

    void ClearLinks(BidirectionalLinkGenerator generator)
    {
        foreach (var link in generator.generatedLinks)
        {
            if (link != null)
                Undo.DestroyObjectImmediate(link.gameObject);
        }
        generator.generatedLinks.Clear();
    }
}
#endif