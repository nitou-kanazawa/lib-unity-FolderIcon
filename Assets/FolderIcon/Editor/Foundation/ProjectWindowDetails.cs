using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class ProjectWindowDetails
{
    static ProjectWindowDetails()
    {
        // EditorApplication.projectWindowItemOnGUI += DrawAssetDetails;
    }

    private static void DrawAssetDetails(string guid, Rect rect)
    {
        // Right align label:
        const int width = 250;
        rect.x += rect.width - width;
        rect.width = width;

        GUI.Label(rect, guid);
    }
}