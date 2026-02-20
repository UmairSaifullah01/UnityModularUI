using UnityEditor;
using UnityEngine;
using Object = System.Object;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace THEBADDEST.UI
{
    public enum TransitionViewMode
    {
        ListView,
        GraphView
    }

    [CustomEditor(typeof(ApplicationFlow))]
    public class ApplicationFlowEditor : UnityEditor.Editor
    {
        private SerializedProperty entriesProperty;
        private SerializedProperty bootStateNameProperty;
        private int selectedIndex = 0;
        private int selectedStateToAddIndex = 0;
        string[] stateNames;
        Vector2 scrollVector;
        Vector2 graphScrollPosition;
        TransitionViewMode viewMode = TransitionViewMode.ListView;
        
        // Performance caching
        private static List<string> cachedAllStateTypes = null;
        private string[] cachedAvailableStateNames = null;
        private int cachedEntriesCount = -1;
        private GUIStyle toggleButtonStyle = null;
        
        // Graph view properties
        private Dictionary<string, Rect> nodePositions = new Dictionary<string, Rect>();
        private HashSet<string> drawnConnections = new HashSet<string>();
        private Dictionary<string, bool> expandedStates = new Dictionary<string, bool>();
        private float nodeWidth = 180f;
        private float nodeHeaderHeight = 35f;
        private float nodeContentHeight = 30f;
        private float nodeTotalHeight = 65f;
        private float nodeSpacing = 220f;
        private float graphZoom = 1f;
        private Vector2 graphPanOffset = Vector2.zero;
        private string selectedNodeForTransition = null;
        private string draggedNode = null;
        private Vector2 dragOffset = Vector2.zero;

        void OnEnable()
        {
            entriesProperty = serializedObject.FindProperty("entries");
            bootStateNameProperty = serializedObject.FindProperty("bootStateName");
            graphScrollPosition = Vector2.zero;
            drawnConnections.Clear();
            // Invalidate cache when editor is enabled
            cachedAvailableStateNames = null;
            cachedEntriesCount = -1;
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

            // View Mode Toggle at the top
            EditorGUILayout.Space(5);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("View Mode", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            
            // Enhanced toggle buttons with better styling (cached)
            if (toggleButtonStyle == null)
            {
                toggleButtonStyle = new GUIStyle(GUI.skin.button);
                toggleButtonStyle.padding = new RectOffset(10, 10, 8, 8);
                toggleButtonStyle.fontSize = 11;
            }
            
            Color originalBgColor = GUI.backgroundColor;
            Color originalContentColor = GUI.contentColor;
            
            // List View Button
            if (viewMode == TransitionViewMode.ListView)
            {
                GUI.backgroundColor = new Color(0.2f, 0.6f, 0.9f);
                GUI.contentColor = Color.white;
            }
            else
            {
                GUI.backgroundColor = new Color(0.3f, 0.3f, 0.3f);
                GUI.contentColor = new Color(0.8f, 0.8f, 0.8f);
            }
            
            if (GUILayout.Button("📋 List View", toggleButtonStyle, GUILayout.Height(30)))
            {
                viewMode = TransitionViewMode.ListView;
            }
            
            // Graph View Button
            if (viewMode == TransitionViewMode.GraphView)
            {
                GUI.backgroundColor = new Color(0.2f, 0.6f, 0.9f);
                GUI.contentColor = Color.white;
            }
            else
            {
                GUI.backgroundColor = new Color(0.3f, 0.3f, 0.3f);
                GUI.contentColor = new Color(0.8f, 0.8f, 0.8f);
            }
            
            if (GUILayout.Button("🔗 Graph View", toggleButtonStyle, GUILayout.Height(30)))
            {
                viewMode = TransitionViewMode.GraphView;
            }
            
            GUI.backgroundColor = originalBgColor;
            GUI.contentColor = originalContentColor;
            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);

            // Rebuild stateNames when null or size changed.
            if (entriesProperty.arraySize == 0)
            {
                // show add button only when empty
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                 if (GUILayout.Button("Generate UIStateNames", GUILayout.Height(30)))
                {
                    GenerateUIStateNames();
                }
                EditorGUILayout.HelpBox("No states defined. Select a state from the dropdown and click the button to add it.", MessageType.Info);
                
                string[] remainingStateNames = GetAvailableStateNames();
                
                if (remainingStateNames.Length > 0)
                {
                    EditorGUILayout.BeginHorizontal();
                    
                    // Ensure selected index is valid
                    if (selectedStateToAddIndex >= remainingStateNames.Length)
                        selectedStateToAddIndex = 0;
                    
                    // Dropdown with explicit height to match Add button (30px)
                    Rect popupRect = EditorGUILayout.GetControlRect(false, 30);
                    selectedStateToAddIndex = EditorGUI.Popup(popupRect, selectedStateToAddIndex, remainingStateNames);
                    
                    // Prominent add button
                    GUI.backgroundColor = new Color(0.2f, 0.6f, 0.9f);
                    if (GUILayout.Button("+ Add", GUILayout.Height(30), GUILayout.Width(200)))
                    {
                        entriesProperty.arraySize++;
                        // initialize new element
                        var newElem = entriesProperty.GetArrayElementAtIndex(entriesProperty.arraySize - 1);
                        var stateNameProp = newElem.FindPropertyRelative("stateName");
                        var stateObjectProp = newElem.FindPropertyRelative("stateObject");
                        var transitionsProp = newElem.FindPropertyRelative("transitions");

                        stateNameProp.stringValue = remainingStateNames[selectedStateToAddIndex];
                        stateObjectProp.objectReferenceValue = null;
                        if (transitionsProp != null)
                            transitionsProp.arraySize = 0;

                        stateNames = null;
                        selectedIndex = entriesProperty.arraySize - 1;
                        selectedStateToAddIndex = 0; // Reset to first item after adding
                        cachedAvailableStateNames = null; // Invalidate cache
                    }
                    GUI.backgroundColor = Color.white;
                    
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.HelpBox("All available states have been added.", MessageType.Info);
                }
                
                EditorGUILayout.EndVertical();

                serializedObject.ApplyModifiedProperties();
                return;
            }

            if (stateNames == null || stateNames.Length != entriesProperty.arraySize)
            {
                RebuildStateNames();
            }

            // ensure selectedIndex is valid
            selectedIndex = Mathf.Clamp(selectedIndex, 0, Mathf.Max(0, entriesProperty.arraySize - 1));

            // Two separate containers based on view mode
            if (viewMode == TransitionViewMode.ListView)
            {
                // Boot State Configuration (only in List View)
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Boot Configuration", EditorStyles.boldLabel);
                if (bootStateNameProperty != null && stateNames != null && stateNames.Length > 0)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Boot State Name:", EditorStyles.boldLabel, GUILayout.Width(120));
                    
                    // Find current boot state index
                    string currentBootState = bootStateNameProperty.stringValue;
                    int bootStateIndex = 0;
                    if (!string.IsNullOrEmpty(currentBootState))
                    {
                        for (int i = 0; i < stateNames.Length; i++)
                        {
                            if (stateNames[i] == currentBootState)
                            {
                                bootStateIndex = i;
                                break;
                            }
                        }
                    }
                    
                    // Show dropdown
                    int newBootStateIndex = EditorGUILayout.Popup(bootStateIndex, stateNames, GUILayout.ExpandWidth(true));
                    if (newBootStateIndex != bootStateIndex)
                    {
                        bootStateNameProperty.stringValue = stateNames[newBootStateIndex];
                    }
                    
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.HelpBox("The initial state where transitions start", MessageType.None);
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(5);
                
                // Container 1: List View - Show ALL states and ALL their transitions
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("📋 All States & Transitions", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox($"Showing all {entriesProperty.arraySize} states and their transitions in list format.", MessageType.Info);
                EditorGUILayout.Space(3);
                
                DrawAllStatesListView();
                
                EditorGUILayout.EndVertical();
            }
            else
            {
                // Container 2: Graph View - Show only graph (no boot config, no state config)
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("🔗 State Flow Graph", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox($"Visual graph showing all {entriesProperty.arraySize} states and their transitions.", MessageType.Info);
                EditorGUILayout.Space(3);
                
                DrawAllStatesGraphView();
                
                EditorGUILayout.EndVertical();
            }
            // State Management Buttons
            EditorGUILayout.Space(5);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("State Management", EditorStyles.boldLabel);
            
            // Add State with Dropdown - Improved UI/UX
            string[] availableStateNames = GetAvailableStateNames();
            
            if (availableStateNames.Length > 0)
            {
                EditorGUILayout.BeginHorizontal();
                
                // Ensure selected index is valid
                if (selectedStateToAddIndex >= availableStateNames.Length)
                    selectedStateToAddIndex = 0;
                
                // Dropdown with explicit height to match Add button (30px)
                Rect popupRect = EditorGUILayout.GetControlRect(false, 30);
                selectedStateToAddIndex = EditorGUI.Popup(popupRect, selectedStateToAddIndex, availableStateNames);
                
                // Prominent add button
                GUI.backgroundColor = new Color(0.2f, 0.6f, 0.9f);
                if (GUILayout.Button("+ Add", GUILayout.Height(30), GUILayout.Width(200)))
                {
                    // add new element at end
                    entriesProperty.arraySize++;
                    int newIndex = entriesProperty.arraySize - 1;

                    var newElem = entriesProperty.GetArrayElementAtIndex(newIndex);
                    var stateNameProp = newElem.FindPropertyRelative("stateName");
                    var stateObjectProp = newElem.FindPropertyRelative("stateObject");
                    var transitionsProp = newElem.FindPropertyRelative("transitions");

                    stateNameProp.stringValue = availableStateNames[selectedStateToAddIndex];
                    if (stateObjectProp != null) stateObjectProp.objectReferenceValue = null;
                    if (transitionsProp != null) transitionsProp.arraySize = 0;

                    // force rebuild & select new
                    stateNames = null;
                    selectedIndex = newIndex;
                    selectedStateToAddIndex = 0; // Reset to first item after adding
                    cachedAvailableStateNames = null; // Invalidate cache
                }
                GUI.backgroundColor = Color.white;
                
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(2);
            }
            else
            {
                EditorGUILayout.HelpBox("All available states have been added.", MessageType.Info);
            }
            EditorGUILayout.HelpBox($"Total States: {entriesProperty.arraySize}", MessageType.None);
            if (GUILayout.Button("Generate UIStateNames", GUILayout.Height(30)))
            {
                GenerateUIStateNames();
            }
            EditorGUILayout.EndVertical();

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

        private string[] GetAvailableStateNames()
        {
            // Check if we need to recalculate
            int currentEntriesCount = entriesProperty != null ? entriesProperty.arraySize : 0;
            bool needsRecalculation = cachedAvailableStateNames == null || cachedEntriesCount != currentEntriesCount;
            
            if (!needsRecalculation)
            {
                return cachedAvailableStateNames;
            }
            
            // Cache all state types (static - only calculated once, shared across all instances)
            if (cachedAllStateTypes == null)
            {
                cachedAllStateTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => !a.IsDynamic)
                    .SelectMany(a => a.GetTypes())
                    .Where(t => t.IsClass && !t.IsAbstract && typeof(THEBADDEST.IState).IsAssignableFrom(t))
                    .Select(t => t.Name)
                    .OrderBy(name => name)
                    .ToList();
            }

            // Get currently used state names
            HashSet<string> usedStateNames = new HashSet<string>();
            if (entriesProperty != null)
            {
                for (int i = 0; i < entriesProperty.arraySize; i++)
                {
                    var element = entriesProperty.GetArrayElementAtIndex(i);
                    var stateNameProp = element.FindPropertyRelative("stateName");
                    if (!string.IsNullOrEmpty(stateNameProp.stringValue))
                    {
                        usedStateNames.Add(stateNameProp.stringValue);
                    }
                }
            }

            // Filter out already used state names and cache result
            cachedAvailableStateNames = cachedAllStateTypes.Where(name => !usedStateNames.Contains(name)).ToArray();
            cachedEntriesCount = currentEntriesCount;
            
            return cachedAvailableStateNames;
        }

        private void DrawAllStatesListView()
        {
            if (entriesProperty == null || entriesProperty.arraySize == 0)
            {
                EditorGUILayout.HelpBox("No states available", MessageType.Info);
                return;
            }

            scrollVector = EditorGUILayout.BeginScrollView(scrollVector, GUILayout.Height(400));
            
            for (int stateIdx = 0; stateIdx < entriesProperty.arraySize; stateIdx++)
            {
                var stateElement = entriesProperty.GetArrayElementAtIndex(stateIdx);
                var stateName = stateElement.FindPropertyRelative("stateName").stringValue;
                var transitionsProp = stateElement.FindPropertyRelative("transitions");
                
                // Initialize expanded state if not exists
                if (!expandedStates.ContainsKey(stateName))
                {
                    expandedStates[stateName] = false;
                }
                
                int transitionCount = transitionsProp != null && transitionsProp.isArray ? transitionsProp.arraySize : 0;
                
                // Collapsible state header
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();
                
                // Expand/Collapse button
                string foldoutLabel = expandedStates[stateName] ? "▼" : "▶";
                if (GUILayout.Button(foldoutLabel, GUILayout.Width(20), GUILayout.Height(20)))
                {
                    expandedStates[stateName] = !expandedStates[stateName];
                }
                
                EditorGUILayout.LabelField(stateName, EditorStyles.boldLabel, GUILayout.ExpandWidth(true));
                EditorGUILayout.LabelField($"{transitionCount} transition(s)", EditorStyles.miniLabel, GUILayout.Width(120));
                
                // Delete button (cross)
                GUI.backgroundColor = new Color(0.9f, 0.3f, 0.3f);
                bool deleteClicked = GUILayout.Button("×", GUILayout.Width(25), GUILayout.Height(20));
                GUI.backgroundColor = Color.white;
                
                if (deleteClicked)
                {
                    if (EditorUtility.DisplayDialog("Remove State", 
                        $"Are you sure you want to remove state '{stateName}'?", 
                        "Remove", "Cancel"))
                    {
                        entriesProperty.DeleteArrayElementAtIndex(stateIdx);
                        // If element was a reference type and still exists, call Delete again:
                        if (stateIdx < entriesProperty.arraySize && entriesProperty.GetArrayElementAtIndex(stateIdx).propertyType == SerializedPropertyType.ObjectReference)
                        {
                            entriesProperty.DeleteArrayElementAtIndex(stateIdx);
                        }
                        
                        // Update selectedIndex
                        if (entriesProperty.arraySize == 0)
                            selectedIndex = 0;
                        else
                            selectedIndex = Mathf.Clamp(selectedIndex, 0, entriesProperty.arraySize - 1);
                        
                        stateNames = null;
                        cachedAvailableStateNames = null; // Invalidate cache
                        serializedObject.ApplyModifiedProperties();
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndScrollView();
                        return;
                    }
                }
                EditorGUILayout.EndHorizontal();
                
                // Show transitions only if expanded
                if (expandedStates[stateName])
                {
                    EditorGUILayout.Space(3);
                    
                    // State GameObject field (shown first)
                    var stateObject = stateElement.FindPropertyRelative("stateObject");
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("State GameObject", GUILayout.Width(120));
                    EditorGUILayout.PropertyField(stateObject, GUIContent.none, includeChildren: true, GUILayout.ExpandWidth(true));
                    EditorGUILayout.EndHorizontal();
                    
                    EditorGUILayout.Space(5);
                    EditorGUILayout.LabelField("Transitions", EditorStyles.boldLabel);
                    EditorGUILayout.Space(3);
                    
                    if (transitionsProp == null || !transitionsProp.isArray || transitionsProp.arraySize == 0)
                    {
                        EditorGUILayout.HelpBox("No transitions defined for this state.", MessageType.None);
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("+ Add Transition", GUILayout.Height(30)))
                        {
                            transitionsProp.arraySize++;
                            serializedObject.ApplyModifiedProperties();
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    else
                    {
                        for (int i = 0; i < transitionsProp.arraySize; i++)
                        {
                            DrawTransitionCard(transitionsProp.GetArrayElementAtIndex(i), stateName, i, transitionsProp);
                        }
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("+ Add Transition", GUILayout.Height(30)))
                        {
                            transitionsProp.arraySize++;
                            serializedObject.ApplyModifiedProperties();
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(3);
            }
            
            EditorGUILayout.EndScrollView();
        }

        private void DrawTransitionCard(SerializedProperty transition, string fromStateName, int index, SerializedProperty transitionsProp)
        {
            var targetState = transition.FindPropertyRelative("targetState");
            var isAnyState = transition.FindPropertyRelative("isAnyState");
            var clearAnyStates = transition.FindPropertyRelative("clearAnyStates");
            var clearAllStates = transition.FindPropertyRelative("clearAllStates");
            
            // Determine header color based on transition type
            Color headerColor = new Color(0.3f, 0.5f, 0.7f);
            string typeLabel = "Standard";
            
            if (clearAllStates.boolValue)
            {
                headerColor = new Color(0.7f, 0.3f, 0.3f);
                typeLabel = "Clear All";
            }
            else if (isAnyState.boolValue)
            {
                headerColor = new Color(0.7f, 0.7f, 0.3f);
                typeLabel = "Any State";
            }
            
            // Card container
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            // Header section
            Rect headerRect = EditorGUILayout.GetControlRect(false, 35);
            GUI.BeginGroup(headerRect);
            EditorGUI.DrawRect(new Rect(0, 0, headerRect.width, headerRect.height), headerColor);
            
            // Arrow indicator
            Color originalColor = GUI.color;
            GUI.color = Color.white;
            GUI.Label(new Rect(5, 8, 20, 20), "→", EditorStyles.boldLabel);
            GUI.color = originalColor;
            
            // From -> To state
            GUIStyle headerLabelStyle = new GUIStyle(EditorStyles.boldLabel);
            headerLabelStyle.normal.textColor = Color.white;
            headerLabelStyle.fontSize = 12;
            string transitionLabel = $"{fromStateName} → {targetState.stringValue}";
            GUI.Label(new Rect(25, 8, headerRect.width - 180, 20), transitionLabel, headerLabelStyle);
            
            // Type badge
            Rect badgeRect = new Rect(headerRect.width - 150, 7, 70, 21);
            EditorGUI.DrawRect(badgeRect, new Color(0, 0, 0, 0.3f));
            GUIStyle badgeStyle = new GUIStyle(EditorStyles.miniLabel);
            badgeStyle.normal.textColor = Color.white;
            badgeStyle.alignment = TextAnchor.MiddleCenter;
            GUI.Label(badgeRect, typeLabel, badgeStyle);
            
            // Action buttons
            if (GUI.Button(new Rect(headerRect.width - 75, 5, 35, 25), "▶"))
            {
                TestTransition(transition);
            }
            if (GUI.Button(new Rect(headerRect.width - 35, 5, 30, 25), "×"))
            {
                transitionsProp.DeleteArrayElementAtIndex(index);
                serializedObject.ApplyModifiedProperties();
                GUI.EndGroup();
                EditorGUILayout.EndVertical();
                return;
            }
            
            GUI.EndGroup();
            
            EditorGUILayout.Space(5);
            
            // Content section
            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 90;
            EditorGUILayout.PropertyField(targetState, new GUIContent("Target State:"), GUILayout.Width(250));
            EditorGUIUtility.labelWidth = 0;
            
            EditorGUILayout.Space(10);
            
            // Flags section: label + gap + toggle per flag (no text inside toggle; controlled width and gap)
            const float flagLabelWidth = 80f;
            const float flagGap = 0f;
            const float flagToggleWidth = 18f;
            const float flagGroupSpacing = 12f;
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Flags", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Any State", GUILayout.MinWidth(flagLabelWidth));
            EditorGUILayout.Space(flagGap);
            isAnyState.boolValue = EditorGUILayout.Toggle(GUIContent.none, isAnyState.boolValue, GUILayout.Width(flagToggleWidth));
            EditorGUILayout.Space(flagGroupSpacing);
            EditorGUILayout.LabelField("Clear Any States", GUILayout.MinWidth(flagLabelWidth));
            EditorGUILayout.Space(flagGap);
            clearAnyStates.boolValue = EditorGUILayout.Toggle(GUIContent.none, clearAnyStates.boolValue, GUILayout.Width(flagToggleWidth));
            EditorGUILayout.Space(flagGroupSpacing);
            EditorGUILayout.LabelField("Clear All States", GUILayout.MinWidth(flagLabelWidth));
            EditorGUILayout.Space(flagGap);
            clearAllStates.boolValue = EditorGUILayout.Toggle(GUIContent.none, clearAllStates.boolValue, GUILayout.Width(flagToggleWidth));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(13);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(3);
        }

        private void DrawTransitionsListView(SerializedProperty transitionsProp)
        {
            if (transitionsProp == null || !transitionsProp.isArray)
            {
                EditorGUILayout.HelpBox("No transitions available", MessageType.Info);
                return;
            }

            if (transitionsProp.arraySize == 0)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox("No transitions. Click 'Add Transition' to create one.", MessageType.Info);
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("+ Add Transition", GUILayout.Height(30)))
                {
                    transitionsProp.arraySize++;
                    serializedObject.ApplyModifiedProperties();
                }
                EditorGUILayout.EndHorizontal();
                return;
            }

            scrollVector = EditorGUILayout.BeginScrollView(scrollVector, GUILayout.Height(300));
            
            for (int i = 0; i < transitionsProp.arraySize; i++)
            {
                var transition = transitionsProp.GetArrayElementAtIndex(i);
                var targetState = transition.FindPropertyRelative("targetState");
                var isAnyState = transition.FindPropertyRelative("isAnyState");
                var clearAnyStates = transition.FindPropertyRelative("clearAnyStates");
                var clearAllStates = transition.FindPropertyRelative("clearAllStates");
                
                // Determine header color based on transition type
                Color headerColor = new Color(0.3f, 0.5f, 0.7f);
                Color indicatorColor = Color.green;
                string typeLabel = "Standard";
                
                if (clearAllStates.boolValue)
                {
                    headerColor = new Color(0.7f, 0.3f, 0.3f);
                    indicatorColor = Color.red;
                    typeLabel = "Clear All";
                }
                else if (isAnyState.boolValue)
                {
                    headerColor = new Color(0.7f, 0.7f, 0.3f);
                    indicatorColor = Color.yellow;
                    typeLabel = "Any State";
                }
                
                // Card container
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                // Header section with colored background using GUI.BeginGroup
                Rect headerRect = EditorGUILayout.GetControlRect(false, 35);
                GUI.BeginGroup(headerRect);
                EditorGUI.DrawRect(new Rect(0, 0, headerRect.width, headerRect.height), headerColor);
                
                // Arrow indicator
                Color originalColor = GUI.color;
                GUI.color = Color.white;
                GUI.Label(new Rect(5, 8, 20, 20), "→", EditorStyles.boldLabel);
                GUI.color = originalColor;
                
                // State name
                GUIStyle headerLabelStyle = new GUIStyle(EditorStyles.boldLabel);
                headerLabelStyle.normal.textColor = Color.white;
                headerLabelStyle.fontSize = 13;
                GUI.Label(new Rect(25, 8, headerRect.width - 180, 20), targetState.stringValue, headerLabelStyle);
                
                // Type badge
                Rect badgeRect = new Rect(headerRect.width - 150, 7, 70, 21);
                EditorGUI.DrawRect(badgeRect, new Color(0, 0, 0, 0.3f));
                GUIStyle badgeStyle = new GUIStyle(EditorStyles.miniLabel);
                badgeStyle.normal.textColor = Color.white;
                badgeStyle.alignment = TextAnchor.MiddleCenter;
                GUI.Label(badgeRect, typeLabel, badgeStyle);
                
                // Action buttons
                if (GUI.Button(new Rect(headerRect.width - 75, 5, 35, 25), "▶"))
                {
                    TestTransition(transition);
                }
                if (GUI.Button(new Rect(headerRect.width - 35, 5, 30, 25), "×"))
                {
                    transitionsProp.DeleteArrayElementAtIndex(i);
                    serializedObject.ApplyModifiedProperties();
                    GUI.EndGroup();
                    EditorGUILayout.EndVertical();
                    break;
                }
                
                GUI.EndGroup();
                
                EditorGUILayout.Space(5);
                
                // Content section
                EditorGUILayout.BeginHorizontal();
                EditorGUIUtility.labelWidth = 90;
                EditorGUILayout.PropertyField(targetState, new GUIContent("Target State:"), GUILayout.Width(250));
                EditorGUIUtility.labelWidth = 0;
                
                EditorGUILayout.Space(10);
                
                // Flags section: label + gap + toggle per flag (no text inside toggle; controlled width and gap)
                const float flagLabelWidth = 120f;
                const float flagGap = 8f;
                const float flagToggleWidth = 18f;
                const float flagGroupSpacing = 12f;
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Flags", EditorStyles.boldLabel);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Any State", GUILayout.Width(flagLabelWidth));
                EditorGUILayout.Space(flagGap);
                isAnyState.boolValue = EditorGUILayout.Toggle(GUIContent.none, isAnyState.boolValue, GUILayout.Width(flagToggleWidth));
                EditorGUILayout.Space(flagGroupSpacing);
                EditorGUILayout.LabelField("Clear Any States", GUILayout.Width(flagLabelWidth));
                EditorGUILayout.Space(flagGap);
                clearAnyStates.boolValue = EditorGUILayout.Toggle(GUIContent.none, clearAnyStates.boolValue, GUILayout.Width(flagToggleWidth));
                EditorGUILayout.Space(flagGroupSpacing);
                EditorGUILayout.LabelField("Clear All States", GUILayout.Width(flagLabelWidth));
                EditorGUILayout.Space(flagGap);
                clearAllStates.boolValue = EditorGUILayout.Toggle(GUIContent.none, clearAllStates.boolValue, GUILayout.Width(flagToggleWidth));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(5);
            }
            
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.Space(5);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Add Transition", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+ Add Standard Transition", GUILayout.Height(35)))
            {
                transitionsProp.arraySize++;
                serializedObject.ApplyModifiedProperties();
            }
            
            // Preset buttons
            if (GUILayout.Button("+ Add (Clear All)", GUILayout.Height(35), GUILayout.Width(150)))
            {
                int newIndex = transitionsProp.arraySize;
                transitionsProp.arraySize++;
                var newTransition = transitionsProp.GetArrayElementAtIndex(newIndex);
                newTransition.FindPropertyRelative("clearAllStates").boolValue = true;
                serializedObject.ApplyModifiedProperties();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.HelpBox($"Total Transitions: {transitionsProp.arraySize}", MessageType.None);
            EditorGUILayout.EndVertical();
        }

        private void DrawAllStatesGraphView()
        {
            if (entriesProperty == null || entriesProperty.arraySize == 0)
            {
                EditorGUILayout.HelpBox("No states available for graph view", MessageType.Info);
                return;
            }

            // Calculate graph layout for all states
            string firstStateName = entriesProperty.arraySize > 0 
                ? entriesProperty.GetArrayElementAtIndex(0).FindPropertyRelative("stateName").stringValue 
                : "";
            CalculateGraphLayout(firstStateName);
            
            
            // Graph area - larger for better visibility
            Rect graphArea = EditorGUILayout.GetControlRect(false, 500);
            graphArea = EditorGUI.IndentedRect(graphArea);
            
            // Draw graph background with grid pattern
            EditorGUI.DrawRect(graphArea, new Color(0.22f, 0.22f, 0.22f));
            
            // Draw grid pattern
            DrawGrid(graphArea, 20f, new Color(0.15f, 0.15f, 0.15f));
            
            // Begin GUI group for graph
            GUI.BeginGroup(graphArea);
            
            // Draw connections first (before nodes) using Handles
            Handles.BeginGUI();
            // Apply pan to Handles matrix
            Matrix4x4 oldHandlesMatrix = Handles.matrix;
            Handles.matrix = Matrix4x4.TRS(graphPanOffset, Quaternion.identity, Vector3.one);
            
            // Draw all connections from all states
            DrawAllGraphConnectionsSimple(graphArea);
            
            Handles.matrix = oldHandlesMatrix;
            Handles.EndGUI();
            
            // Apply pan to GUI for nodes
            Matrix4x4 oldGUIMatrix = GUI.matrix;
            GUI.matrix = Matrix4x4.TRS(graphPanOffset, Quaternion.identity, Vector3.one) * GUI.matrix;
            
            // Draw all nodes
            DrawGraphNodes("", graphArea);
            
            GUI.matrix = oldGUIMatrix;
            
            GUI.EndGroup();
            
            // Handle mouse input for panning/dragging
            HandleGraphInput(graphArea);
            
            EditorGUILayout.Space(5);
            
            // Legend with better styling
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Legend", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(10);
            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(GUILayout.Width(25), GUILayout.Height(18)), Color.green);
            EditorGUILayout.LabelField("Normal Transition", GUILayout.Width(120));
            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(GUILayout.Width(25), GUILayout.Height(18)), Color.yellow);
            EditorGUILayout.LabelField("Any State", GUILayout.Width(100));
            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(GUILayout.Width(25), GUILayout.Height(18)), Color.red);
            EditorGUILayout.LabelField("Clear All", GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void CalculateGraphLayout(string currentStateName)
        {
            if (entriesProperty == null) return;
            
            // Try to load positions from metadata
            var flow = target as ApplicationFlow;
            if (flow != null && flow.nodeMetaData != null && flow.nodeMetaData.Count > 0)
            {
                bool anyMissing = false;
                foreach(var meta in flow.nodeMetaData)
                {
                    if (!string.IsNullOrEmpty(meta.stateName))
                    {
                        nodePositions[meta.stateName] = new Rect(meta.position.x, meta.position.y, 224, 74);
                    }
                }
                
                // Check if we have positions for all current states
                for (int i = 0; i < entriesProperty.arraySize; i++)
                {
                    var element = entriesProperty.GetArrayElementAtIndex(i);
                    var stateName = element.FindPropertyRelative("stateName").stringValue;
                    if (!nodePositions.ContainsKey(stateName))
                    {
                        anyMissing = true; 
                        break;
                    }
                }
                
                if (!anyMissing) return;
            }
            
            // Initialize positions if needed (first run or missing nodes)
            AutoLayoutGraph();
        }

        private void AutoLayoutGraph()
        {
            nodePositions.Clear();
            
            if (entriesProperty == null) return;
            
            // Get boot state name
            string bootStateName = bootStateNameProperty != null ? bootStateNameProperty.stringValue : "";
            
            // Simple grid layout - boot state first at top
            int cols = Mathf.CeilToInt(Mathf.Sqrt(entriesProperty.arraySize));
            int row = 0;
            int col = 0;
            
            float startX = 100f;
            float startY = 100f;
            
            // First, place boot state at top center if it exists
            if (!string.IsNullOrEmpty(bootStateName))
            {
                for (int i = 0; i < entriesProperty.arraySize; i++)
                {
                    var element = entriesProperty.GetArrayElementAtIndex(i);
                    var stateName = element.FindPropertyRelative("stateName").stringValue;
                    if (stateName == bootStateName)
                    {
                        float x = startX + (cols / 2f) * nodeSpacing - 112f; // Center horizontally
                        float y = startY;
                        nodePositions[stateName] = new Rect(x, y, 224, 74);
                        break;
                    }
                }
                row = 1; // Start other states on second row
            }
            
            // Place remaining states
            for (int i = 0; i < entriesProperty.arraySize; i++)
            {
                var element = entriesProperty.GetArrayElementAtIndex(i);
                var stateName = element.FindPropertyRelative("stateName").stringValue;
                
                // Skip boot state as it's already placed
                if (stateName == bootStateName) continue;
                
                // If it was already placed (duplicate check), skip
                if (nodePositions.ContainsKey(stateName)) continue;
                
                float x = startX + col * nodeSpacing;
                float y = startY + row * nodeSpacing;
                
                nodePositions[stateName] = new Rect(x, y, 224, 74);
                
                col++;
                if (col >= cols)
                {
                    col = 0;
                    row++;
                }
            }
            
            SaveNodePositions();
        }

        private void SaveNodePositions()
        {
            var flow = target as ApplicationFlow;
            if (flow == null) return;
            
            if (flow.nodeMetaData == null) flow.nodeMetaData = new List<ApplicationFlow.NodeData>();
            flow.nodeMetaData.Clear();
            
            foreach (var kvp in nodePositions)
            {
                flow.nodeMetaData.Add(new ApplicationFlow.NodeData 
                { 
                    stateName = kvp.Key, 
                    position = kvp.Value.position 
                });
            }
            
            EditorUtility.SetDirty(flow);
        }

        private void DrawGraphNodes(string currentStateName, Rect graphArea)
        {
            if (entriesProperty == null) return;
            
            // Get boot state name
            string bootStateName = bootStateNameProperty != null ? bootStateNameProperty.stringValue : "";
            
            for (int i = 0; i < entriesProperty.arraySize; i++)
            {
                var element = entriesProperty.GetArrayElementAtIndex(i);
                var stateName = element.FindPropertyRelative("stateName").stringValue;
                
                if (!nodePositions.ContainsKey(stateName))
                {
                    nodePositions[stateName] = new Rect(50 + i * 200, 50, 224, 74);
                }
                
                Rect nodeRect = nodePositions[stateName];
                
                // Check if this is boot state
                bool isBootState = !string.IsNullOrEmpty(bootStateName) && stateName == bootStateName;
                bool isCurrentState = !string.IsNullOrEmpty(currentStateName) && stateName == currentStateName;
                
                // Custom Node Styling
                // Background
                Color nodeColor = new Color(0.2f, 0.2f, 0.2f, 0.9f);
                EditorGUI.DrawRect(nodeRect, nodeColor);
                
                // Header Color based on state type
                Color headerColor = new Color(0.3f, 0.5f, 0.7f); // Normal: Blue
                if (isBootState) headerColor = new Color(0.2f, 0.6f, 0.3f); // Boot: Green
                if (isCurrentState) headerColor = new Color(0.8f, 0.5f, 0.2f); // Current: Orange (Runtime)
                
                // Header Rect
                Rect headerRect = new Rect(nodeRect.x, nodeRect.y, nodeRect.width, 25);
                EditorGUI.DrawRect(headerRect, headerColor);
                
                // Outline/Border
                DrawRectOutline(nodeRect, new Color(0.1f, 0.1f, 0.1f), 1f);
                if (isCurrentState) DrawRectOutline(nodeRect, Color.yellow, 2f);
                
                GUILayout.BeginArea(nodeRect);
                
                // State Name
                GUIStyle labelStyle = new GUIStyle(GUI.skin.label) 
                { 
                    alignment = TextAnchor.MiddleCenter, 
                    fontSize = 12,
                    fontStyle = FontStyle.Bold,
                    normal = { textColor = Color.white }
                };
                
                // State name label in header
                string displayName = isBootState ? $"★ {stateName}" : stateName;
                GUI.Label(new Rect(0, 4, nodeRect.width, 20), displayName, labelStyle);
                
                EditorGUILayout.BeginVertical();
                GUILayout.Space(28); // Skip header
                
                // Transition count
                var transitions = element.FindPropertyRelative("transitions");
                int transitionCount = transitions != null && transitions.isArray ? transitions.arraySize : 0;
                
                GUIStyle countStyle = new GUIStyle(GUI.skin.label) 
                { 
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 10,
                    normal = { textColor = new Color(0.8f, 0.8f, 0.8f) }
                };
                
                if (transitionCount > 0)
                {
                    GUILayout.Label($"{transitionCount} Transition(s)", countStyle);
                    // Preview first few destinations?
                    if (transitionListStyle == null)
                    {
                        transitionListStyle = new GUIStyle(GUI.skin.label);
                        transitionListStyle.fontSize = 9;
                        transitionListStyle.alignment = TextAnchor.MiddleCenter;
                        transitionListStyle.normal.textColor = new Color(0.7f, 0.7f, 0.7f);
                    }
                    
                    if (transitionCount > 0)
                    {
                         string firstTarget = transitions.GetArrayElementAtIndex(0).FindPropertyRelative("targetState").stringValue;
                         GUILayout.Label($"→ {firstTarget}" + (transitionCount > 1 ? "..." : ""), transitionListStyle);
                    }
                }
                else
                {
                    GUILayout.Label("No Transitions", countStyle);
                }
                
                EditorGUILayout.EndVertical();
                GUILayout.EndArea();
            }
        }
        
        private GUIStyle transitionListStyle;

        private void DrawAllGraphConnectionsSimple(Rect graphArea)
        {
            if (entriesProperty == null) return;
            
            // Clear drawn connections at start of each frame
            drawnConnections.Clear();
            
            // Draw connections from all states (using HashSet to prevent duplicates)
            for (int i = 0; i < entriesProperty.arraySize; i++)
            {
                var element = entriesProperty.GetArrayElementAtIndex(i);
                var fromState = element.FindPropertyRelative("stateName").stringValue;
                var transitionsProp = element.FindPropertyRelative("transitions");
                
                if (transitionsProp == null || !transitionsProp.isArray) continue;
                if (!nodePositions.ContainsKey(fromState)) continue;
                
                Rect fromRect = nodePositions[fromState];
                Vector2 fromPoint = new Vector2(fromRect.x + fromRect.width / 2, fromRect.y + fromRect.height);
                
                for (int j = 0; j < transitionsProp.arraySize; j++)
                {
                    var transition = transitionsProp.GetArrayElementAtIndex(j);
                    var targetState = transition.FindPropertyRelative("targetState");
                    var isAnyState = transition.FindPropertyRelative("isAnyState");
                    var clearAllStates = transition.FindPropertyRelative("clearAllStates");
                    
                    string toState = targetState.stringValue;
                    if (string.IsNullOrEmpty(toState) || !nodePositions.ContainsKey(toState)) continue;
                    
                    // Create unique connection key to prevent duplicates
                    string connectionKey = $"{fromState}->{toState}";
                    if (drawnConnections.Contains(connectionKey)) continue;
                    drawnConnections.Add(connectionKey);
                    
                    Rect toRect = nodePositions[toState];
                    Vector2 toPoint = new Vector2(toRect.x + toRect.width / 2, toRect.y);
                    
                    // Determine line color based on transition properties
                    Color lineColor = Color.white;
                    if (clearAllStates.boolValue)
                        lineColor = Color.red;
                    else if (isAnyState.boolValue)
                        lineColor = Color.yellow;
                    else
                        lineColor = Color.green;
                    
                    // Draw connection using Handles
                    DrawConnectionWithHandles(fromPoint, toPoint, lineColor, fromRect, toRect);
                }
            }
        }

        private void DrawConnectionWithHandles(Vector2 fromCenter, Vector2 toCenter, Color color, Rect fromRect, Rect toRect)
        {
            // Calculate best anchor points
            Vector2 start = fromCenter;
            Vector2 end = toCenter;
            Vector2 startTangent = Vector2.right;
            Vector2 endTangent = Vector2.left;
            
            // Determine relative positions
            bool isRight = toRect.xMin > fromRect.xMax + 20;
            bool isLeft = toRect.xMax < fromRect.xMin - 20;
            bool isBelow = toRect.yMin > fromRect.yMax + 20;
            bool isAbove = toRect.yMax < fromRect.yMin - 20;
            
            if (isRight)
            {
                start = new Vector2(fromRect.xMax, fromRect.center.y);
                end = new Vector2(toRect.xMin, toRect.center.y);
                startTangent = Vector2.right;
                endTangent = Vector2.left;
            }
            else if (isLeft)
            {
                start = new Vector2(fromRect.xMin, fromRect.center.y);
                end = new Vector2(toRect.xMax, toRect.center.y);
                startTangent = Vector2.left;
                endTangent = Vector2.right;
            }
            else if (isBelow)
            {
                start = new Vector2(fromRect.center.x, fromRect.yMax);
                end = new Vector2(toRect.center.x, toRect.yMin);
                startTangent = Vector2.up; // Actually down in GUI coords, but we'll scale it
                endTangent = Vector2.down;
            }
            else if (isAbove)
            {
                start = new Vector2(fromRect.center.x, fromRect.yMin);
                end = new Vector2(toRect.center.x, toRect.yMax);
                startTangent = Vector2.down;
                endTangent = Vector2.up;
            }
            
            // Adjust tangents based on distance
            float dist = Vector2.Distance(start, end);
            float tangentStrength = Mathf.Min(dist * 0.5f, 150f);
            
            // If strictly vertical, modify vertical tangents
            if (isBelow)
            {
                 startTangent = Vector2.up * tangentStrength; // down
                 endTangent = Vector2.down * tangentStrength; // up
            }
            else if (isAbove)
            {
                 startTangent = Vector2.down * tangentStrength;
                 endTangent = Vector2.up * tangentStrength;
            }
            else
            {
                startTangent *= tangentStrength;
                endTangent *= tangentStrength;
            }
            
            using (new Handles.DrawingScope(color))
            {
                Vector3 start3D = new Vector3(start.x, start.y, 0);
                Vector3 end3D = new Vector3(end.x, end.y, 0);
                Vector3 startTan3D = new Vector3(start.x + startTangent.x, start.y + startTangent.y, 0);
                Vector3 endTan3D = new Vector3(end.x + endTangent.x, end.y + endTangent.y, 0);

                if (isBelow || isAbove)
                {
                    // Flip Y for vertical tangents in 3D/Handle space if needed, 
                    // but typically Handles.DrawBezier works fine with screen coords z=0
                    startTan3D = new Vector3(start.x, start.y + (isBelow ? 1 : -1) * tangentStrength, 0);
                    endTan3D = new Vector3(end.x, end.y + (isBelow ? -1 : 1) * tangentStrength, 0);
                }

                Handles.DrawBezier(start3D, end3D, startTan3D, endTan3D, color, Texture2D.whiteTexture, 3f);
                
                // Draw Arrow
                Vector2 dir = (end - (Vector2)endTan3D).normalized;
                if (dir == Vector2.zero) dir = (end - start).normalized;
                
                Vector2 arrowBase = end - dir * 12f;
                Vector2 normal = new Vector2(-dir.y, dir.x) * 8f;
                
                Vector3[] arrowPoints = new Vector3[]
                {
                    end3D,
                    new Vector3(arrowBase.x + normal.x, arrowBase.y + normal.y, 0),
                    new Vector3(arrowBase.x - normal.x, arrowBase.y - normal.y, 0)
                };
                
                Handles.DrawAAConvexPolygon(arrowPoints);
            }
        }

        private void DrawRectOutline(Rect rect, Color color, float width)
        {
            using (new Handles.DrawingScope(color))
            {
                Handles.DrawLine(new Vector3(rect.x, rect.y), new Vector3(rect.xMax, rect.y), width);
                Handles.DrawLine(new Vector3(rect.xMax, rect.y), new Vector3(rect.xMax, rect.yMax), width);
                Handles.DrawLine(new Vector3(rect.xMax, rect.yMax), new Vector3(rect.x, rect.yMax), width);
                Handles.DrawLine(new Vector3(rect.x, rect.yMax), new Vector3(rect.x, rect.y), width);
            }
        }

        private void DrawGrid(Rect area, float gridSize, Color gridColor)
        {
            using (new Handles.DrawingScope(gridColor))
            {
                // Draw vertical lines
                for (float x = 0; x < area.width; x += gridSize)
                {
                    Handles.DrawLine(
                        new Vector3(area.x + x, area.y),
                        new Vector3(area.x + x, area.yMax),
                        1f
                    );
                }
                
                // Draw horizontal lines
                for (float y = 0; y < area.height; y += gridSize)
                {
                    Handles.DrawLine(
                        new Vector3(area.x, area.y + y),
                        new Vector3(area.xMax, area.y + y),
                        1f
                    );
                }
            }
        }

        private void HandleGraphInput(Rect graphArea)
        {
            Event e = Event.current;
            
            if (!graphArea.Contains(e.mousePosition))
                return;
            
            // Convert mouse position to graph coordinates (just apply pan offset)
            Vector2 graphMousePos = e.mousePosition - graphArea.position - graphPanOffset;
            
            // Check if clicking/dragging a node
            // Need to check in reverse order (top first)
            string clickedNode = null;
            
            // Use a temporary list to check hits to avoid collection modified errors or iteration issues
            var keys = new List<string>(nodePositions.Keys);
            
            foreach (var key in keys)
            {
                Rect nodeRect = nodePositions[key];
                if (nodeRect.Contains(graphMousePos))
                {
                    clickedNode = key;
                    break;
                }
            }
            
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0 && clickedNode != null)
                    {
                        draggedNode = clickedNode;
                        dragOffset = graphMousePos - nodePositions[clickedNode].position;
                        e.Use();
                        GUI.changed = true;
                    }
                    else if (e.button == 2 || (e.alt && e.button == 0))
                    {
                        // Start panning
                        e.Use();
                    }
                    else if (e.button == 0) 
                    {
                         // Click on empty space
                         GUI.FocusControl(null);
                    }
                    break;
                    
                case EventType.MouseDrag:
                    if (e.button == 0 && draggedNode != null)
                    {
                        // Drag node
                        var currentRect = nodePositions[draggedNode];
                        nodePositions[draggedNode] = new Rect(
                            graphMousePos.x - dragOffset.x,
                            graphMousePos.y - dragOffset.y,
                            currentRect.width,
                            currentRect.height
                        );
                        e.Use();
                        Repaint(); 
                    }
                    else if (e.button == 2 || (e.alt && e.button == 0))
                    {
                        // Pan view
                        graphPanOffset += e.delta;
                        e.Use();
                        Repaint();
                    }
                    break;
                    
                case EventType.MouseUp:
                    if (e.button == 0 && draggedNode != null)
                    {
                        draggedNode = null;
                        SaveNodePositions(); // Save on drag end
                        e.Use();
                    }
                    break;
            }
        }

        private void TestTransition(SerializedProperty transitionProp)
        {
            object targetObject = GetTargetObjectOfProperty(transitionProp);
            if (targetObject is UITransition transition)
            {
                transition.Run();
                Debug.Log($"Testing transition to: {transition.ToState}");
            }
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
                    int index = System.Convert.ToInt32(
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

            var f = type.GetField(name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (f != null) return f.GetValue(source);

            var p = type.GetProperty(name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);
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

        public static void DrawScript(Object target)
        {
            EditorGUI.BeginDisabledGroup(true);
            var monoScript = (target as MonoBehaviour) != null ? MonoScript.FromMonoBehaviour((MonoBehaviour)target) : MonoScript.FromScriptableObject((ScriptableObject)target);
            EditorGUILayout.ObjectField("Script", monoScript, target.GetType(), false);
            EditorGUI.EndDisabledGroup();
        }
        private void GenerateUIStateNames()
        {
            if (entriesProperty == null) return;
        
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("namespace THEBADDEST.UI");
            sb.AppendLine("{");
            sb.AppendLine("\tpublic static class UIStateNames");
            sb.AppendLine("\t{");
        
            HashSet<string> processedNames = new HashSet<string>();
        
            for (int i = 0; i < entriesProperty.arraySize; i++)
            {
                var element = entriesProperty.GetArrayElementAtIndex(i);
                var stateNameProp = element.FindPropertyRelative("stateName");
                string rawName = stateNameProp.stringValue;
        
                if (string.IsNullOrEmpty(rawName)) continue;
        
                // Sanitize name for variable usage (remove spaces, special chars)
                string varName = Regex.Replace(rawName, @"[^a-zA-Z0-9_]", "");
                
                // Ensure starts with letter or underscore
                if (varName.Length > 0 && char.IsDigit(varName[0])) varName = "_" + varName;
        
                if (!string.IsNullOrEmpty(varName) && !processedNames.Contains(varName))
                {
                    sb.AppendLine($"\t\tpublic const string {varName} = \"{rawName}\";");
                    processedNames.Add(varName);
                }
            }
        
            sb.AppendLine("\t}");
            sb.AppendLine("}");
        
            // Find path to save - default to Assets/UnityModularUI/Runtime/UIStateNames.cs
            string folderPath = "Assets/UnityModularUI/Runtime";
            string[] guids = AssetDatabase.FindAssets("UIState t:Script");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                folderPath = Path.GetDirectoryName(path);
            }
        
            string filePath = Path.Combine(folderPath, "UIStateNames.cs");
        
            try 
            {
                File.WriteAllText(filePath, sb.ToString());
                AssetDatabase.Refresh();
                EditorUtility.DisplayDialog("Success", $"UIStateNames.cs generated at:\n{filePath}", "OK");
            }
            catch (System.Exception e)
            {
                EditorUtility.DisplayDialog("Error", $"Failed to generate UIStateNames.cs:\n{e.Message}", "OK");
            }
        }
    }
}
