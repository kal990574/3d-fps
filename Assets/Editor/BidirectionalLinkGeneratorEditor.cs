#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;
using Unity.AI.Navigation;
using System.Collections.Generic;

[CustomEditor(typeof(BidirectionalLinkGenerator))]
public class BidirectionalLinkGeneratorEditor : Editor
{
    // 엣지 중점 → 길이 매핑 (동적 width 계산용).
    private Dictionary<Vector3, float> _edgeLengthMap = new Dictionary<Vector3, float>();

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
        _edgeLengthMap.Clear();

        var triangulation = NavMesh.CalculateTriangulation();
        var vertices = triangulation.vertices;
        var indices = triangulation.indices;

        if (vertices.Length == 0)
        {
            Debug.LogWarning("NavMesh가 없습니다. NavMesh를 먼저 베이크하세요.");
            return new List<Vector3>();
        }

        // 엣지 카운트: 경계 엣지는 한 삼각형에만 속함.
        var edgeCount = new Dictionary<long, int>();
        var edgeData = new Dictionary<long, (Vector3 midpoint, float length)>();

        int triangleCount = indices.Length / 3;

        for (int t = 0; t < triangleCount; t++)
        {
            int baseIdx = t * 3;
            int i0 = indices[baseIdx];
            int i1 = indices[baseIdx + 1];
            int i2 = indices[baseIdx + 2];

            ProcessEdge(i0, i1, vertices, edgeCount, edgeData);
            ProcessEdge(i1, i2, vertices, edgeCount, edgeData);
            ProcessEdge(i2, i0, vertices, edgeCount, edgeData);
        }

        // 경계 엣지만 추출 (count == 1).
        var boundaryEdges = new List<Vector3>();
        var center = generator.transform.position;
        float radiusSqr = generator.scanRadius * generator.scanRadius;

        foreach (var kvp in edgeCount)
        {
            if (kvp.Value == 1)
            {
                var (midpoint, length) = edgeData[kvp.Key];

                // 범위 내 엣지만 포함.
                float distSqr = (midpoint.x - center.x) * (midpoint.x - center.x) +
                                (midpoint.z - center.z) * (midpoint.z - center.z);

                if (distSqr <= radiusSqr)
                {
                    boundaryEdges.Add(midpoint);
                    _edgeLengthMap[midpoint] = length;
                }
            }
        }

        return boundaryEdges;
    }

    void ProcessEdge(int i0, int i1, Vector3[] vertices,
        Dictionary<long, int> edgeCount, Dictionary<long, (Vector3, float)> edgeData)
    {
        int minIdx = Mathf.Min(i0, i1);
        int maxIdx = Mathf.Max(i0, i1);
        long edgeKey = ((long)minIdx << 32) | (uint)maxIdx;

        if (edgeCount.ContainsKey(edgeKey))
        {
            edgeCount[edgeKey]++;
        }
        else
        {
            edgeCount[edgeKey] = 1;
            Vector3 v0 = vertices[i0];
            Vector3 v1 = vertices[i1];
            edgeData[edgeKey] = ((v0 + v1) * 0.5f, Vector3.Distance(v0, v1));
        }
    }

    float GetEdgeLength(Vector3 midpoint)
    {
        if (_edgeLengthMap.TryGetValue(midpoint, out float length))
            return length;
        return float.MaxValue;
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

        // 수직 정렬 체크: 링크가 너무 틀어지면 생성 안 함.
        // 수평거리 vs 높이차 비율 체크 (45도 이상 기울면 제외)
        if (horizontalDist > heightDiff) return;

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

        // 엣지 길이 기반 동적 width 계산.
        float edgeLengthA = GetEdgeLength(a);
        float edgeLengthB = GetEdgeLength(b);
        float dynamicWidth = Mathf.Min(generator.linkWidth, edgeLengthA, edgeLengthB);

        // 링크 생성
        GameObject linkObj = new GameObject($"Link_{generator.generatedLinks.Count}");
        linkObj.transform.SetParent(generator.transform);
        linkObj.transform.position = (a + b) / 2f;

        var link = linkObj.AddComponent<NavMeshLink>();
        link.startPoint = linkObj.transform.InverseTransformPoint(a);
        link.endPoint = linkObj.transform.InverseTransformPoint(b);
        link.width = dynamicWidth;
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