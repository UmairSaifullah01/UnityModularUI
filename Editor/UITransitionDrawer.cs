using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;


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
                object targetObject = GetTargetObjectOfProperty(property);
                if (targetObject is UITransition transition)
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
        
        private object GetTargetObjectOfProperty(SerializedProperty prop)
        {
            if (prop == null) return null;

            string path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            string[] elements = path.Split('.');

            foreach (var element in elements)
            {
                if (element.Contains("["))
                {
                    string elementName = element.Substring(0, element.IndexOf("["));
                    int index = Convert.ToInt32(
                        element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", "")
                    );

                    obj = GetValue_Indexed(obj, elementName, index);
                }
                else
                {
                    obj = GetValue(obj, element);
                }
            }
            return obj;
        }

        private object GetValue(object source, string name)
        {
            if (source == null) return null;
            var type = source.GetType();

            var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (f != null) return f.GetValue(source);

            var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (p != null) return p.GetValue(source, null);

            return null;
        }

        private object GetValue_Indexed(object source, string name, int index)
        {
            var enumerable = GetValue(source, name) as System.Collections.IEnumerable;
            if (enumerable == null) return null;
            var enm = enumerable.GetEnumerator();

            for (int i = 0; i <= index; i++)
            {
                if (!enm.MoveNext()) return null;
            }
            return enm.Current;
        }

    }
} 