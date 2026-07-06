using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FolderIcon.Editor
{
    /// <summary>
    /// FolderIconパッケージのプロジェクト設定（ScriptableSingleton）．
    /// テキストシリアライズで ProjectSettings/ 配下に永続化され、VCSにコミットして
    /// チームで共有する運用を想定する（仕様書 §8.1）．
    /// </summary>
    [FilePath(
        relativePath: "ProjectSettings/FolderIconSettings.asset",
        location: FilePathAttribute.Location.ProjectFolder
    )]
    internal sealed class FolderIconSettings : ScriptableSingleton<FolderIconSettings>
    {
        [SerializeField] private bool _isEnabled = true;
        [SerializeField] private bool _useDefaultPreset = true;
        [SerializeField] private bool _ignoreCase = true;
        [SerializeField] private bool _ignoreLeadingUnderscore = true;
        [SerializeField] private List<FolderIconRule> _userRules = new();
        [SerializeField] private int _version = 1;

        /// <summary>機能全体のON/OFF</summary>
        public bool IsEnabled => _isEnabled;

        /// <summary>同梱デフォルトプリセットのレイヤーを使用するか（プリセット本体はM2で導入）</summary>
        public bool UseDefaultPreset => _useDefaultPreset;

        /// <summary>名前正規化: 大文字小文字を無視する</summary>
        public bool IgnoreCase => _ignoreCase;

        /// <summary>名前正規化: 先頭のアンダースコアを無視する</summary>
        public bool IgnoreLeadingUnderscore => _ignoreLeadingUnderscore;

        /// <summary>ユーザー定義ルール（リスト上位が優先）</summary>
        public IReadOnlyList<FolderIconRule> UserRules => _userRules;

        /// <summary>設定データのフォーマットバージョン（将来のマイグレーション用）</summary>
        public int Version => _version;

        private void OnEnable()
        {
            // 手動編集などでリストが欠落していても既定値で動作継続する（§9）
            _userRules ??= new List<FolderIconRule>();
        }


        // ----------------------------------------------------------------------------
        // ユーザールール操作

        /// <summary>
        /// 指定パスに対する FolderPath 完全一致ルールを追加する．
        /// 同一パスの既存ルールがあればアイコンを上書きする（§7.1）．
        /// </summary>
        public void SetPathRule(string folderPath, Texture2D icon)
        {
            if (string.IsNullOrEmpty(folderPath) || icon == null)
                return;

            var rule = FindPathRule(folderPath);
            if (rule != null)
            {
                rule.Enabled = true;
                rule.Icon = icon;
            }
            else
            {
                _userRules.Add(new FolderIconRule(FolderMatchType.FolderPath, folderPath, icon));
            }
        }

        /// <summary>
        /// 指定パスに対する FolderPath 完全一致ルールを取得する．存在しなければnull．
        /// </summary>
        public FolderIconRule FindPathRule(string folderPath)
        {
            foreach (var rule in _userRules)
            {
                if (rule.MatchType == FolderMatchType.FolderPath && rule.Pattern == folderPath)
                    return rule;
            }
            return null;
        }

        /// <summary>
        /// 指定パスに対する FolderPath 完全一致ルールを削除する．
        /// </summary>
        /// <returns>1件以上削除した場合はtrue</returns>
        public bool RemovePathRule(string folderPath)
        {
            int removed = _userRules.RemoveAll(rule =>
                rule.MatchType == FolderMatchType.FolderPath && rule.Pattern == folderPath);
            return removed > 0;
        }


        // ----------------------------------------------------------------------------
        // 永続化

        /// <summary>
        /// 現在の設定を保存し、判定キャッシュのクリアとプロジェクトウィンドウの再描画を行う．
        /// </summary>
        public void Save()
        {
            Save(saveAsText: true);
            FolderIconResolver.ClearCache();
            EditorApplication.RepaintProjectWindow();
        }
    }
}
