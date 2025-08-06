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
    /// Unityプロジェクトウィンドウでフォルダアイコンを描画するクラス．
    /// 各表示モード（ツリービュー、リストビュー、グリッドビュー）に応じて
    /// 適切な位置とサイズでアイコンを描画する．
    /// </summary>
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

            // TODO: パフォーマンス改善の検討
            // - Dictionary管理で検索効率を改善
            // - 描画の効率化について調査
            // - キャッシュ機構の導入を検討
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

            // アイコンテクスチャを描画
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
                FolderIconViewMode.TreeView => GetTreeViewIconRect(rect),
                FolderIconViewMode.ContentListView => GetListViewIconRect(rect),
                FolderIconViewMode.ContentGridView => GetGridViewIconRect(rect),
                _ => throw new System.ArgumentException($"Invalid view mode: {viewMode}", nameof(viewMode))
            };
        }

        /// <summary>
        /// ツリービュー用のアイコン描画領域を計算する．
        /// </summary>
        private static Rect GetTreeViewIconRect(Rect rect)
        {
            float iconSize = rect.height + FolderIconConstants.ICON_SIZE_EXPANSION;
            float x = rect.x + FolderIconConstants.TREE_VIEW_X_OFFSET;
            float y = rect.y + FolderIconConstants.ICON_Y_OFFSET;
            return new Rect(x, y, iconSize, iconSize);
        }

        /// <summary>
        /// リストビュー用のアイコン描画領域を計算する．
        /// </summary>
        private static Rect GetListViewIconRect(Rect rect)
        {
            float iconSize = rect.height + FolderIconConstants.ICON_SIZE_EXPANSION;
            float x = rect.x + FolderIconConstants.ICON_X_OFFSET;
            float y = rect.y + FolderIconConstants.ICON_Y_OFFSET;
            return new Rect(x, y, iconSize, iconSize);
        }

        /// <summary>
        /// グリッドビュー用のアイコン描画領域を計算する．
        /// </summary>
        private static Rect GetGridViewIconRect(Rect rect)
        {
            float iconSize = rect.width + FolderIconConstants.ICON_SIZE_EXPANSION;
            float x = rect.x + FolderIconConstants.ICON_X_OFFSET;
            float y = rect.y + FolderIconConstants.ICON_Y_OFFSET;
            return new Rect(x, y, iconSize, iconSize);
        }
        #endregion
    }
}
#endif


// 参考資料：[Unity-Folder-Icons/FolderIcons/Editor/FolderIconGUI.cs](https://github.com/WooshiiDev/Unity-Folder-Icons/blob/main/FolderIcons/Editor/FolderIconGUI.cs)