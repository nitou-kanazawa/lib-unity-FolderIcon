#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

namespace FolderIcon.Editor.Core.Settings
{
    internal sealed class FolderIconSettingsProvider : SettingsProvider
    {
        private UnityEditor.Editor _editor;

        public FolderIconSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null)
            : base(path, scopes, keywords)
        {
        }

        /// <summary>
        /// 設定ファイルのインスペクターを生成
        /// </summary>
        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            // NOTE: ScriptableSingletonを編集可能にする
            var preferences = FolderIconSettingsSO.instance;
            preferences.hideFlags = HideFlags.HideAndDontSave & ~HideFlags.NotEditable;

            // 設定ファイルの標準のインスペクターのエディタを生成
            UnityEditor.Editor.CreateCachedEditor(preferences, null, ref _editor);
        }

        /// <summary>
        /// 設定ファイルのインスペクターを表示
        /// </summary>
        public override void OnGUI(string searchContext)
        {
            EditorGUI.BeginChangeCheck();

            // 設定ファイルの標準インスペクタを表示
            _editor.OnInspectorGUI();

            if (EditorGUI.EndChangeCheck())
            {
                FolderIconSettingsSO.instance.Save();
            }
        }

        #region Static

        // NOTE: 設定パスは、ProjectSettingsフォルダ直下に配置する
        private static readonly string SettingPath = "Project/FolderIcon";

        [SettingsProvider]
        public static SettingsProvider CreateSettingProvider()
        {
            // Note: 第三引数のkeywordsは、検索時にこの設定項目を引っかけるためのキーワード
            return new FolderIconSettingsProvider(SettingPath, SettingsScope.Project, null);
        }
        #endregion
    }
}
#endif