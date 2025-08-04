using UnityEditor;
using UnityEngine;
using THEBADDEST.UI;

namespace THEBADDEST.UI
{
    [CustomPropertyDrawer(typeof(UITransition))]
    public class UITransitionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            float y = position.y;
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;
            float width = position.width;

            // Target State
            Rect rect = new Rect(position.x, y, width, lineHeight);
            SerializedProperty targetState = property.FindPropertyRelative("targetState");
            EditorGUI.PropertyField(rect, targetState, new GUIContent("Target State"));
            y += lineHeight + spacing;

            // Bools as table headers
            float colWidth = width / 3f;
            Rect col1 = new Rect(position.x, y, colWidth, lineHeight);
            Rect col2 = new Rect(position.x + colWidth, y, colWidth, lineHeight);
            Rect col3 = new Rect(position.x + 2 * colWidth, y, colWidth, lineHeight);
            EditorGUI.LabelField(col1, "Is Any State", EditorStyles.boldLabel);
            EditorGUI.LabelField(col2, "Clear Any States", EditorStyles.boldLabel);
            EditorGUI.LabelField(col3, "Clear All States", EditorStyles.boldLabel);
            y += lineHeight + spacing;

            // Bools as checkboxes in a row
            SerializedProperty isAnyState = property.FindPropertyRelative("isAnyState");
            SerializedProperty clearAnyStates = property.FindPropertyRelative("clearAnyStates");
            SerializedProperty clearAllStates = property.FindPropertyRelative("clearAllStates");
            col1 = new Rect(position.x, y, colWidth, lineHeight);
            col2 = new Rect(position.x + colWidth, y, colWidth, lineHeight);
            col3 = new Rect(position.x + 2 * colWidth, y, colWidth, lineHeight);
            isAnyState.boolValue = EditorGUI.Toggle(col1, isAnyState.boolValue);
            clearAnyStates.boolValue = EditorGUI.Toggle(col2, clearAnyStates.boolValue);
            clearAllStates.boolValue = EditorGUI.Toggle(col3, clearAllStates.boolValue);
            y += lineHeight + spacing;

            // Run Button
            rect = new Rect(position.x, y, width, lineHeight);
            if (GUI.Button(rect, "Run"))
            {
                object targetObj = fieldInfo.GetValue(property.serializedObject.targetObject);
                if (targetObj is UITransition transition)
                {
                    transition.Run();
                }
            }
            y += lineHeight + spacing;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Target State + header + checkboxes + button, each with spacing
            return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 4;
        }
    }
} 