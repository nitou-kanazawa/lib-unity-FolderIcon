using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FolderIcon.Editor
{
    /// <summary>
    /// 選択フォルダにアイコンを個別指定（FolderPath ルール）するユーティリティウィンドウ（仕様書 §7.1）．
    /// 右クリックメニューから開く．
    /// </summary>
    internal sealed class FolderIconSettingWindow : EditorWindow
    {
        [SerializeField] private List<string> _targetPaths = new();
        [SerializeField] private Texture2D _selectedIcon;

        public static void Open(IReadOnlyList<string> folderPaths)
        {
            var window = GetWindow<FolderIconSettingWindow>(utility: true, title: "Set Folder Icon", focus: true);
            window._targetPaths = new List<string>(folderPaths);
            window._selectedIcon = window.GetInitialIcon();
            window.minSize = new Vector2(340f, 190f);
            window.Show();
        }

        private void OnGUI()
        {
            if (_targetPaths.Count == 0)
            {
                EditorGUILayout.HelpBox("No folder selected. Reopen from the folder context menu.", MessageType.Info);
                if (GUILayout.Button("Close"))
                    Close();
                return;
            }

            DrawTargetInfo();
            EditorGUILayout.Space();

            _selectedIcon = (Texture2D)EditorGUILayout.ObjectField(
                "Icon", _selectedIcon, typeof(Texture2D), allowSceneObjects: false);

            EditorGUILayout.Space();
            DrawButtons();
        }

        private void DrawTargetInfo()
        {
            if (_targetPaths.Count == 1)
            {
                EditorGUILayout.LabelField("Target", _targetPaths[0]);
            }
            else
            {
                EditorGUILayout.LabelField("Target", $"{_targetPaths[0]}  (+{_targetPaths.Count - 1} more)");
            }

            // 現在の適用状態と出所（§7.1）．複数選択時は先頭フォルダのみ表示する
            var current = FolderIconResolver.Resolve(_targetPaths[0], out var source);
            var sourceLabel = source switch
            {
                FolderIconSource.UserPathRule => "This folder (path rule)",
                FolderIconSource.UserNameRule => "Name rule",
                _ => "None",
            };
            EditorGUILayout.LabelField("Current", sourceLabel);

            if (current != null)
            {
                var previewRect = GUILayoutUtility.GetRect(32f, 32f, GUILayout.Width(32f));
                GUI.DrawTexture(previewRect, current, ScaleMode.ScaleToFit, alphaBlend: true);
            }
        }

        private void DrawButtons()
        {
            var settings = FolderIconSettings.instance;

            using (new EditorGUI.DisabledScope(_selectedIcon == null))
            {
                if (GUILayout.Button("Apply"))
                {
                    foreach (var path in _targetPaths)
                        settings.SetPathRule(path, _selectedIcon);
                    settings.Save();
                    Close();
                }
            }

            bool hasPathRule = false;
            foreach (var path in _targetPaths)
            {
                if (settings.FindPathRule(path) != null)
                {
                    hasPathRule = true;
                    break;
                }
            }

            // 個別指定が存在する場合のみ解除ボタンを表示（解除後は名前ルール等の下位に戻る）
            if (hasPathRule && GUILayout.Button("Remove"))
            {
                foreach (var path in _targetPaths)
                    settings.RemovePathRule(path);
                settings.Save();
                Close();
            }
        }

        /// <summary>
        /// ウィンドウを開いた時点の初期アイコン．
        /// 先頭フォルダに個別指定が既にあればそれを引き継ぐ．
        /// </summary>
        private Texture2D GetInitialIcon()
        {
            if (_targetPaths.Count == 0)
                return null;
            return FolderIconSettings.instance.FindPathRule(_targetPaths[0])?.Icon;
        }
    }
}
