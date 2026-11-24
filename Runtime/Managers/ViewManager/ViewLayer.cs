using System;
using System.Collections.Generic;
using UnityEngine;

namespace VED.Utilities
{
    [Serializable]
    public class ViewLayer
    {
        [SerializeField] private string     _id             = string.Empty;
        [SerializeField] private bool       _separateCamera = false;
        [SerializeField] private List<View> _views          = new List<View>();

        public string     ID             => _id;
        public bool       SeparateCamera => _separateCamera;
        public List<View> Views          => _views;
    }
}