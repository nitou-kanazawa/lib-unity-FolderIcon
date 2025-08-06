#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using FolderIcon.Editor.Core.Settings;

namespace FolderIcon.Editor.Core.Drawer
{
    internal enum FolderIconViewMode
    {
        TreeView,           // 階層ツリー表示
        ContentListView,    // コンテンツリスト表示
        ContentGridView,    // コンテンツグリッド表示
    }


    /// <summary>
    [InitializeOnLoad]
    internal static class FolderDrawer
    {
        static FolderDrawer()
        {
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

        private static void DrawFolder(FolderIconEntry entry, Rect rect)
        {
            if (entry.IconTexture == null)
                return;

            // フォルダアイコンの描画領域を計算
            var viewMode = GetViewMode(rect);
            var imageRect = GetIconRect(rect, viewMode);

            // 
            GUI.DrawTexture(imageRect, entry.IconTexture, ScaleMode.ScaleToFit, true);
        }


        #region View Mode

        /// <summary>
        /// アイコンの表示モードを判定する．
        /// </summary>
        /// <param name="rect">フォルダの矩形</param>
        /// <returns>表示モード</returns>
        private static FolderIconViewMode GetViewMode(Rect rect)
        {
            // Grid View
            if (rect.height > FolderIconConstants.GRID_VIEW_THRESHOLD)
                return FolderIconViewMode.ContentGridView;

            // List View
            if (rect.x > FolderIconConstants.CONTENT_VIEW_X_THRESHOLD)
                return FolderIconViewMode.ContentListView;

            // Tree View
            return FolderIconViewMode.TreeView;
        }

        /// <summary>
        /// アイコンの描画領域を計算する．
        /// </summary>
        /// <param name="rect">フォルダの矩形</param>
        /// <param name="viewMode">表示モード</param>
        /// <returns>アイコンの描画領域</returns>
        private static Rect GetIconRect(Rect rect, FolderIconViewMode viewMode)
        {
            return viewMode switch
            {
                FolderIconViewMode.TreeView => new Rect(rect.x + 2, rect.y - 1, rect.height + 2, rect.height + 2),
                FolderIconViewMode.ContentListView => new Rect(rect.x - 1, rect.y - 1, rect.height + 2, rect.height + 2),
                FolderIconViewMode.ContentGridView => new Rect(rect.x - 1, rect.y - 1, rect.width + 2, rect.width + 2),
                _ => throw new System.Exception("Invalid view mode")
            };
        }
        #endregion
    }
}
#endif


// 参考資料：[Unity-Folder-Icons/FolderIcons/Editor/FolderIconGUI.cs](https://github.com/WooshiiDev/Unity-Folder-Icons/blob/main/FolderIcons/Editor/FolderIconGUI.cs)