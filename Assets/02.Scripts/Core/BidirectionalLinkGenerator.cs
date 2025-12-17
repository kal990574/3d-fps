using UnityEngine;
using Unity.AI.Navigation;
using System.Collections.Generic;

public class BidirectionalLinkGenerator : MonoBehaviour
{
    [Header("Settings")]
    public float maxJumpDistance = 4f;
    public float maxHeightDiff = 2f;
    public float linkWidth = 1f;
    public float scanStep = 1f;
    public float scanRadius = 30f;
    
    [Header("Generated")]
    public List<NavMeshLink> generatedLinks = new List<NavMeshLink>();
}