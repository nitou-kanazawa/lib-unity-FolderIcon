using UnityEditor;
using UnityEngine;

namespace FolderIcon.Editor.Core
{
    public class FolderIconSettingWindow : EditorWindow
    {
        private string folderPath;
        private Texture2D selectedIcon;

        public static void ShowWindow(string folderPath)
        {
            var window = GetWindow<FolderIconSettingWindow>(true, "フォルダアイコン設定");
            window.folderPath = folderPath;
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label($"設定対象: {folderPath}", EditorStyles.boldLabel);
            GUILayout.Space(10);
            GUILayout.Label("アイコンを選択:");
            selectedIcon = (Texture2D)EditorGUILayout.ObjectField(selectedIcon, typeof(Texture2D), false);
            GUILayout.Space(10);
            if (GUILayout.Button("設定"))
            {
                // TODO: アイコン設定処理
                Close();
            }
        }
    }
}
