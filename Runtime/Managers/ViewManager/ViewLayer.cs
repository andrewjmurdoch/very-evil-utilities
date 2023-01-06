using System;
using System.Collections.Generic;
using UnityEngine;

namespace VED.Utilities
{
    [Serializable]
    public class ViewLayer
    {
        public string ID => _id;
        [SerializeField] private string _id = string.Empty;

        public List<View> Views => _views;
        [SerializeField] private List<View> _views = new List<View>();
    }
}