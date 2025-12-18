#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;
using Unity.AI.Navigation;
using System.Collections.Generic;
using System.Linq;

public class BidirectionalLinkWindow : EditorWindow
{
    private BidirectionalLinkGenerator _generator;
    private Vector2 _scrollPosition;

    // 엣지 데이터 구조체
    private struct EdgeData
    {
        public Vector3 Start;
        public Vector3 End;
        public Vector3 Midpoint;
        public float Length;
        public float Height;

        public EdgeData(Vector3 start, Vector3 end)
        {
            Start = start;
            End = end;
            Midpoint = (start + end) * 0.5f;
            Length = Vector3.Distance(start, end);
            Height = Midpoint.y;
        }
    }

    // 높이 레이어 구조체
    private struct HeightLayer
    {
        public float MinHeight;
        public float MaxHeight;
        public List<EdgeData> Edges;

        public float CenterHeight => (MinHeight + MaxHeight) * 0.5f;
    }

    [MenuItem("Window/AI/Bidirectional Link Generator")]
    public static void ShowWindow()
    {
        var window = GetWindow<BidirectionalLinkWindow>("Link Generator");
        window.minSize = new Vector2(350, 400);
    }

    private void OnEnable()
    {
        if (_generator == null)
        {
            _generator = FindObjectOfType<BidirectionalLinkGenerator>();
        }
    }

    private void OnGUI()
    {
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

        DrawHeader();
        DrawGeneratorField();

        if (_generator == null)
        {
            DrawNoGeneratorMessage();
            EditorGUILayout.EndScrollView();
            return;
        }

        DrawSettings();
        DrawAdvancedSettings();
        DrawActionButtons();
        DrawDebugSection();
        DrawStatusSection();

        EditorGUILayout.EndScrollView();

        SaveChanges();
    }

    private void DrawHeader()
    {
        EditorGUILayout.LabelField("Bidirectional Link Generator", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "NavMesh 경계 엣지를 분석하여 양방향 이동 가능한 링크를 자동 생성합니다.\n" +
            "점프(위로)와 드롭(아래로) 모두 지원합니다.",
            MessageType.Info);
        EditorGUILayout.Space(10);
    }

    private void DrawGeneratorField()
    {
        _generator = (BidirectionalLinkGenerator)EditorGUILayout.ObjectField(
            "Target Generator",
            _generator,
            typeof(BidirectionalLinkGenerator),
            true
        );
    }

    private void DrawNoGeneratorMessage()
    {
        EditorGUILayout.HelpBox(
            "Generator 오브젝트를 선택하세요.\n" +
            "씬에 BidirectionalLinkGenerator 컴포넌트가 있는 오브젝트가 필요합니다.",
            MessageType.Warning);

        EditorGUILayout.Space(10);

        if (GUILayout.Button("씬에서 자동 탐색", GUILayout.Height(25)))
        {
            _generator = FindObjectOfType<BidirectionalLinkGenerator>();
            if (_generator == null)
            {
                EditorUtility.DisplayDialog("Not Found",
                    "씬에 BidirectionalLinkGenerator가 없습니다.\n" +
                    "빈 GameObject에 BidirectionalLinkGenerator 컴포넌트를 추가하세요.",
                    "OK");
            }
        }

        if (GUILayout.Button("새 Generator 생성", GUILayout.Height(25)))
        {
            CreateNewGenerator();
        }
    }

    private void CreateNewGenerator()
    {
        var go = new GameObject("BidirectionalLinkGenerator");
        _generator = go.AddComponent<BidirectionalLinkGenerator>();
        Selection.activeGameObject = go;
        Undo.RegisterCreatedObjectUndo(go, "Create BidirectionalLinkGenerator");
    }

    private void DrawSettings()
    {
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Link Settings", EditorStyles.boldLabel);

        EditorGUI.indentLevel++;
        _generator.maxHorizontalDistance = EditorGUILayout.FloatField(
            new GUIContent("Max Horizontal Distance", "수평 거리 제한"),
            _generator.maxHorizontalDistance);

        _generator.minHeightDiff = EditorGUILayout.FloatField(
            new GUIContent("Min Height Diff", "최소 높이 차이 (이 이하는 링크 안 만듦)"),
            _generator.minHeightDiff);

        _generator.maxHeightDiff = EditorGUILayout.FloatField(
            new GUIContent("Max Height Diff", "최대 높이 차이"),
            _generator.maxHeightDiff);

        _generator.linkWidth = EditorGUILayout.FloatField(
            new GUIContent("Link Width", "링크 기본 너비"),
            _generator.linkWidth);

        _generator.scanRadius = EditorGUILayout.FloatField(
            new GUIContent("Scan Radius", "스캔 반경"),
            _generator.scanRadius);

        EditorGUI.indentLevel--;
    }

