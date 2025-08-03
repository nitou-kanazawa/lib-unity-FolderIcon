#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using FolderIcon.Editor.Core.Settings;

namespace FolderIcon.Editor.Core.Drawer
{
    [InitializeOnLoad]
    internal static class FolderDrawer
    {
        static FolderDrawer()
        {
            Debug.Log("FolderDrawer initialized");
            EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;

        }

        private static void OnProjectWindowItemGUI(string guid, Rect selectionRect)
        {
            if (Application.isPlaying || Event.current.type != EventType.Repaint)
                return;

            // Check if the item is a folder
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (!AssetDatabase.IsValidFolder(path))
                return;

            if (!FolderIconSettingsSO.instance.IsEnabled)
                return;

            // FIXME: 
            // - Dictonay管理で検索効率を改善
            // - 描画の効率化について調査
            var entry = FolderIconSettingsSO.instance.EntryStore.GetEntry(path);
            if (entry == null)
                return;

            DrawFolder(entry, selectionRect);
        }

        private static bool IsMainListAsset(Rect rect)
        {
            // Don't draw details if project view shows large preview icons:
            if (rect.height > 20)
                return false;

            return true;
        }


        private static void DrawFolder(FolderIconEntry entry, Rect rect)
        {
            if (entry.IconTexture == null)
                return;

            Rect imageRect = GetIconRect(rect);
            GUI.DrawTexture(imageRect, entry.IconTexture, ScaleMode.ScaleToFit, true);

            // フォルダアイコンの描画領域を計算
            Rect GetIconRect(Rect rect)
            {
                // Projectウィンドウの表示モードによってrectのサイズが異なるため調整
                float size = Mathf.Max(rect.height, 16f);
                float x = rect.x + 2;
                float y = rect.y + (rect.height - size) / 2f;
                return new Rect(x, y, size, size);
            }
        }


        #region Utility Methods

        /// <summary>
        /// Check if the current rect is the side view of folders
        /// </summary>
        /// <param name="rect">Current rect</param>
        public static bool IsSideView(Rect rect)
        {
            // Check if the item is in the side view
#if UNITY_2019_3_OR_NEWER
            return rect.x != 14;
#else
            return rect.x != 13;
#endif
        }

        /// <summary>
        /// Check if the current rect is in tree view
        /// </summary>
        /// <param name="rect">Current rect</param>
        private static bool IsTreeView(Rect rect)
        {
            // Check if the item is in the tree view
            return rect.width > rect.height;
        }
        #endregion

    }
}
#endif


// 参考資料：[Unity-Folder-Icons/FolderIcons/Editor/FolderIconGUI.cs](https://github.com/WooshiiDev/Unity-Folder-Icons/blob/main/FolderIcons/Editor/FolderIconGUI.cs)