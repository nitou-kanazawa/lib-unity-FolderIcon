# FolderIcon 仕様書

Unity エディタのプロジェクトウィンドウで、フォルダアイコンをカスタマイズ表示する Editor 拡張パッケージ。

- パッケージ名: `jp.nitou.foldericon`
- 対応 Unity: `6000.0` 以降
- 本書のステータス: **Draft v1.0**（実装前レビュー用）

---

## 1. 目的と方針

### 1.1 目的

プロジェクトウィンドウ上でフォルダの役割を視覚的に判別できるようにし、アセットナビゲーションを高速化する。

### 1.2 設計方針

実用性を最優先する。具体的には：

1. **導線が一周すること** — 「アイコンを設定する → 表示される → 解除できる」が迷わず行えること。
2. **挙動が予測できること** — どのルールが適用されたか直感的に分かるシンプルなマッチング仕様。
3. **チームで共有できること** — 設定は VCS にコミットでき、チーム全員が同じ表示を得られる。
4. **壊れないこと** — 設定が空・不正でもエディタの動作を阻害しない（例外を漏らさない）。

### 1.3 スコープ外（本バージョンでは実装しない）

| 項目 | 理由 / 備考 |
|------|------------|
| 正規表現マッチ | ワイルドカードで実用上十分。将来拡張として設計だけ考慮 |
| パス階層パターン（`Assets/**/Tests` 等） | 同上 |
| 親フォルダからの設定継承（Hierarchical） | 挙動の予測性を損なうため見送り |
| マーカーファイル方式（`.foldericon`） | リポジトリを汚すため見送り |
| 個人設定レイヤー（UserSettings） | 将来拡張。データ構造は分離を妨げない形にしておく |
| カラーフォルダ・バッジ素材の同梱 | 将来拡張（§9）。まず任意テクスチャ置換で成立させる |
| ランタイム機能 | 不要。Editor 専用（Runtime アセンブリは削除する） |
| ファイルへのアイコン適用 | フォルダのみ対象 |

---

## 2. 決定事項

レビューで論点となった項目の決定を明記する。

| # | 論点 | 決定 |
|---|------|------|
| D1 | 設定の保存スコープ | **プロジェクト共有**。`ScriptableSingleton` + `FilePath(ProjectSettings/FolderIconSettings.asset)` で永続化し、VCS にコミットする運用を想定 |
| D2 | マッチング方式 | **3 種類**: ①フォルダパス完全一致（個別指定）②フォルダ名完全一致 ③フォルダ名ワイルドカード。正規表現は将来拡張 |
| D3 | アイコン表現 | **任意 Texture2D による全置換**のみ。カラー＋バッジは将来拡張 |
| D4 | 主要な操作導線 | **右クリック即設定**を主導線とし、ProjectSettings のルール一覧管理を補助導線とする |
| D5 | 旧設計の扱い | `Matcher/`（`IFolderMatcher` 系）、`EntryId`、`Foundation/` は**削除**。`Pattern`/`FolderIconEntry` 系を正とし本仕様に沿って改修 |

---

## 3. 用語

| 用語 | 定義 |
|------|------|
| ルール（Rule） | 「マッチ条件 + アイコン」の 1 組。設定データの最小単位 |
| マッチ条件 | フォルダがルールの適用対象かを判定する条件（§4） |
| 適用アイコン | マッチしたフォルダに描画する `Texture2D` |
| 設定ストア | 全ルールを保持する永続化データ（§6） |

---

## 4. マッチング仕様

### 4.1 マッチ種別

| 種別 | 対象 | 判定 | 例 |
|------|------|------|-----|
| `FolderPath` | アセットパス全体 | 完全一致（大文字小文字区別あり※） | `Assets/App/Scripts` |
| `FolderName` | フォルダ名のみ | 完全一致 | `Prefabs` |
| `NamePattern` | フォルダ名のみ | ワイルドカード（`*`=0文字以上、`?`=任意1文字） | `*Manager`, `Test*` |

※ `FolderPath` は Unity の AssetDatabase が返すパスをそのまま比較する。`FolderName` / `NamePattern` は大文字小文字を**区別しない**（Windows の慣習に合わせる）。

### 4.2 優先順位

複数ルールがマッチした場合、以下の順で 1 つだけ適用する：

