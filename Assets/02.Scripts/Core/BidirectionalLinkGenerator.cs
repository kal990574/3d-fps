using UnityEngine;
using Unity.AI.Navigation;
using System.Collections.Generic;

public class BidirectionalLinkGenerator : MonoBehaviour
{
    [Header("Settings")]
    public float maxJumpDistance = 2f;
    public float maxHeightDiff = 10f;
    public float linkWidth = 20f;
    public float scanRadius = 100f;

    [Header("Generated")]
    public List<NavMeshLink> generatedLinks = new List<NavMeshLink>();
}