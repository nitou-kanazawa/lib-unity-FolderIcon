using System;
using UnityEngine;

namespace FolderIcon.Editor.Core
{
    /// <summary>
    /// フォルダアイコンの設定エントリ
    /// </summary>
    [Serializable]
    internal class FolderIconEntry
    {
        [SerializeField] private bool _isEnabled = true;
        [SerializeField] private Pattern _pattern;
        [SerializeField] private Texture2D _iconTexture = null;

        /// <summary>
        /// このエントリが有効かどうか
        /// </summary>
        public bool IsEnabled
        {
            get => _isEnabled;
            set => _isEnabled = value;
        }

        /// <summary>
        /// フォルダ名のマッチングパターン
        /// </summary>
        public Pattern Pattern => _pattern;

        /// <summary>
        /// 表示するアイコンテクスチャ
        /// </summary>
        public Texture2D IconTexture => _iconTexture;

        /// <summary>
        /// 指定されたフォルダパスがこのエントリにマッチするかチェックする
        /// </summary>
        /// <param name="folderPath">フォルダのパス</param>
        /// <returns>マッチする場合はtrue</returns>
        public bool IsMatch(string folderPath)
        {
            if (!_isEnabled || _pattern == null)
                return false;

            return _pattern.IsMatch(folderPath);
        }
    }
}
