using System;
using UnityEngine;

namespace FolderIcon.Editor.Core
{

    [Serializable]
    public class FolderIconEntry
    {
        [HideInInspector]
        [SerializeField] private string _id = Guid.NewGuid().ToString();
        [SerializeField] private bool _isEnabled = true;
        [SerializeField] private Pattern _pattern;
        [SerializeField] private Texture2D _iconTexture;

        public bool IsEnabled
        {
            get => _isEnabled;
            set => _isEnabled = value;
        }
    }
}
