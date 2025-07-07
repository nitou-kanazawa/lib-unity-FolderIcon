using System;
using System.Collections.Generic;
using UnityEngine;

namespace FolderIcon.Editor.Core
{
    /// <summary>
    /// フォルダアイコンのエントリを管理するクラス
    /// </summary>
    [Serializable]
    public class FolderIconEntryStore
    {
        [SerializeField]
        internal List<FolderIconEntry> _entries = new();

        public IReadOnlyList<FolderIconEntry> Entries => _entries;
    }
}