1. **種別の優先度**: `FolderPath` > `FolderName` > `NamePattern`
   （具体的な指定ほど強い。「このフォルダだけ変えたい」が常にルールに勝つ）
2. 同種別内では**リストの上にあるルールが優先**（ProjectSettings で並べ替え可能）

タイブレークが暗黙にならないよう、UI 上で「このフォルダに現在適用されているルール」を確認できること（§7.2）。

### 4.3 判定の入出力

```
入力: フォルダのアセットパス（例 "Assets/App/Scripts"）
出力: 適用アイコン（Texture2D）または null（カスタマイズなし）
```

- 無効化されたルール（`enabled == false`）はマッチ対象外。
- アイコン未設定（null）のルールはマッチ対象外。
- パスが `Assets/` または `Packages/` 配下のフォルダでない場合は早期リターン。

### 4.4 ワイルドカードの内部実装

`NamePattern` は入力文字列を **`Regex.Escape` した上で** `*` → `.*`、`?` → `.` に置換して正規表現へ変換する。ユーザー入力がメタ文字（`+` `(` 等）を含んでも文字どおり扱う。変換した Regex はコンパイルしてキャッシュし、**パターン文字列が変更されたらキャッシュを無効化**する（現行実装のキャッシュ更新漏れバグを修正）。

---

## 5. 描画仕様

### 5.1 フック

`EditorApplication.projectWindowItemOnGUI` を使用する。

- `EventType.Repaint` 時のみ処理する。
- Play モード中も描画する（エディタ操作は Play 中も行われるため。現行実装の `Application.isPlaying` チェックは撤廃）。

### 5.2 表示モード対応

プロジェクトウィンドウの 3 表示形態すべてに対応する：

| モード | 判定 | 描画 |
|--------|------|------|
| ツリービュー（1 カラム / 2 カラム左ペイン） | `rect.height <= 20 && rect.x <= 20` 相当 | 行頭の標準フォルダアイコン位置（16px 相当）に上書き |
| リストビュー（2 カラム右・小サイズ） | `rect.height <= 20 && rect.x > 20` | 同上 |
| グリッドビュー（2 カラム右・大サイズ） | `rect.height > 20` | サムネイル領域（`rect.width` 基準の正方形）に上書き |

描画は `GUI.DrawTexture(rect, icon, ScaleMode.ScaleToFit, alphaBlend: true)`。フォルダの標準アイコンの**上に重ね描き**する方式（背景消去はしない。全置換に見えるよう不透明アイコンを推奨する旨をドキュメントに記載）。

### 5.3 パフォーマンス

- **判定結果をキャッシュする**: `Dictionary<string(path), Texture2D>`。miss 時のみルール走査。
- キャッシュ無効化タイミング:
  - 設定変更時（ルール追加/削除/編集/並べ替え/有効切替） → 全クリア
  - アセットリネーム・移動・削除時（`AssetPostprocessor.OnPostprocessAllAssets`） → 全クリア（部分無効化はしない。頻度が低く全クリアで十分）
- ルール走査は LINQ を使わず for ループ（Repaint 毎のアロケーション回避）。
- 上限などの制約: ルール数上限は設けない。ただし 1000 件を超えたら Warning ログを 1 回出す。

---

## 6. データ仕様

### 6.1 永続化

- `ScriptableSingleton<FolderIconSettings>` + `[FilePath("ProjectSettings/FolderIconSettings.asset", Location.ProjectFolder)]`
- テキストシリアライズされ、アイコンへの参照は GUID で保持される（プロジェクト間でアセットが揃っていれば共有可能）。
- **チーム共有のため VCS にコミットする**ことを README に明記する。
- 現行の `ProjectSettings/Nitou/FolderIcon.dat` は廃止（後方互換は取らない。v1.0 到達前のため）。

### 6.2 データモデル

```csharp
enum FolderMatchType { FolderPath, FolderName, NamePattern }

[Serializable]
class FolderIconRule
{
    bool            enabled = true;
    FolderMatchType matchType;
    string          pattern;      // パス / 名前 / ワイルドカード文字列
    Texture2D       icon;
}

class FolderIconSettings : ScriptableSingleton<FolderIconSettings>
{
    bool                 isEnabled = true;   // 機能全体の ON/OFF
    List<FolderIconRule> rules;              // null 不可。デシリアライズ後に必ず non-null を保証
    int                  version = 1;        // 将来のマイグレーション用
}
```

