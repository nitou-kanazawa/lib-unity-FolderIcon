#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FolderIcon.Editor.Core.Settings
{
    /// <summary>
    /// FolderIconパッケージの設定を管理するScriptableSingleton．
    /// プロジェクト設定として永続化され、フォルダアイコンの有効/無効状態と
    /// アイコンエントリの管理を行う．
    /// </summary>
    [FilePath(
        relativePath: "ProjectSettings/Nitou/FolderIcon.dat",
        location: FilePathAttribute.Location.ProjectFolder
    )]
    internal sealed class FolderIconSettingsSO : ScriptableSingleton<FolderIconSettingsSO>
    {
        [SerializeField] private bool _isEnabled = true;
        [SerializeField] private FolderIconEntryStore _entryStore;

        /// <summary>
        /// フォルダアイコン機能が有効かどうかを取得する
        /// </summary>
        public bool IsEnabled => _isEnabled;

        /// <summary>
        /// フォルダアイコンエントリの管理ストアを取得する
        /// </summary>
        public FolderIconEntryStore EntryStore => _entryStore;


        /// <summary>
        /// 現在の設定をプロジェクト設定ファイルに保存する
        /// </summary>
        public void Save() => Save(this);
    }
}
#endif