    private void DrawAdvancedSettings()
    {
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Advanced Settings", EditorStyles.boldLabel);

        EditorGUI.indentLevel++;

        _generator.useDynamicWidth = EditorGUILayout.Toggle(
            new GUIContent("Use Dynamic Width", "엣지 길이에 따라 링크 너비 자동 조절"),
            _generator.useDynamicWidth);

        if (_generator.useDynamicWidth)
        {
            EditorGUI.indentLevel++;
            _generator.minLinkWidth = EditorGUILayout.FloatField(
                new GUIContent("Min Link Width", "동적 너비의 최소값"),
                _generator.minLinkWidth);
            EditorGUI.indentLevel--;
        }

        _generator.checkExistingPath = EditorGUILayout.Toggle(
            new GUIContent("Check Existing Path", "이미 NavMesh로 연결된 곳은 스킵"),
            _generator.checkExistingPath);

        if (_generator.checkExistingPath)
        {
            EditorGUI.indentLevel++;
            _generator.existingPathMultiplier = EditorGUILayout.FloatField(
                new GUIContent("Path Multiplier", "기존 경로가 직선거리의 N배 이내면 스킵"),
                _generator.existingPathMultiplier);
            EditorGUI.indentLevel--;
        }

        _generator.heightLayerThreshold = EditorGUILayout.FloatField(
            new GUIContent("Height Layer Threshold", "높이 레이어 분류 기준"),
            _generator.heightLayerThreshold);

        _generator.showDebugLogs = EditorGUILayout.Toggle(
            new GUIContent("Show Debug Logs", "콘솔에 디버그 로그 출력"),
            _generator.showDebugLogs);

        EditorGUI.indentLevel--;
    }

    private void DrawActionButtons()
    {
        EditorGUILayout.Space(15);

        // Generate 버튼
        GUI.backgroundColor = new Color(0.4f, 0.8f, 0.4f);
        if (GUILayout.Button("Generate Bidirectional Links", GUILayout.Height(40)))
        {
            GenerateLinks();
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space(5);

        // Clear 버튼
        GUI.backgroundColor = new Color(0.9f, 0.5f, 0.5f);
        if (GUILayout.Button("Clear All Links", GUILayout.Height(30)))
        {
            ClearLinks();
        }
        GUI.backgroundColor = Color.white;
    }

    private void DrawDebugSection()
    {
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Debug Tools", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Visualize Edges (5s)", GUILayout.Height(25)))
        {
            VisualizeEdges();
        }

        if (GUILayout.Button("Visualize Layers (5s)", GUILayout.Height(25)))
        {
            VisualizeLayers();
        }

