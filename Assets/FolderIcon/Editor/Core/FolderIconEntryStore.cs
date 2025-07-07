using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FolderIcon.Editor.Core
{
    /// <summary>
    /// フォルダアイコンのエントリを管理するクラス
    /// </summary>
    [Serializable]
    internal class FolderIconEntryStore
    {
        [SerializeField]
        internal List<FolderIconEntry> _entries = new();

        public IReadOnlyList<FolderIconEntry> Entries => _entries;

        internal FolderIconEntry GetEntry(string path)
        {
            return _entries.FirstOrDefault(entry => entry.Pattern.IsMatch(path));
        }
    }
}