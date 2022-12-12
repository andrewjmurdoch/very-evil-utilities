#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace VED.Utilities
{
    [CustomPropertyDrawer(typeof(Meter))]
    public class MeterPropertyDrawer : PropertyDrawer
    {
        private const int RATIO = 3;
        private const float ARRAY_ICON_HEIGHT = 35f;
        private const float ARRAY_ELEMENT_HEIGHT = 20f;
        private const float SPACER = 2f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // 3 due to having 3 lines (min/max/value, rules, events) by default
            float height = (ARRAY_ELEMENT_HEIGHT * 3f) + (SPACER * 3f);

            SerializedProperty propRules = property.FindPropertyRelative("_rules");
            if (propRules.isExpanded)
            {
                height += ARRAY_ICON_HEIGHT;
                if (propRules.arraySize <= 0) height += ARRAY_ELEMENT_HEIGHT;
                for (int i = 0; i < propRules.arraySize; i++)
                {
                    height += ARRAY_ELEMENT_HEIGHT;
                    if (propRules.GetArrayElementAtIndex(i).isExpanded) height += ARRAY_ELEMENT_HEIGHT;
                }
            }

            SerializedProperty propEvents = property.FindPropertyRelative("_events");
            if (propEvents.isExpanded)
            {
                height += ARRAY_ICON_HEIGHT;
                if (propEvents.arraySize <= 0) height += ARRAY_ELEMENT_HEIGHT;
                for (int i = 0; i < propEvents.arraySize; i++)
                {
                    height += ARRAY_ELEMENT_HEIGHT;
                    if (propEvents.GetArrayElementAtIndex(i).isExpanded) height += ARRAY_ELEMENT_HEIGHT;
                }
            }

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Meter Min/Max/Value
            GUIStyle style = new GUIStyle();
            style.richText = true;
            Rect rectName = new Rect(position.min.x, position.min.y, position.width / RATIO, position.height);
            EditorGUI.LabelField(rectName, "<b><color=#c0c0c0ff>" + property.displayName + "</color></b>", style);

            SerializedProperty propMinimum = property.FindPropertyRelative("_minimum");
            SerializedProperty propMaximum = property.FindPropertyRelative("_maximum");
            SerializedProperty propValue = property.FindPropertyRelative("_value");

            int[] values = new int[]
            {
                propMinimum.intValue,
                propMaximum.intValue,
                propValue.intValue
            };

            GUIContent[] labels = new GUIContent[]
            {
                new GUIContent("Minimum"),
                new GUIContent("Maximum"),
                new GUIContent("Value")
            };

            Rect rectMulti = new Rect(position.min.x + position.width / RATIO, position.min.y, (RATIO - 1) * position.width / RATIO, EditorGUIUtility.singleLineHeight);
            EditorGUI.MultiIntField(rectMulti, labels, values);

            propMinimum.intValue = values[0];
            propMaximum.intValue = values[1];
            propValue.intValue = values[2];

            // Meter Rules
            SerializedProperty propRules = property.FindPropertyRelative("_rules");
            float propRulesHeight = ARRAY_ELEMENT_HEIGHT;
            if (propRules.isExpanded)
            {
                propRulesHeight += ARRAY_ICON_HEIGHT;
                if (propRules.arraySize <= 0)  propRulesHeight += ARRAY_ELEMENT_HEIGHT;
                for (int i = 0; i < propRules.arraySize; i++)
                {
                    propRulesHeight += ARRAY_ELEMENT_HEIGHT;
                    if (propRules.GetArrayElementAtIndex(i).isExpanded) propRulesHeight += ARRAY_ELEMENT_HEIGHT;
                }
            }
            Rect rectRules = new Rect(position.min.x, position.min.y + rectMulti.height + SPACER, position.width, propRulesHeight);
            EditorGUI.PropertyField(rectRules, propRules, true);
            
            // Meter Events
            SerializedProperty propEvents = property.FindPropertyRelative("_events");
            float propEventHeight = ARRAY_ELEMENT_HEIGHT;
            if (propEvents.isExpanded)
            {
                propEventHeight += ARRAY_ICON_HEIGHT;
                if (propEvents.arraySize <= 0) propEventHeight += ARRAY_ELEMENT_HEIGHT;
                for (int i = 0; i < propEvents.arraySize; i++)
                {
                    propEventHeight += ARRAY_ELEMENT_HEIGHT;
                    if (propEvents.GetArrayElementAtIndex(i).isExpanded) propEventHeight += ARRAY_ELEMENT_HEIGHT;
                }
            }
            Rect rectEvents = new Rect(position.min.x, position.min.y + rectMulti.height + rectRules.height + SPACER, position.width, propEventHeight);
            EditorGUI.PropertyField(rectEvents, propEvents, true);
        }
    }
}
#endif