        EditorGUILayout.EndHorizontal();
    }

    private void DrawStatusSection()
    {
        EditorGUILayout.Space(10);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Status", EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"Generated Links: {_generator.generatedLinks.Count}");
        EditorGUILayout.EndVertical();
    }

    private void SaveChanges()
    {
        if (GUI.changed && _generator != null)
        {
            EditorUtility.SetDirty(_generator);
        }
    }

    // ============================================================
    // 링크 생성 로직
    // ============================================================

    private void GenerateLinks()
    {
        ClearLinks();

        Log("링크 생성 시작...");

        // 1. NavMesh 경계 엣지 추출
        var edges = FindBoundaryEdges();
        if (edges.Count == 0)
        {
            Debug.LogWarning("경계 엣지를 찾을 수 없습니다. NavMesh를 먼저 베이크하세요.");
            return;
        }
        Log($"발견된 경계 엣지: {edges.Count}개");

        // 2. 높이별 레이어 분류
        var layers = ClassifyByHeightLayers(edges);
        Log($"높이 레이어: {layers.Count}개");

        // 3. 인접 레이어 간 링크 생성
        int linkCount = CreateLinksBetweenLayers(layers);

        Log($"생성 완료! 총 {linkCount}개의 링크가 생성되었습니다.");
    }

    private List<EdgeData> FindBoundaryEdges()
    {
        var triangulation = NavMesh.CalculateTriangulation();
        var vertices = triangulation.vertices;
        var indices = triangulation.indices;

        if (vertices.Length == 0)
        {
            return new List<EdgeData>();
        }

        // 엣지별 삼각형 소속 카운트
        var edgeTriangleCount = new Dictionary<long, int>();
        var edgeVertices = new Dictionary<long, (int i0, int i1)>();

        int triangleCount = indices.Length / 3;

        for (int t = 0; t < triangleCount; t++)
        {
            int baseIdx = t * 3;
            int i0 = indices[baseIdx];
            int i1 = indices[baseIdx + 1];
            int i2 = indices[baseIdx + 2];

            CountEdge(i0, i1, edgeTriangleCount, edgeVertices);
            CountEdge(i1, i2, edgeTriangleCount, edgeVertices);
            CountEdge(i2, i0, edgeTriangleCount, edgeVertices);
        }

        // 경계 엣지 추출 (count == 1)
        var boundaryEdges = new List<EdgeData>();
        var center = _generator.transform.position;
        float radiusSqr = _generator.scanRadius * _generator.scanRadius;

        foreach (var kvp in edgeTriangleCount)
        {
            if (kvp.Value == 1)
            {
                var (i0, i1) = edgeVertices[kvp.Key];
                var v0 = vertices[i0];
                var v1 = vertices[i1];
                var edge = new EdgeData(v0, v1);

                // 범위 체크
                float distSqr = (edge.Midpoint.x - center.x) * (edge.Midpoint.x - center.x) +
                                (edge.Midpoint.z - center.z) * (edge.Midpoint.z - center.z);

                if (distSqr <= radiusSqr)
                {
                    boundaryEdges.Add(edge);
                }
            }
        }

        return boundaryEdges;
    }

    private void CountEdge(int i0, int i1,
        Dictionary<long, int> edgeCount,
        Dictionary<long, (int, int)> edgeVertices)
    {
        int minIdx = Mathf.Min(i0, i1);
        int maxIdx = Mathf.Max(i0, i1);
        long key = ((long)minIdx << 32) | (uint)maxIdx;

        if (edgeCount.ContainsKey(key))
        {
            edgeCount[key]++;
        }
        else
        {
            edgeCount[key] = 1;
            edgeVertices[key] = (i0, i1);
        }
    }

    private List<HeightLayer> ClassifyByHeightLayers(List<EdgeData> edges)
    {
        if (edges.Count == 0) return new List<HeightLayer>();

        // 높이 기준 정렬
        var sortedEdges = edges.OrderBy(e => e.Height).ToList();

        var layers = new List<HeightLayer>();
        var currentLayer = new HeightLayer
        {
            MinHeight = sortedEdges[0].Height,
            MaxHeight = sortedEdges[0].Height,
            Edges = new List<EdgeData> { sortedEdges[0] }
        };

        for (int i = 1; i < sortedEdges.Count; i++)
        {
            var edge = sortedEdges[i];

            // 현재 레이어와 높이 차이가 threshold 이내면 같은 레이어
            if (edge.Height - currentLayer.MaxHeight <= _generator.heightLayerThreshold)
            {
                currentLayer.MaxHeight = edge.Height;
                currentLayer.Edges.Add(edge);
            }
            else
            {
                // 새 레이어 시작
                layers.Add(currentLayer);
                currentLayer = new HeightLayer
                {
                    MinHeight = edge.Height,
                    MaxHeight = edge.Height,
                    Edges = new List<EdgeData> { edge }
                };
            }
        }

        // 마지막 레이어 추가
        layers.Add(currentLayer);

        return layers;
    }

    private int CreateLinksBetweenLayers(List<HeightLayer> layers)
    {
        int totalLinks = 0;

        // 모든 레이어 조합 간 링크 생성
        for (int i = 0; i < layers.Count; i++)
        {
            for (int j = i + 1; j < layers.Count; j++)
            {
                var lowerLayer = layers[i];
                var upperLayer = layers[j];

                float heightDiff = upperLayer.CenterHeight - lowerLayer.CenterHeight;

                // 높이 차이 체크
                if (heightDiff < _generator.minHeightDiff || heightDiff > _generator.maxHeightDiff)
                {
                    continue;
                }

                Log($"레이어 {i} (h={lowerLayer.CenterHeight:F1}) ↔ 레이어 {j} (h={upperLayer.CenterHeight:F1}) 연결 시도");

                // Spatial hash로 최적화된 검색
                var createdPositions = new HashSet<Vector3Int>();

                foreach (var upperEdge in upperLayer.Edges)
                {
                    foreach (var lowerEdge in lowerLayer.Edges)
                    {
                        if (TryCreateLink(upperEdge, lowerEdge, createdPositions))
                        {
                            totalLinks++;
                        }
                    }
                }
            }
        }

        return totalLinks;
    }

    private bool TryCreateLink(EdgeData upper, EdgeData lower, HashSet<Vector3Int> createdPositions)
    {
        // 수평 거리 계산
        float horizontalDist = Vector2.Distance(
            new Vector2(upper.Midpoint.x, upper.Midpoint.z),
            new Vector2(lower.Midpoint.x, lower.Midpoint.z)
        );

        // 수평 거리 체크
        if (horizontalDist > _generator.maxHorizontalDistance)
        {
            return false;
        }

        // 높이 차이 계산
        float heightDiff = Mathf.Abs(upper.Midpoint.y - lower.Midpoint.y);

        // 높이 차이 체크
        if (heightDiff < _generator.minHeightDiff || heightDiff > _generator.maxHeightDiff)
        {
            return false;
        }

        // 수직 정렬 체크 (45도 이상 기울면 제외)
        if (horizontalDist > heightDiff)
        {
            return false;
        }

        // 중복 체크 (같은 위치에 이미 링크가 있는지)
        var posKey = new Vector3Int(
            Mathf.RoundToInt(upper.Midpoint.x * 2),
            Mathf.RoundToInt(upper.Midpoint.y * 2),
            Mathf.RoundToInt(upper.Midpoint.z * 2)
        );

        if (createdPositions.Contains(posKey))
        {
            return false;
        }

        // 기존 NavMesh 경로 체크
        if (_generator.checkExistingPath)
        {
            if (IsAlreadyConnected(upper.Midpoint, lower.Midpoint))
            {
                return false;
            }
        }

        // 링크 생성
        CreateNavMeshLink(upper, lower);
        createdPositions.Add(posKey);

        return true;
    }

    private bool IsAlreadyConnected(Vector3 a, Vector3 b)
    {
        var path = new NavMeshPath();
        if (NavMesh.CalculatePath(a, b, NavMesh.AllAreas, path))
        {
            if (path.status == NavMeshPathStatus.PathComplete)
            {
                float pathLength = CalculatePathLength(path);
                float directDist = Vector3.Distance(a, b);

                if (pathLength < directDist * _generator.existingPathMultiplier)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private float CalculatePathLength(NavMeshPath path)
    {
        float length = 0f;
        for (int i = 1; i < path.corners.Length; i++)
        {
            length += Vector3.Distance(path.corners[i - 1], path.corners[i]);
        }
        return length;
    }

    private void CreateNavMeshLink(EdgeData upper, EdgeData lower)
    {
        // 너비 계산
        float width = _generator.linkWidth;

        if (_generator.useDynamicWidth)
        {
            float minEdgeLength = Mathf.Min(upper.Length, lower.Length);
            width = Mathf.Max(_generator.minLinkWidth, Mathf.Min(width, minEdgeLength));
        }

        // 링크 오브젝트 생성
        var linkObj = new GameObject($"Link_{_generator.generatedLinks.Count}");
        linkObj.transform.SetParent(_generator.transform);
        linkObj.transform.position = (upper.Midpoint + lower.Midpoint) * 0.5f;

        // NavMeshLink 컴포넌트 설정
        var link = linkObj.AddComponent<NavMeshLink>();
        link.startPoint = linkObj.transform.InverseTransformPoint(upper.Midpoint);
        link.endPoint = linkObj.transform.InverseTransformPoint(lower.Midpoint);
        link.width = width;
        link.bidirectional = true;

        _generator.generatedLinks.Add(link);
        Undo.RegisterCreatedObjectUndo(linkObj, "Create NavMesh Link");
    }

    private void ClearLinks()
    {
        foreach (var link in _generator.generatedLinks)
        {
            if (link != null)
            {
                Undo.DestroyObjectImmediate(link.gameObject);
            }
        }
        _generator.generatedLinks.Clear();
        Log("모든 링크가 삭제되었습니다.");
    }

    // ============================================================
    // 디버그 시각화
    // ============================================================

    private void VisualizeEdges()
    {
        var edges = FindBoundaryEdges();
        Log($"발견된 경계 엣지: {edges.Count}개");

        foreach (var edge in edges)
        {
            // 엣지 선 그리기
            Debug.DrawLine(edge.Start, edge.End, Color.cyan, 5f);
            // 중점 표시
            Debug.DrawRay(edge.Midpoint, Vector3.up * 1f, Color.red, 5f);
        }
    }

    private void VisualizeLayers()
    {
        var edges = FindBoundaryEdges();
        var layers = ClassifyByHeightLayers(edges);

        Log($"높이 레이어: {layers.Count}개");

        Color[] colors = { Color.red, Color.green, Color.blue, Color.yellow, Color.magenta, Color.cyan };

        for (int i = 0; i < layers.Count; i++)
        {
            var layer = layers[i];
            var color = colors[i % colors.Length];

            Log($"  레이어 {i}: 높이 {layer.MinHeight:F1}~{layer.MaxHeight:F1}, 엣지 {layer.Edges.Count}개");

            foreach (var edge in layer.Edges)
            {
                Debug.DrawLine(edge.Start, edge.End, color, 5f);
                Debug.DrawRay(edge.Midpoint, Vector3.up * 0.5f, color, 5f);
            }
        }
    }

    private void Log(string message)
    {
        if (_generator.showDebugLogs)
        {
            Debug.Log($"[LinkGenerator] {message}");
        }
    }
}
#endif
