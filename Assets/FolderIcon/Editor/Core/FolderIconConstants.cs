#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace FolderIcon.Editor
{
    public static class FolderIconConstants
    {
        // GUI
        public const float MAX_TREE_WIDTH = 118f;
        public const float MAX_PROJECT_WIDTH = 96f;
        public const float MAX_TREE_HEIGHT = 16f;
        public const float MAX_PROJECT_HEIGHT = 110f;

        // Icon Drawing
        public const float DEFAULT_ICON_SIZE = 16f;
        public const float ICON_PADDING = 2f;

        public const float GRID_VIEW_THRESHOLD = 20f;
        public const float CONTENT_VIEW_X_THRESHOLD = 20f;

        public const float TREE_VIEW_WIDTH_THRESHOLD = 200f;

        // Color
        public static readonly Color SelectedColor = new Color(0.235f, 0.360f, 0.580f);

        public static Color BackgroundColour = EditorGUIUtility.isProSkin
            ? new Color32(51, 51, 51, 255)
            : new Color32(190, 190, 190, 255);
    }
}
#endif