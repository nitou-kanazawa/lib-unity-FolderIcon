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

        public Regex GetRegex()
        {
            var regexString = _type switch
            {
                MatchType.FullMatch => $"^{_value}$",
                MatchType.PartialMatch => _value,
                _ => throw new NotImplementedException($"MatchType {_value} is not implemented.")
            };
            return new Regex(regexString, RegexOptions.Compiled);
        }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(_value))
                return false;

            // 正規表現として有効かチェック
            return GetRegex() != null;
        }

        public bool IsMatch(string path)
        {
            if (!IsValid()) return false;
            return GetRegex().IsMatch(path);
        }
    }
}