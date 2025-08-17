using System;

namespace FolderIcon.Editor.Core.Matcher
{
    public enum MatchMethod
    {
        /// <summary>完全一致</summary>
        Exact,
        /// <summary>ワイルドカードによるパターンマッチ</summary>
        Pattern,
        /// <summary>正規表現によるマッチング</summary>
        Regex,
    }

    public interface IFolderMatcher
    {
        bool IsMatch(string folderPath);
        int GetPriority();
    }

}