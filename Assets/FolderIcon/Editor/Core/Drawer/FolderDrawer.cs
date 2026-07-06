using System;
using UnityEditor;
using UnityEngine;

namespace FolderIcon.Editor
{
    /// <summary>
    /// プロジェクトウィンドウのフォルダにカスタムアイコンを重ね描きするクラス（仕様書 §6）．
    /// ツリービュー・リストビュー・グリッドビューの3表示形態に対応する．
    /// </summary>
    [InitializeOnLoad]
    internal static class FolderDrawer
    {
        // 表示モード判定と位置調整の定数（実測による調整値）
        private const float GridViewHeightThreshold = 20f;   // これより高い矩形はグリッドビュー
        private const float TreeViewXThreshold = 20f;        // これ以下のxはツリービュー
        private const float IconSizeExpansion = 2f;
        private const float IconXOffset = -1f;
        private const float IconYOffset = -1f;
        private const float TreeViewXOffset = 2f;

        private static bool _exceptionLogged;

        static FolderDrawer()
        {
            EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;
        }

        private static void OnProjectWindowItemGUI(string guid, Rect selectionRect)
        {
            if (Event.current.type != EventType.Repaint)
                return;

            // 描画フックから例外を漏らさない（§9）
            try
            {
                DrawFolderIcon(guid, selectionRect);
            }
            catch (Exception e)
            {
                // Repaint毎に呼ばれるため、ログ洪水を防いで初回のみ報告する
                if (_exceptionLogged) return;
                _exceptionLogged = true;
                Debug.LogException(e);
            }
        }

        private static void DrawFolderIcon(string guid, Rect selectionRect)
        {
            if (!FolderIconSettings.instance.IsEnabled)
                return;

            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (!IsTargetPath(path) || !AssetDatabase.IsValidFolder(path))
                return;

            var icon = FolderIconResolver.Resolve(path);
            if (icon == null)
                return;

            var iconRect = GetIconRect(selectionRect);
            GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit, alphaBlend: true);
        }

        /// <summary>
        /// Assets/ または Packages/ 配下のみ対象とする（§4.4）．
        /// </summary>
        private static bool IsTargetPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            return path == "Assets"
                || path.StartsWith("Assets/", StringComparison.Ordinal)
                || path.StartsWith("Packages/", StringComparison.Ordinal);
        }

        /// <summary>
        /// 表示モードに応じたアイコンの描画領域を計算する（§6.2）．
        /// </summary>
        private static Rect GetIconRect(Rect rect)
        {
            // グリッドビュー: サムネイル領域（幅基準の正方形）に描画
            if (rect.height > GridViewHeightThreshold)
            {
                float size = rect.width + IconSizeExpansion;
                return new Rect(rect.x + IconXOffset, rect.y + IconYOffset, size, size);
            }

            // ツリービュー / リストビュー: 行頭の標準アイコン位置に描画
            float iconSize = rect.height + IconSizeExpansion;
            float xOffset = rect.x > TreeViewXThreshold ? IconXOffset : TreeViewXOffset;
            return new Rect(rect.x + xOffset, rect.y + IconYOffset, iconSize, iconSize);
        }
    }
}
