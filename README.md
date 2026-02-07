# Folder Icon

プロジェクトのフォルダアイコンをカスタマイズできる Unity Package．

[![license](https://img.shields.io/badge/LICENSE-MIT-green.svg)](LICENSE)

## 🌀 開発環境
- Unity `6000.0.30f1`

## 🌀 機能


機能要件
1. フォルダアイコン描画機能
2. フォルダ判定機能
3. アイコンリソース管理
4. 設定データ管理
  4.1 設定データ構造
  4.2 保存場所
5. インポート・エクスポート機能
6. GUI設定画面
  6.1 ProjectSettings統合
7 コンテキストメニュー機能
8. Package Manager対応

非機能要件
- 制約事項
  - 最大ルール数
  - フォルダのみ対象（ファイルは非対応）


## 🌀 セットアップ

#### インストール

1. Window > Package ManagerからPackage Managerを開く
2. 「+」ボタン > Add package from git URL
3. 以下のURLを入力する
```
https://github.com/nitou-kanazawa/lib-unity-FolderIcon.git?path=Assets/FolderIcon
```

あるいはPackages/manifest.jsonを開き、dependenciesブロックに以下を追記
```
{
    "dependencies": {
        "jp.nitou.flder-icon": "https://github.com/nitou-kanazawa/lib-unity-FolderIcon.git?path=Assets/FolderIcon"
    }
}
```

