# フォルダアイコン パス判定機能 仕様書

## 1. 概要
プロジェクトウィンドウのフォルダアイコンを、フォルダパスに基づいて自動的に設定する機能。

## 2. 判定戦略

### 2.1 判定戦略の種類

| 戦略 | 説明 | 優先度 |
|------|------|--------|
| **ExactName** | フォルダ名の完全一致 | A |
| **Pattern** | ワイルドカード（`*`, `?`）によるパターンマッチ | C |
| **Path** | フォルダパスのパターンマッチ | B |
| **Regex** | 正規表現による高度なマッチング | B |
| **Attribute** | メタファイルやマーカーファイルによる指定 | A |
| **Hierarchical** | 親フォルダからの設定継承 | C |

### 2.2 優先順位
複数の条件がマッチした場合、優先度の高い設定を適用する。

## 3. 各戦略の仕様

### 3.1 ExactName（完全一致）
- **判定方法**: フォルダ名が設定値と完全に一致
- **大文字小文字**: 設定により区別/無視を選択可能
- **用途**: Unity標準フォルダ（Scripts, Editor, Resources等）

### 3.2 Pattern（ワイルドカード）
- **記法**:
  - `*` : 0文字以上の任意の文字列
  - `?` : 任意の1文字
- **例**:
  - `*Controller` : "Controller"で終わるフォルダ
  - `Test*` : "Test"で始まるフォルダ

### 3.3 Path（パス指定）
- **記法**:
  - `*/` : 任意の1階層
  - `**/` : 任意の深さの階層
- **例**:
  - `Assets/*/Prefabs` : Assets直下の各フォルダ内のPrefabs
  - `Assets/**/Tests` : Assets以下の任意の階層のTests

### 3.4 Regex（正規表現）
- **.NET正規表現構文**を使用
- **例**:
  - `\d{2,3}$` : 2-3桁の数字で終わる
  - `^Assets/(Game|Core)/` : Game又はCore配下

### 3.5 Attribute（属性ベース）
- **マーカーファイル**: `.foldericon` ファイルの存在で判定
- **メタファイル拡張**: `.meta`ファイルにカスタムプロパティ追加
- **自動判定**: フォルダ内容から自動的にアイコンを決定

### 3.6 Hierarchical（階層継承）
- **継承ルール**: 親フォルダの設定を子フォルダが自動継承
- **上書き設定**: 特定の子フォルダで個別設定可能
- **深さ制限**: 継承の最大階層数を指定可能

## 4. 設定データ構造

```csharp
[System.Serializable]
public class IconRule
{
    public IconMatchingStrategy strategy;  // 判定戦略
    public string pattern;                 // パターン文字列
    public Texture2D icon;                 // 適用するアイコン
    public Color tintColor;               // アイコンの色調整
    public bool caseSensitive;            // 大文字小文字の区別
    public int priority;                  // カスタム優先度
}
```

## 5. パフォーマンス要件

### 5.1 キャッシング
- 判定結果は必ずキャッシュする
- キャッシュサイズ上限: 1000エントリ（LRU方式）

### 5.2 処理順序
1. 完全一致（O(1)）
2. パスの深さでフィルタリング
3. パターンマッチング
4. 正規表現（最も重い処理）

### 5.3 描画最適化
- `EventType.Repaint` 時のみ描画処理実行
- 表示範囲外のフォルダは処理スキップ

## 6. デフォルト設定

### 6.1 Unity標準フォルダ

| フォルダ名 | アイコン |
|-----------|----------|
| Scripts | cs Script Icon |
| Editor | Folder Icon |
| Resources | FolderEmpty Icon |
| Plugins | dll Script Icon |
| StreamingAssets | MovieTexture Icon |

### 6.2 一般的なプロジェクトフォルダ

| パターン | アイコン | 戦略 |
|----------|----------|------|
| Prefabs | Prefab Icon | ExactName |
| Materials | Material Icon | ExactName |
| *Manager | SettingsIcon | Pattern |
| *Test* | TestIcon | Pattern |
| Assets/\*/UI | UI Icon | Path |

## 7. エラー処理

- 無効なパスは早期リターン
- アイコンロード失敗時はデフォルトアイコンを使用
- 正規表現のコンパイルエラーは起動時に検出・ログ出力

## 8. 拡張性

- 新しい判定戦略の追加が可能
- ScriptableObjectによる設定の永続化
- EditorWindowによるGUI設定画面の提供