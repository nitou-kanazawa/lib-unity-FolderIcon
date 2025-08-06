#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace FolderIcon.Editor
{
    /// <summary>
    /// FolderIconパッケージで使用する定数を定義するクラス．
    /// GUI描画、アイコン配置、色設定などの定数を含む．
    /// </summary>
    public static class FolderIconConstants
    {
        // Icon Drawing
        public const float GRID_VIEW_THRESHOLD = 20f;
        public const float CONTENT_VIEW_X_THRESHOLD = 20f;

        // Icon Position Adjustments
        public const float ICON_SIZE_EXPANSION = 2f;  // アイコンサイズの拡張量
        public const float ICON_X_OFFSET = -1f;       // X座標のオフセット
        public const float ICON_Y_OFFSET = -1f;       // Y座標のオフセット
        public const float TREE_VIEW_X_OFFSET = 2f;   // ツリービュー専用のX座標オフセット

        // Color
        public static readonly Color SelectedColor = new Color(0.235f, 0.360f, 0.580f);

        public static Color BackgroundColour = EditorGUIUtility.isProSkin
            ? new Color32(51, 51, 51, 255)
            : new Color32(190, 190, 190, 255);
    }
}
#endif