using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FolderIcon.Editor
{
    /// <summary>
    /// アイコンの適用元（どのレイヤーのルールにマッチしたか）．
    /// </summary>
    internal enum FolderIconSource
    {
        None,
        UserPathRule,
        UserNameRule,
        // Preset: M2で追加予定
    }


    /// <summary>
    /// フォルダパスから適用アイコンを解決するクラス（仕様書 §4）．
    ///
    /// 優先順位（§4.3）:
    ///   1. ユーザールール > プリセットルール（プリセットはM2で導入）
    ///   2. 同一レイヤー内では FolderPath > FolderName
    ///   3. 同順位ならリスト上位が優先
    ///
    /// パフォーマンス（§6.3）:
    ///   - 解決結果は パス→アイコン の辞書にキャッシュする（マッチなしのnullも記録）
    ///   - ルールは種別ごとの辞書に事前展開し、判定を O(1) にする
    /// </summary>
    internal static class FolderIconResolver
    {
        // パスごとの解決結果キャッシュ（マッチしなかった場合のnullも保持する）
        private static readonly Dictionary<string, Texture2D> _resultCache = new();

        // ユーザールールの事前展開辞書（nullなら次回解決時に再構築）
        private static Dictionary<string, Texture2D> _userPathLookup;
        private static Dictionary<string, Texture2D> _userNameLookup;

        /// <summary>
        /// 全キャッシュを破棄する．設定変更時とアセット変更時に呼ぶ．
        /// </summary>
        public static void ClearCache()
        {
            _resultCache.Clear();
            _userPathLookup = null;
            _userNameLookup = null;
        }

        /// <summary>
        /// フォルダパスに適用するアイコンを解決する．カスタマイズなしの場合はnull．
        /// </summary>
        public static Texture2D Resolve(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
                return null;

            if (_resultCache.TryGetValue(folderPath, out var cached))
                return cached;

            var icon = ResolveCore(folderPath, out _);
            _resultCache[folderPath] = icon;
            return icon;
        }

        /// <summary>
        /// アイコンを適用元情報つきで解決する（設定ウィンドウの状態表示用．キャッシュは使わない）．
        /// </summary>
        public static Texture2D Resolve(string folderPath, out FolderIconSource source)
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                source = FolderIconSource.None;
                return null;
            }
            return ResolveCore(folderPath, out source);
        }

        /// <summary>
        /// フォルダ名を比較用に正規化する（§4.2）．
        /// 正規化後に空文字になった場合はマッチ対象外として扱うこと．
        /// </summary>
        public static string NormalizeName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return string.Empty;

            var settings = FolderIconSettings.instance;
            if (settings.IgnoreLeadingUnderscore)
                name = name.TrimStart('_');
            if (settings.IgnoreCase)
                name = name.ToLowerInvariant();
            return name;
        }


        // ----------------------------------------------------------------------------

        private static Texture2D ResolveCore(string folderPath, out FolderIconSource source)
        {
            BuildLookupsIfNeeded();

            // ユーザーレイヤー: FolderPath > FolderName
            if (_userPathLookup.TryGetValue(folderPath, out var icon))
            {
                source = FolderIconSource.UserPathRule;
                return icon;
            }

            var normalizedName = NormalizeName(System.IO.Path.GetFileName(folderPath));
            if (normalizedName.Length > 0 && _userNameLookup.TryGetValue(normalizedName, out icon))
            {
                source = FolderIconSource.UserNameRule;
                return icon;
            }

            // プリセットレイヤー: M2で導入（FolderIconSettings.UseDefaultPreset で制御）

            source = FolderIconSource.None;
            return null;
        }

        private static void BuildLookupsIfNeeded()
        {
            if (_userPathLookup != null && _userNameLookup != null)
                return;

            _userPathLookup = new Dictionary<string, Texture2D>();
            _userNameLookup = new Dictionary<string, Texture2D>();

            // リスト上位優先: 先頭から走査し、既登録キーは上書きしない（§4.3）
            var rules = FolderIconSettings.instance.UserRules;
            for (int i = 0; i < rules.Count; i++)
            {
                var rule = rules[i];
                if (!rule.Enabled || !rule.IsValid)
                    continue;

                switch (rule.MatchType)
                {
                    case FolderMatchType.FolderPath:
                        _userPathLookup.TryAdd(rule.Pattern, rule.Icon);
                        break;

                    case FolderMatchType.FolderName:
                        var key = NormalizeName(rule.Pattern);
                        if (key.Length > 0)
                            _userNameLookup.TryAdd(key, rule.Icon);
                        break;
                }
            }
        }
    }


    /// <summary>
    /// フォルダのリネーム・移動・削除で判定キャッシュが陳腐化するため、
    /// アセット変更時にキャッシュを全クリアする（§6.3．部分無効化はしない）．
    /// </summary>
    internal sealed class FolderIconAssetPostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            FolderIconResolver.ClearCache();
        }
    }
}
