
## 要件定義
- 目的: Unityエディタ上でプロジェクトウィンドウのフォルダアイコンをカスタマイズできるようにする
- 対象: Editor拡張のみ（ランタイム不要）
- データ:
  - フォルダ名のパターン（正規表現等）
  - アイコン画像（Texture2D）
- データ管理:
  - ScriptableObjectで一元管理
  - 常に1つのみ存在（シングルトン的運用）
  - ProjectSettingsウィンドウから編集可能
- 拡張性:
  - デフォルトデータのダウンロード・インポート機能
  - ユーザー定義データの保存・編集

---
## 設計

#### データ定義
- `Pattern`（既存）：フォルダ名の一致条件
- `FolderIconEntry`（既存）：Pattern＋Texture2D＋有効/無効
- `FolderIconEntryStore`（既存）：Entryリスト
- `FolderIconSettingSO`（既存）：ScriptableObjectでエントリ管理

#### データ管理
- `FolderIconSettingSO`をプロジェクト内に1つだけ生成・管理
- ProjectSettingsProviderでProjectSettingsウィンドウにインスペクタを表示
- デフォルトデータのインポート機能（メニューからDL/インポート）

#### データ活用（描画）
- Projectウィンドウの描画フック（EditorApplication.projectWindowItemOnGUI等）で
- フォルダ名とPatternを照合
- 該当する場合、Texture2Dでアイコン描画

---