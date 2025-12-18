#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BidirectionalLinkGenerator))]
public class BidirectionalLinkGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "Window → AI → Bidirectional Link Generator 메뉴에서\n" +
            "더 많은 기능을 사용할 수 있습니다.",
            MessageType.Info);

        if (GUILayout.Button("Open Link Generator Window", GUILayout.Height(30)))
        {
            BidirectionalLinkWindow.ShowWindow();
        }
    }
}
#endif
