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

        /// <summary>
        /// 登録されているエントリの読み取り専用リスト
        /// </summary>
        public IReadOnlyList<FolderIconEntry> Entries => _entries;

        /// <summary>
        /// 指定されたフォルダパスにマッチするエントリを取得する
        /// </summary>
        /// <param name="folderPath">フォルダのパス</param>
        /// <returns>マッチするエントリ、見つからない場合はnull</returns>
        internal FolderIconEntry GetEntry(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath) || _entries == null)
                return null;

            // 有効なエントリから最初にマッチするものを返す
            return _entries.FirstOrDefault(entry => entry.IsMatch(folderPath));
        }

        /// <summary>
        /// エントリを追加する
        /// </summary>
        /// <param name="entry">追加するエントリ</param>
        internal void AddEntry(FolderIconEntry entry)
        {
            if (entry != null)
                _entries.Add(entry);
        }

        /// <summary>
        /// エントリを削除する
        /// </summary>
        /// <param name="entry">削除するエントリ</param>
        internal void RemoveEntry(FolderIconEntry entry)
        {
            if (entry != null)
                _entries.Remove(entry);
        }

        /// <summary>
        /// 全てのエントリをクリアする
        /// </summary>
        internal void ClearEntries()
        {
            _entries.Clear();
        }
    }
}