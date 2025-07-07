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
    public sealed class FolderIconSettingsSO : ScriptableSingleton<FolderIconSettingsSO>
    {
        [SerializeField] private FolderIconEntryStore _entryStore;

        public FolderIconEntryStore EntryStore => _entryStore;

        public void Save() => Save(this);
    }
}
#endif