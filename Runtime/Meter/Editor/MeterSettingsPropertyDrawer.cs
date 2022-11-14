#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace VED.Utilities
{
    [CustomPropertyDrawer(typeof(MeterSettings))]
    public class MeterSettingsPropertyDrawer : PropertyDrawer
    {
        private const int RATIO = 5;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUIStyle style = new GUIStyle();
            style.richText = true;
            Rect rectName = new Rect(position.min.x, position.min.y, position.width / RATIO, position.height);
            EditorGUI.LabelField(rectName, "<b><color=#c0c0c0ff>" + property.displayName + "</color></b>", style);

            SerializedProperty propMinimum = property.FindPropertyRelative("Minimum");
            SerializedProperty propMaximum = property.FindPropertyRelative("Maximum");
            SerializedProperty propInitial = property.FindPropertyRelative("Initial");

            int[] values = new int[]
            {
                propMinimum.intValue,
                propMaximum.intValue,
                propInitial.intValue
            };

            GUIContent[] labels = new GUIContent[]
            {
                new GUIContent("Minimum"),
                new GUIContent("Maximum"),
                new GUIContent("Initial")
            };

            Rect rectMulti = new Rect(position.min.x + position.width / RATIO, position.min.y, (RATIO - 1) * position.width / RATIO, position.height);
            EditorGUI.MultiIntField(rectMulti, labels, values);

            propMinimum.intValue = values[0];
            propMaximum.intValue = values[1];
            propInitial.intValue = values[2];
        }
    }
}
#endif