#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using FolderIcon.Editor.Core.Settings;
using System.IO;

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
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(path) ||
                Event.current.type != EventType.Repaint ||
                !File.GetAttributes(path).HasFlag(FileAttributes.Directory) ||
                !FolderIconSettingsSO.instance.IsEnabled)
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
    }
}
#endif