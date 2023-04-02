using System;
using UnityEngine;

namespace VED.Utilities
{
    public sealed partial class Easing
    {
        [Serializable]
        public class Controller
        {
            public Shape Shape => _shape;
            [SerializeField] private Shape _shape = Shape.SINE;

            public Extent Extent => _extent;
            [SerializeField] private Extent _extent = Extent.OUT;

            public Controller(Shape shape, Extent extent)
            {
                _shape = shape;
                _extent = extent;
            }

            public float Ease(float a, float b, float t)
            {
                return Easing.Ease(_shape, _extent, a, b, t);
            }

            public Vector3 Ease(Vector3 a, Vector3 b, float t)
            {
                return Easing.Ease(_shape, _extent, a, b, t);
            }
        }
    }
}