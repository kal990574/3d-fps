using UnityEngine;
using Unity.AI.Navigation;
using System.Collections.Generic;

public class BidirectionalLinkGenerator : MonoBehaviour
{
    [Header("Link Generation Settings")]
    [Tooltip("링크가 연결할 수 있는 최대 수평 거리")]
    public float maxHorizontalDistance = 2f;

    [Tooltip("링크가 연결할 수 있는 최소 높이 차이")]
    public float minHeightDiff = 0.5f;

    [Tooltip("링크가 연결할 수 있는 최대 높이 차이")]
    public float maxHeightDiff = 10f;

    [Tooltip("링크의 기본 너비")]
    public float linkWidth = 1f;

    [Tooltip("링크 너비의 최소값 (동적 계산 시)")]
    public float minLinkWidth = 0.5f;

    [Header("Scan Settings")]
    [Tooltip("Generator 위치 기준 스캔 반경")]
    public float scanRadius = 100f;

    [Tooltip("높이 레이어 간격 (이 값보다 높이 차이가 크면 다른 레이어로 분류)")]
    public float heightLayerThreshold = 1f;

    [Header("Advanced Settings")]
    [Tooltip("동적 너비 사용 (엣지 길이에 따라 너비 자동 조절)")]
    public bool useDynamicWidth = true;

    [Tooltip("기존 NavMesh 경로 체크 (이미 연결된 곳은 스킵)")]
    public bool checkExistingPath = true;

    [Tooltip("기존 경로 대비 직선거리 배율 (이 값 이내면 스킵)")]
    public float existingPathMultiplier = 2f;

    [Header("Generated Links")]
    public List<NavMeshLink> generatedLinks = new List<NavMeshLink>();

    [Header("Debug")]
    public bool showDebugLogs = true;
}
