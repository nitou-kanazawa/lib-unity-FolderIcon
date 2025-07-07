using System;
using UnityEngine;

namespace FolderIcon.Editor.Core
{
    [Serializable]
    public abstract class EntryId
    {
        [SerializeField] private string _value;

        public string Value
        {
            get => _value;
            set => _value = value;
        }
    }


}
