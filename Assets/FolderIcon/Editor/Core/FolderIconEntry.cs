using System;
using UnityEngine;

namespace FolderIcon.Editor.Core
{

    [Serializable]
    internal class FolderIconEntry
    {
        [SerializeField] private bool _isEnabled = true;
        [SerializeField] private Pattern _pattern;
        [SerializeField] private Texture2D _iconTexture = null;

        public bool IsEnabled
        {
            get => _isEnabled;
            set => _isEnabled = value;
        }

        public Pattern Pattern => _pattern;

        public Texture2D IconTexture => _iconTexture;
    }
}
