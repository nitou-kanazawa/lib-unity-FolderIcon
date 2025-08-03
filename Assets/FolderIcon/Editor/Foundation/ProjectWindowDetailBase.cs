#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Nitou.Editor
{
    /// <summary>
    /// Base class for project window details.
    /// </summary>
    public abstract class ProjectWindowDetailBase
    {
        private const string ShowPrefsKey = "ProjectWindowDetails.Show.";
        public TextAlignment Alignment = TextAlignment.Left;
        public abstract string Name { get; }
        public virtual int ColumnWidth { get; } = 100;

        public bool Visible
        {
            get => EditorPrefs.GetBool(string.Concat(ShowPrefsKey, Name));
            set => EditorPrefs.SetBool(string.Concat(ShowPrefsKey, Name), value);
        }

        public abstract string GetLabel(string guid, string assetPath, Object asset);
    }


    public class GuidDetail : ProjectWindowDetailBase
    {
        public override string Name { get; } = "Guid";
        public override int ColumnWidth { get; } = 230;


        public override string GetLabel(string guid, string assetPath, Object asset)
        {
            return guid;
        }
    }
}
#endif