using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace FolderIcon.Editor.Core
{
    public enum MatchType
    {
        /// <summary>完全一致</summary>
        FullMatch,
        /// <summary>部分一致</summary>
        PartialMatch,
    }


    [Serializable]
    public sealed class Pattern
    {
        [SerializeField] internal string _value;
        [SerializeField] internal MatchType _type = MatchType.FullMatch;

        private Regex _compiledRegex;
        private bool _isRegexCompiled = false;

        /// <summary>
        /// 正規表現パターンを取得する（キャッシュ付き）
        /// </summary>
        public Regex GetRegex()
        {
            if (_isRegexCompiled && _compiledRegex != null)
                return _compiledRegex;

            try
            {
                var regexString = _type switch
                {
                    MatchType.FullMatch => $"^{_value}$",
                    MatchType.PartialMatch => _value,
                    _ => throw new NotImplementedException($"MatchType {_type} is not implemented.")
                };

                _compiledRegex = new Regex(regexString, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                _isRegexCompiled = true;
                return _compiledRegex;
            }
            catch (ArgumentException)
            {
                // 無効な正規表現パターンの場合
                _compiledRegex = null;
                _isRegexCompiled = true;
                return null;
            }
        }

        /// <summary>
        /// パターンが有効かどうかをチェックする
        /// </summary>
        public bool IsValid()
        {
            if (string.IsNullOrEmpty(_value))
                return false;

            return GetRegex() != null;
        }

        /// <summary>
        /// フォルダ名がパターンにマッチするかチェックする
        /// </summary>
        /// <param name="folderPath">フォルダのパス</param>
        /// <returns>マッチする場合はtrue</returns>
        public bool IsMatch(string folderPath)
        {
            if (!IsValid())
                return false;

            // パスからフォルダ名のみを抽出
            var folderName = System.IO.Path.GetFileName(folderPath);
            if (string.IsNullOrEmpty(folderName))
                return false;

            var regex = GetRegex();
            return regex?.IsMatch(folderName) ?? false;
        }
    }
}