- **null 安全**: `rules` は `OnEnable` で null なら生成する（現行の `_entryStore` 未初期化 NRE を修正）。
- `version` フィールドをデータに含め、将来フォーマット変更時のマイグレーションを可能にする。

### 6.3 バリデーション

| 状態 | 扱い |
|------|------|
| `pattern` が空 | マッチ対象外（エラーにしない）。UI 上で警告表示 |
| `icon` が null（参照切れ含む） | マッチ対象外。UI 上で警告表示 |
| 同一 `matchType` + `pattern` の重複 | 許容する（上優先ルールが勝つ）。UI 上で情報表示 |

---

## 7. UI 仕様

### 7.1 コンテキストメニュー（主導線）

`Assets/Set Folder Icon...`（優先度 2000、メニュー表記は英語に統一）

- フォルダ選択時のみ有効（validate ハンドラで制御）。
- 実行すると小型のユーティリティウィンドウを表示：
  - 対象フォルダのパス表示
  - アイコン選択（`ObjectField`）＋ 現在の適用結果プレビュー
  - **[適用]**: `FolderPath` 完全一致ルールとして保存（既存の同パスルールがあれば上書き）
  - **[解除]**: 対象パスの `FolderPath` ルールを削除（存在する場合のみ表示）
- 適用/解除の即時反映（`EditorApplication.RepaintProjectWindow()`）。
- 複数フォルダ選択時は選択中の全フォルダに同一アイコンを適用する。

### 7.2 ProjectSettings（補助導線）

`Project Settings > Folder Icon`

- 機能全体の ON/OFF トグル。
- ルール一覧を **ReorderableList** で表示・編集：
  - 各行: enabled トグル / matchType / pattern / icon
  - ドラッグで並べ替え（同種別内の優先順位）
  - 追加・削除
- バリデーション警告の表示（§6.3）。
- 変更は即座に `Save()` し、判定キャッシュをクリアする。
- 検索キーワード（"folder", "icon" 等）を SettingsProvider に登録する。

### 7.3 多言語

UI 文言は**英語のみ**とする（パッケージとして配布するため。現行実装の日本語メニューは置き換え）。

---

## 8. エラー処理・堅牢性

- `projectWindowItemOnGUI` ハンドラ内は例外を外に漏らさない（描画毎のログ洪水を防ぐため、同一例外は初回のみ `Debug.LogException`）。
- 不正なパターンや参照切れアイコンがあっても他のルールの動作に影響しない。
- 設定ファイルが手動編集などで壊れていた場合、既定値（空ルール・機能 ON）で起動する。

---

## 9. 将来拡張（本仕様では設計余地のみ確保）

優先度順：

1. **同梱アイコンセット**（カラーフォルダ + バッジ）: 選ぶだけで使える体験。`icon` が Texture2D 参照である限りデータ互換のまま追加可能
2. **正規表現 / パスパターンマッチ**: `FolderMatchType` に列挙子を追加する形で拡張。優先順位は `FolderPath > FolderName > NamePattern > Regex` とする想定
3. **個人設定レイヤー**: `UserSettings/` 配下に同スキーマの上書きストアを追加
4. **インポート / エクスポート**: ルールセットの JSON 入出力（チーム間・プロジェクト間の移送用）
5. サブアイコン（ツリービューで標準アイコンの右に小さく併記）

---

## 10. 実装マイルストーン

### M1 — MVP（導線を一周させる）
- [ ] `FolderIconSettings` の null 安全化・新スキーマ化（§6）
- [ ] マッチング実装（3 種別 + 優先順位 + ワイルドカード修正）（§4）
- [ ] 判定キャッシュ（§5.3）
- [ ] コンテキストメニュー → 設定ウィンドウの保存/解除処理（§7.1）
- [ ] 旧設計の削除（`Matcher/`, `EntryId`, `Foundation/`, Runtime アセンブリ）（D5）

### M2 — ルール管理
- [ ] ProjectSettings の ReorderableList UI（§7.2）
- [ ] バリデーション表示（§6.3）
- [ ] `AssetPostprocessor` によるキャッシュ無効化

### M3 — 配布品質
- [ ] README 更新（typo `flder-icon` 修正、共有運用の記載、スクリーンショット）
- [ ] package.json 整備（keywords, CHANGELOG, Samples~）
- [ ] EditMode テスト（マッチング・優先順位・キャッシュ無効化）
