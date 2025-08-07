using UnityEditor;
using UnityEngine;
using System.IO;

namespace FolderIcon.Editor.Core
{
    public static class FolderIconContextMenu
    {
        [MenuItem("Assets/フォルダアイコンを設定", false, 2000)]
        private static void SetFolderIcon()
        {
            // 選択されたフォルダのパスを取得
            var selected = Selection.activeObject;
            var path = AssetDatabase.GetAssetPath(selected);
            if (!AssetDatabase.IsValidFolder(path))
            {
                EditorUtility.DisplayDialog("フォルダ選択", "フォルダを選択してください。", "OK");
                return;
            }
            // アイコン設定ウィンドウを表示
            FolderIconSettingWindow.ShowWindow(path);
        }

        [MenuItem("Assets/フォルダアイコンを設定", true)]
        private static bool SetFolderIcon_Validate()
        {
            var selected = Selection.activeObject;
            if (selected == null) return false;
            var path = AssetDatabase.GetAssetPath(selected);
            return AssetDatabase.IsValidFolder(path);
        }
    }
}
