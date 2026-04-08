#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace VED.Utilities
{
    [CustomPropertyDrawer(typeof(Optional<>))]
    public class OptionalPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty valueProperty = property.FindPropertyRelative("Value");
            return EditorGUI.GetPropertyHeight(valueProperty);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty valueProperty   = property.FindPropertyRelative("Value");
            SerializedProperty enabledProperty = property.FindPropertyRelative("Enabled");

            EditorGUI.BeginProperty(position, label, property);
                position.width -= 24;
                EditorGUI.BeginDisabledGroup(!enabledProperty.boolValue);
                
                if (   valueProperty.propertyType == SerializedPropertyType.String 
                    && valueProperty.stringValue.Length > 20)
                {
                    EditorGUI.LabelField(position, label);
                    GUIStyle style = new(EditorStyles.textArea) { wordWrap = true };
                    float height = EditorGUIUtility.singleLineHeight * 3f;
                    valueProperty.stringValue = EditorGUILayout.TextArea(valueProperty.stringValue, style, GUILayout.Height(height));
                }
                else
                {
                    EditorGUI.PropertyField(position, valueProperty, label, true);
                }
                
                EditorGUI.EndDisabledGroup();

                int indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;
                position.x += position.width + 24;
                position.width = position.height = EditorGUI.GetPropertyHeight(enabledProperty);
                position.x -= position.width;
                EditorGUI.PropertyField(position, enabledProperty, GUIContent.none);
                EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
}
#endif