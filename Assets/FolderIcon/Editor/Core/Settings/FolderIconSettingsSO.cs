#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FolderIcon.Editor.Core.Settings
{
    [FilePath(
        relativePath: "ProjectSettings/Nitou/FolderIcon.dat",
        location: FilePathAttribute.Location.ProjectFolder
    )]
    internal sealed class FolderIconSettingsSO : ScriptableSingleton<FolderIconSettingsSO>
    {
        [SerializeField] private bool _isEnabled = true;
        [SerializeField] private FolderIconEntryStore _entryStore;

        /// <summary>
        /// 
        /// </summary>
        public bool IsEnabled => _isEnabled;

        /// <summary>
        /// 
        /// </summary>
        public FolderIconEntryStore EntryStore => _entryStore;


        public void Save() => Save(this);
    }
}
#endif