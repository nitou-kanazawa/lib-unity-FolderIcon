using System;
using UnityEngine;

namespace FolderIcon.Editor
{
    /// <summary>
    /// フォルダとアイコンを対応付けるマッチ条件の種別．
    /// </summary>
    public enum FolderMatchType
    {
        /// <summary>アセットパス全体の完全一致（右クリックからの個別指定用）</summary>
        FolderPath,

        /// <summary>フォルダ名の一致（名前正規化つき）．基本運用はこちら</summary>
        FolderName,
    }


    /// <summary>
    /// 「マッチ条件 + アイコン」1組を表すルール．設定データの最小単位．
    /// </summary>
    [Serializable]
    public sealed class FolderIconRule
    {
        [SerializeField] private bool _enabled = true;
        [SerializeField] private FolderMatchType _matchType = FolderMatchType.FolderName;
        [SerializeField] private string _pattern;
        [SerializeField] private Texture2D _icon;

        /// <summary>このルールが有効かどうか</summary>
        public bool Enabled
        {
            get => _enabled;
            set => _enabled = value;
        }

        /// <summary>マッチ条件の種別</summary>
        public FolderMatchType MatchType
        {
            get => _matchType;
            set => _matchType = value;
        }

        /// <summary>マッチ対象のパスまたはフォルダ名</summary>
        public string Pattern
        {
            get => _pattern;
            set => _pattern = value;
        }

        /// <summary>適用するアイコン</summary>
        public Texture2D Icon
        {
            get => _icon;
            set => _icon = value;
        }

        /// <summary>
        /// マッチ判定の対象にできるルールかどうか．
        /// パターンが空、またはアイコン未設定（参照切れ含む）のルールは対象外（§8.3）．
        /// </summary>
        public bool IsValid => !string.IsNullOrEmpty(_pattern) && _icon != null;

        public FolderIconRule() { }

        public FolderIconRule(FolderMatchType matchType, string pattern, Texture2D icon)
        {
            _matchType = matchType;
            _pattern = pattern;
            _icon = icon;
        }
    }
}
