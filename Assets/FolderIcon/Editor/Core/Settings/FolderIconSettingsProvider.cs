using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace FolderIcon.Editor
{
    /// <summary>
    /// ProjectSettingsウィンドウに設定画面を表示するプロバイダ（仕様書 §7.2）．
    /// M1では標準インスペクタの表示のみ．ReorderableList等の専用UIはM3で実装する．
    /// </summary>
    internal sealed class FolderIconSettingsProvider : SettingsProvider
    {
        private UnityEditor.Editor _editor;

        private FolderIconSettingsProvider(string path, SettingsScope scopes)
            : base(path, scopes)
        {
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            // ScriptableSingletonは既定でNotEditableのため、編集可能に変更する
            var settings = FolderIconSettings.instance;
            settings.hideFlags = HideFlags.HideAndDontSave & ~HideFlags.NotEditable;

            UnityEditor.Editor.CreateCachedEditor(settings, null, ref _editor);
        }

        public override void OnGUI(string searchContext)
        {
            if (_editor == null)
                return;

            EditorGUI.BeginChangeCheck();
            _editor.OnInspectorGUI();
            if (EditorGUI.EndChangeCheck())
            {
                FolderIconSettings.instance.Save();
            }
        }

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            return new FolderIconSettingsProvider("Project/Folder Icon", SettingsScope.Project)
            {
                keywords = new[] { "folder", "icon", "project window" },
            };
        }
    }
}
