using System.Collections.Generic;
using UnityEditor;

namespace FolderIcon.Editor
{
    /// <summary>
    /// フォルダの右クリックメニューからアイコン設定ウィンドウを開く（仕様書 §7.1）．
    /// 主導線．複数フォルダ選択にも対応する．
    /// </summary>
    internal static class FolderIconContextMenu
    {
        private const string MenuPath = "Assets/Set Folder Icon...";

        [MenuItem(MenuPath, false, 2000)]
        private static void Open()
        {
            FolderIconSettingWindow.Open(GetSelectedFolderPaths());
        }

        [MenuItem(MenuPath, true)]
        private static bool Validate()
        {
            return GetSelectedFolderPaths().Count > 0;
        }

        private static List<string> GetSelectedFolderPaths()
        {
            var paths = new List<string>();
            foreach (var obj in Selection.objects)
            {
                var path = AssetDatabase.GetAssetPath(obj);
                if (AssetDatabase.IsValidFolder(path) && !paths.Contains(path))
                    paths.Add(path);
            }
            return paths;
        }
    }
}
