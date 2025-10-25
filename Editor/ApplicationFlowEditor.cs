using UnityEditor;
using UnityEngine;
using Object = System.Object;

namespace THEBADDEST.UI
{
    [CustomEditor(typeof(ApplicationFlow))]
    public class ApplicationFlowEditor : Editor
    {
        private SerializedProperty entriesProperty;
        private int selectedIndex = 0;
        string[] stateNames;
        Vector2 scrollVector;

        void OnEnable()
        {
            entriesProperty = serializedObject.FindProperty("entries");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawScript(target);

            if (entriesProperty == null)
            {
                serializedObject.ApplyModifiedProperties();
                return;
            }

            // Rebuild stateNames when null or size changed.
            if (entriesProperty.arraySize == 0)
            {
                // show add button only when empty
                EditorGUILayout.BeginHorizontal(GUI.skin.window);
                if (GUILayout.Button("Add"))
                {
                    entriesProperty.arraySize++;
                    // initialize new element
                    var newElem = entriesProperty.GetArrayElementAtIndex(entriesProperty.arraySize - 1);
                    var stateNameProp = newElem.FindPropertyRelative("stateName");
                    var stateObjectProp = newElem.FindPropertyRelative("stateObject");
                    var transitionsProp = newElem.FindPropertyRelative("transitions");

                    stateNameProp.stringValue = $"State {entriesProperty.arraySize}";
                    stateObjectProp.objectReferenceValue = null;
                    if (transitionsProp != null)
                        transitionsProp.arraySize = 0;

                    stateNames = null;
                    selectedIndex = entriesProperty.arraySize - 1;
                }
                EditorGUILayout.EndHorizontal();

                serializedObject.ApplyModifiedProperties();
                return;
            }

            if (stateNames == null || stateNames.Length != entriesProperty.arraySize)
            {
                RebuildStateNames();
            }

            // ensure selectedIndex is valid
            selectedIndex = Mathf.Clamp(selectedIndex, 0, Mathf.Max(0, entriesProperty.arraySize - 1));

            EditorGUILayout.BeginHorizontal(GUI.skin.window,GUILayout.Height(20));
            // Draw popup (clamped)
            selectedIndex = EditorGUILayout.Popup("Selected", selectedIndex, stateNames);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            // Show selected element
            EditorGUILayout.BeginVertical(GUI.skin.window);
            var selectedElement = entriesProperty.GetArrayElementAtIndex(selectedIndex);
            var stateName = selectedElement.FindPropertyRelative("stateName");
            EditorGUILayout.PropertyField(stateName);
            var stateObject = selectedElement.FindPropertyRelative("stateObject");
            EditorGUILayout.PropertyField(stateObject, includeChildren: true);
            var transitions = selectedElement.FindPropertyRelative("transitions");
            EditorGUILayout.PropertyField(transitions, includeChildren: true);
            GUILayout.EndVertical();
            // Buttons
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal(GUI.skin.window,GUILayout.Height(20));
            if (GUILayout.Button("Add"))
            {
                // add new element at end
                entriesProperty.arraySize++;
                int newIndex = entriesProperty.arraySize - 1;

                var newElem = entriesProperty.GetArrayElementAtIndex(newIndex);
                var stateNameProp = newElem.FindPropertyRelative("stateName");
                var stateObjectProp = newElem.FindPropertyRelative("stateObject");
                var transitionsProp = newElem.FindPropertyRelative("transitions");

                stateNameProp.stringValue = $"New State";
                if (stateObjectProp != null) stateObjectProp.objectReferenceValue = null;
                if (transitionsProp != null) transitionsProp.arraySize = 0;

                // force rebuild & select new
                stateNames = null;
                selectedIndex = newIndex;
            }

            if (GUILayout.Button("Remove"))
            {
                // safe removal
                if (entriesProperty.arraySize > 0)
                {
                    entriesProperty.DeleteArrayElementAtIndex(selectedIndex);
                    // If element was a reference type and still exists, call Delete again:
                    if (selectedIndex < entriesProperty.arraySize && entriesProperty.GetArrayElementAtIndex(selectedIndex).propertyType == SerializedPropertyType.ObjectReference)
                    {
                        // try another delete to clear reference (defensive; may not be needed for pure serializable classes)
                        entriesProperty.DeleteArrayElementAtIndex(selectedIndex);
                    }

                    // clamp selectedIndex
                    if (entriesProperty.arraySize == 0)
                        selectedIndex = 0;
                    else
                        selectedIndex = Mathf.Clamp(selectedIndex, 0, entriesProperty.arraySize - 1);

                    stateNames = null;
                }
            }
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

        private void RebuildStateNames()
        {
            if (entriesProperty == null)
            {
                stateNames = new string[0];
                return;
            }

            stateNames = new string[entriesProperty.arraySize];
            for (int i = 0; i < entriesProperty.arraySize; i++)
            {
                var element = entriesProperty.GetArrayElementAtIndex(i);
                var stateNameProp = element.FindPropertyRelative("stateName");
                stateNames[i] = string.IsNullOrEmpty(stateNameProp.stringValue) ? $"State {i}" : stateNameProp.stringValue;
            }
        }

        public static void DrawScript(Object target)
        {
            EditorGUI.BeginDisabledGroup(true);
            var monoScript = (target as MonoBehaviour) != null ? MonoScript.FromMonoBehaviour((MonoBehaviour)target) : MonoScript.FromScriptableObject((ScriptableObject)target);
            EditorGUILayout.ObjectField("Script", monoScript, target.GetType(), false);
            EditorGUI.EndDisabledGroup();
        }
    }
}
