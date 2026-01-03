using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditorInternal;
using Object = UnityEngine.Object;


namespace THEBADDEST.UI.Editor
{
    /// <summary>
    /// Provides context menu items to convert Unity components to custom view components
    /// </summary>
    public static class ComponentConverter
    {
        /// <summary>
        /// Helper class to store serialized property data
        /// </summary>
        private class SerializedPropertyData
        {
            public SerializedPropertyType propertyType;
            public object value;
            public string propertyPath;
        }
        // Mapping of Unity component types to custom view component types
        private static readonly Type[] ConvertibleTypes = new Type[]
        {
            typeof(TextMeshProUGUI),
            typeof(Image),
            typeof(Button),
            typeof(Toggle),
            typeof(Slider),
            typeof(TMP_Dropdown),
            typeof(TMP_InputField),
            typeof(ScrollRect),
        };

        [MenuItem("CONTEXT/TextMeshProUGUI/Convert to TextView", false, 1000)]
        private static void ConvertTextMeshProUGUIToTextView(MenuCommand command)
        {
            ConvertComponent<TextMeshProUGUI, TextView>(command.context as Component);
        }

        [MenuItem("CONTEXT/Image/Convert to ImageView", false, 1000)]
        private static void ConvertImageToImageView(MenuCommand command)
        {
            ConvertComponent<Image, ImageView>(command.context as Component);
        }

        [MenuItem("CONTEXT/Image/Convert to IconView", false, 1001)]
        private static void ConvertImageToIconView(MenuCommand command)
        {
            ConvertComponent<Image, IconView>(command.context as Component);
        }

        [MenuItem("CONTEXT/Image/Convert to ImageButtonView", false, 1002)]
        private static void ConvertImageToImageButtonView(MenuCommand command)
        {
            ConvertComponent<Image, ImageButtonView>(command.context as Component);
        }

        [MenuItem("CONTEXT/Button/Convert to ButtonView", false, 1000)]
        private static void ConvertButtonToButtonView(MenuCommand command)
        {
            ConvertNonInheritingComponent<Button, ButtonView>(command.context as Component);
        }

        [MenuItem("CONTEXT/Toggle/Convert to ToggleView", false, 1000)]
        private static void ConvertToggleToToggleView(MenuCommand command)
        {
            ConvertNonInheritingComponent<Toggle, ToggleView>(command.context as Component);
        }

        [MenuItem("CONTEXT/Slider/Convert to SliderView", false, 1000)]
        private static void ConvertSliderToSliderView(MenuCommand command)
        {
            ConvertComponent<Slider, SliderView>(command.context as Component);
        }

        [MenuItem("CONTEXT/TMP_Dropdown/Convert to DropdownView", false, 1000)]
        private static void ConvertTMPDropdownToDropdownView(MenuCommand command)
        {
            ConvertComponent<TMP_Dropdown, DropdownView>(command.context as Component);
        }

        [MenuItem("CONTEXT/TMP_InputField/Convert to InputFieldView", false, 1000)]
        private static void ConvertTMPInputFieldToInputFieldView(MenuCommand command)
        {
            ConvertComponent<TMP_InputField, InputFieldView>(command.context as Component);
        }

        [MenuItem("CONTEXT/ScrollRect/Convert to ScrollDataView", false, 1000)]
        private static void ConvertScrollRectToScrollDataView(MenuCommand command)
        {
            ConvertComponent<ScrollRect, ScrollDataView>(command.context as Component);
        }

        // ========== CREATE MENU ITEMS ==========
        // These menu items appear in GameObject > UI > Modular UI submenu to directly create custom view components
        // Priority numbers create natural separators (gaps in priority create visual separators in Unity menus)

        // ========== Text & Image Components ==========
        [MenuItem("GameObject/UI/Modular UI/TextView", false, 2001)]
        private static void CreateTextView(MenuCommand menuCommand)
        {
            CreateUIElement<TextView>("TextView");
        }

        [MenuItem("GameObject/UI/Modular UI/ImageView", false, 2002)]
        private static void CreateImageView(MenuCommand menuCommand)
        {
            CreateUIElement<ImageView>("ImageView");
        }

        [MenuItem("GameObject/UI/Modular UI/IconView", false, 2003)]
        private static void CreateIconView(MenuCommand menuCommand)
        {
            CreateUIElement<IconView>("IconView");
        }

        [MenuItem("GameObject/UI/Modular UI/ImageButtonView", false, 2004)]
        private static void CreateImageButtonView(MenuCommand menuCommand)
        {
            CreateUIElement<ImageButtonView>("ImageButtonView");
        }

        // ========== Interactive Components ==========
        [MenuItem("GameObject/UI/Modular UI/ButtonView", false, 2010)]
        private static void CreateButtonView(MenuCommand menuCommand)
        {
            CreateUIElement<ButtonView>("ButtonView");
        }

        [MenuItem("GameObject/UI/Modular UI/ToggleView", false, 2011)]
        private static void CreateToggleView(MenuCommand menuCommand)
        {
            CreateUIElement<ToggleView>("ToggleView");
        }

        [MenuItem("GameObject/UI/Modular UI/SliderView", false, 2012)]
        private static void CreateSliderView(MenuCommand menuCommand)
        {
            CreateUIElement<SliderView>("SliderView");
        }

        [MenuItem("GameObject/UI/Modular UI/DropdownView", false, 2013)]
        private static void CreateDropdownView(MenuCommand menuCommand)
        {
            CreateUIElement<DropdownView>("DropdownView");
        }

        [MenuItem("GameObject/UI/Modular UI/InputFieldView", false, 2014)]
        private static void CreateInputFieldView(MenuCommand menuCommand)
        {
            CreateUIElement<InputFieldView>("InputFieldView");
        }

        [MenuItem("GameObject/UI/Modular UI/ScrollDataView", false, 2015)]
        private static void CreateScrollDataView(MenuCommand menuCommand)
        {
            CreateUIElement<ScrollDataView>("ScrollDataView");
        }

        // ========== Container & Layout Components ==========
        [MenuItem("GameObject/UI/Modular UI/PanelView", false, 2020)]
        private static void CreatePanelView(MenuCommand menuCommand)
        {
            CreateUIElement<PanelView>("PanelView");
        }

        [MenuItem("GameObject/UI/Modular UI/TabView", false, 2021)]
        private static void CreateTabView(MenuCommand menuCommand)
        {
            CreateUIElement<TabView>("TabView");
        }

        // ========== Utility Components ==========
        [MenuItem("GameObject/UI/Modular UI/ProgressBarView", false, 2030)]
        private static void CreateProgressBarView(MenuCommand menuCommand)
        {
            CreateUIElement<ProgressBarView>("ProgressBarView");
        }

        [MenuItem("GameObject/UI/Modular UI/StepperView", false, 2031)]
        private static void CreateStepperView(MenuCommand menuCommand)
        {
            CreateUIElement<StepperView>("StepperView");
        }

        [MenuItem("GameObject/UI/Modular UI/TooltipView", false, 2032)]
        private static void CreateTooltipView(MenuCommand menuCommand)
        {
            CreateUIElement<TooltipView>("TooltipView");
        }

        [MenuItem("GameObject/UI/Modular UI/ColorPickerView", false, 2033)]
        private static void CreateColorPickerView(MenuCommand menuCommand)
        {
            CreateUIElement<ColorPickerView>("ColorPickerView");
        }

        /// <summary>
        /// Creates a UI element with the specified component type, ensuring it has a Canvas parent
        /// </summary>
        private static void CreateUIElement<T>(string name) where T : Component
        {
            // Get the parent transform (from menu command or selection)
            Transform parent = null;
            GameObject selected = Selection.activeGameObject;
            
            if (selected != null)
            {
                // Check if selected object has a Canvas or is a Canvas
                Canvas canvas = selected.GetComponent<Canvas>();
                if (canvas == null)
                {
                    canvas = selected.GetComponentInParent<Canvas>();
                }
                
                if (canvas != null)
                {
                    parent = selected.transform;
                }
            }

            // If no valid parent found, find or create a Canvas
            if (parent == null)
            {
                Canvas canvas = Object.FindObjectOfType<Canvas>();
                if (canvas == null)
                {
                    // Create a new Canvas
                    GameObject canvasGO = new GameObject("Canvas");
                    canvas = canvasGO.AddComponent<Canvas>();
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
                    canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                    
                    // Create EventSystem if it doesn't exist
                    if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
                    {
                        GameObject eventSystem = new GameObject("EventSystem");
                        eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
                        eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                    }
                    
                    parent = canvas.transform;
                    Undo.RegisterCreatedObjectUndo(canvasGO, "Create " + name);
                }
                else
                {
                    parent = canvas.transform;
                }
            }

            // Create the GameObject with the component
            GameObject go = new GameObject(name);
            T component = go.AddComponent<T>();
            
            // Set parent
            go.transform.SetParent(parent, false);
            
            // Set RectTransform defaults
            RectTransform rectTransform = go.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = Vector2.zero;
                rectTransform.sizeDelta = new Vector2(100, 100);
            }

            // Register undo and select
            Undo.RegisterCreatedObjectUndo(go, "Create " + name);
            Selection.activeGameObject = go;
        }

        /// <summary>
        /// Converts a Unity component to a custom view component while preserving all properties
        /// Used for components where the target inherits from the source
        /// </summary>
        private static void ConvertComponent<TSource, TTarget>(Component sourceComponent)
            where TSource : Component
            where TTarget : Component
        {
            if (sourceComponent == null)
            {
                Debug.LogError("Source component is null");
                return;
            }

            if (!(sourceComponent is TSource))
            {
                Debug.LogError($"Component is not of type {typeof(TSource).Name}");
                return;
            }

            GameObject gameObject = sourceComponent.gameObject;

            // Check if target component already exists
            TTarget existingTarget = gameObject.GetComponent<TTarget>();
            if (existingTarget != null)
            {
                Debug.LogWarning($"{typeof(TTarget).Name} already exists on {gameObject.name}");
                return;
            }

            // Record undo
            Undo.RegisterCompleteObjectUndo(gameObject, $"Convert {typeof(TSource).Name} to {typeof(TTarget).Name}");

            // Check if target inherits from source (they share the same base component type)
            // In this case, we must remove source before adding target
            bool targetInheritsFromSource = typeof(TSource).IsAssignableFrom(typeof(TTarget));

            TTarget targetComponent;

            if (targetInheritsFromSource)
            {
                // For inheriting types, we need to:
                // 1. Copy ALL properties manually first (before removing source)
                // 2. Remove source component
                // 3. Add target component
                // 4. Apply all copied properties
                
                // Step 1: Copy all properties from source BEFORE destroying it
                SerializedObject sourceSerializedObject = new SerializedObject(sourceComponent);
                sourceSerializedObject.Update();
                
                // Store all property values with their paths
                var allProperties = new System.Collections.Generic.Dictionary<string, SerializedPropertyData>();
                SerializedProperty sourceIterator = sourceSerializedObject.GetIterator();
                
                // Iterate through ALL properties including nested ones
                if (sourceIterator.NextVisible(true))
                {
                    do
                    {
                        if (sourceIterator.name == "m_Script") continue; // Skip script reference
                        
                        // Store property data
                        allProperties[sourceIterator.propertyPath] = new SerializedPropertyData
                        {
                            propertyType = sourceIterator.propertyType,
                            value = GetPropertyValue(sourceIterator),
                            propertyPath = sourceIterator.propertyPath
                        };
                        
                        // Also handle arrays and nested structures
                        if (sourceIterator.isArray && sourceIterator.propertyType == SerializedPropertyType.Generic)
                        {
                            for (int i = 0; i < sourceIterator.arraySize; i++)
                            {
                                SerializedProperty arrayElement = sourceIterator.GetArrayElementAtIndex(i);
                                string arrayPath = $"{sourceIterator.propertyPath}.Array.data[{i}]";
                                CopyPropertyRecursively(arrayElement, allProperties, arrayPath);
                            }
                        }
                        else if (sourceIterator.propertyType == SerializedPropertyType.Generic && sourceIterator.hasChildren)
                        {
                            CopyPropertyRecursively(sourceIterator, allProperties, sourceIterator.propertyPath);
                        }
                    }
                    while (sourceIterator.NextVisible(false));
                }
                
                // Step 2: Use ComponentUtility as backup (copies to clipboard)
                ComponentUtility.CopyComponent(sourceComponent);
                
                // Step 3: Remove source component (required before adding target that inherits from it)
                Undo.DestroyObjectImmediate(sourceComponent);
                
                // Step 4: Add target component
                targetComponent = Undo.AddComponent<TTarget>(gameObject);
                
                // Step 5: Apply all properties to target
                SerializedObject targetSerializedObject = new SerializedObject(targetComponent);
                targetSerializedObject.Update();
                
                int copiedCount = 0;
                foreach (var propData in allProperties)
                {
                    SerializedProperty targetProp = targetSerializedObject.FindProperty(propData.Key);
                    if (targetProp != null)
                    {
                        try
                        {
                            SetPropertyValue(targetProp, propData.Value.value);
                            copiedCount++;
                        }
                        catch (Exception e)
                        {
                            Debug.LogWarning($"Could not copy property {propData.Key}: {e.Message}");
                        }
                    }
                }
                
                targetSerializedObject.ApplyModifiedProperties();
                
                // Step 6: Also try ComponentUtility paste as additional safety
                try
                {
                    ComponentUtility.PasteComponentValues(targetComponent);
                }
                catch
                {
                    // ComponentUtility might fail if types don't match exactly, that's okay
                }
                
                Debug.Log($"Copied {copiedCount} properties from {typeof(TSource).Name} to {typeof(TTarget).Name}");
            }
            else
            {
                // For non-inheriting cases, we can add target first
                targetComponent = Undo.AddComponent<TTarget>(gameObject);
                
                // Copy properties using ComponentUtility
                ComponentUtility.CopyComponent(sourceComponent);
                ComponentUtility.PasteComponentValues(targetComponent);
                
                // Also manually copy compatible properties
                CopyAllPropertiesManually(sourceComponent, targetComponent, typeof(TSource));
                
                // Remove source component
                Undo.DestroyObjectImmediate(sourceComponent);
            }
        

            // Mark scene as dirty
            if (!Application.isPlaying)
            {
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
            }

            Debug.Log($"Successfully converted {typeof(TSource).Name} to {typeof(TTarget).Name} on {gameObject.name}");
        }

        /// <summary>
        /// Manually copies all properties from source to target component
        /// This ensures all properties are preserved, including arrays and nested structures
        /// </summary>
        private static void CopyAllPropertiesManually(Component source, Component target, Type sourceType)
        {
            if (source == null || target == null) return;

            SerializedObject sourceObject = new SerializedObject(source);
            SerializedObject targetObject = new SerializedObject(target);
            
            sourceObject.Update();
            targetObject.Update();

            SerializedProperty sourceIterator = sourceObject.GetIterator();
            int copiedCount = 0;

            // Iterate through all source properties
            if (sourceIterator.NextVisible(true))
            {
                do
                {
                    if (sourceIterator.name == "m_Script") continue; // Skip script reference
                    
                    // Find corresponding property in target
                    SerializedProperty targetProp = targetObject.FindProperty(sourceIterator.propertyPath);
                    if (targetProp != null)
                    {
                        // Copy the property value
                        if (CopySerializedProperty(sourceIterator, targetProp))
                        {
                            copiedCount++;
                        }
                    }
                }
                while (sourceIterator.NextVisible(false));
            }

            targetObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Copies a serialized property from source to target, handling all property types including arrays
        /// </summary>
        private static bool CopySerializedProperty(SerializedProperty source, SerializedProperty target)
        {
            try
            {
                // Handle arrays
                if (source.isArray && target.isArray)
                {
                    target.arraySize = source.arraySize;
                    for (int i = 0; i < source.arraySize; i++)
                    {
                        SerializedProperty sourceElement = source.GetArrayElementAtIndex(i);
                        SerializedProperty targetElement = target.GetArrayElementAtIndex(i);
                        CopySerializedPropertyRecursive(sourceElement, targetElement);
                    }
                    return true;
                }
                
                // Handle regular properties
                if (source.propertyType == target.propertyType)
                {
                    switch (source.propertyType)
                    {
                        case SerializedPropertyType.Integer:
                            target.intValue = source.intValue;
                            return true;
                        case SerializedPropertyType.Boolean:
                            target.boolValue = source.boolValue;
                            return true;
                        case SerializedPropertyType.Float:
                            target.floatValue = source.floatValue;
                            return true;
                        case SerializedPropertyType.String:
                            target.stringValue = source.stringValue;
                            return true;
                        case SerializedPropertyType.Color:
                            target.colorValue = source.colorValue;
                            return true;
                        case SerializedPropertyType.ObjectReference:
                            target.objectReferenceValue = source.objectReferenceValue;
                            return true;
                        case SerializedPropertyType.LayerMask:
                            target.intValue = source.intValue;
                            return true;
                        case SerializedPropertyType.Enum:
                            target.enumValueIndex = source.enumValueIndex;
                            return true;
                        case SerializedPropertyType.Vector2:
                            target.vector2Value = source.vector2Value;
                            return true;
                        case SerializedPropertyType.Vector3:
                            target.vector3Value = source.vector3Value;
                            return true;
                        case SerializedPropertyType.Vector4:
                            target.vector4Value = source.vector4Value;
                            return true;
                        case SerializedPropertyType.Rect:
                            target.rectValue = source.rectValue;
                            return true;
                        case SerializedPropertyType.AnimationCurve:
                            target.animationCurveValue = source.animationCurveValue;
                            return true;
                        case SerializedPropertyType.Bounds:
                            target.boundsValue = source.boundsValue;
                            return true;
                        case SerializedPropertyType.Quaternion:
                            target.quaternionValue = source.quaternionValue;
                            return true;
                        case SerializedPropertyType.Vector2Int:
                            target.vector2IntValue = source.vector2IntValue;
                            return true;
                        case SerializedPropertyType.Vector3Int:
                            target.vector3IntValue = source.vector3IntValue;
                            return true;
                        case SerializedPropertyType.RectInt:
                            target.rectIntValue = source.rectIntValue;
                            return true;
                        case SerializedPropertyType.BoundsInt:
                            target.boundsIntValue = source.boundsIntValue;
                            return true;
                        case SerializedPropertyType.Generic:
                            // Handle nested structures
                            return CopySerializedPropertyRecursive(source, target);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to copy property {source.propertyPath}: {e.Message}");
            }
            
            return false;
        }

        /// <summary>
        /// Recursively copies a property and all its children to the properties dictionary
        /// </summary>
        private static void CopyPropertyRecursively(SerializedProperty prop, System.Collections.Generic.Dictionary<string, SerializedPropertyData> properties, string basePath)
        {
            SerializedProperty copy = prop.Copy();
            SerializedProperty end = prop.GetEndProperty();
            
            if (copy.NextVisible(true))
            {
                do
                {
                    if (SerializedProperty.EqualContents(copy, end)) break;
                    
                    // Use the actual property path from the copy
                    string fullPath = copy.propertyPath;
                    
                    // Only store leaf properties (actual values, not containers)
                    if (copy.propertyType != SerializedPropertyType.Generic && 
                        copy.propertyType != SerializedPropertyType.ArraySize)
                    {
                        if (!properties.ContainsKey(fullPath))
                        {
                            properties[fullPath] = new SerializedPropertyData
                            {
                                propertyType = copy.propertyType,
                                value = GetPropertyValue(copy),
                                propertyPath = fullPath
                            };
                        }
                    }
                    else if (copy.hasChildren)
                    {
                        // Recursively copy nested properties (path is already correct from copy.propertyPath)
                        CopyPropertyRecursively(copy, properties, fullPath);
                    }
                }
                while (copy.NextVisible(false));
            }
        }

        /// <summary>
        /// Recursively copies nested properties
        /// </summary>
        private static bool CopySerializedPropertyRecursive(SerializedProperty source, SerializedProperty target)
        {
            if (!source.hasChildren || !target.hasChildren) return false;

            bool copied = false;
            SerializedProperty sourceCopy = source.Copy();
            SerializedProperty targetCopy = target.Copy();
            SerializedProperty sourceEnd = source.GetEndProperty();
            SerializedProperty targetEnd = target.GetEndProperty();

            if (sourceCopy.NextVisible(true) && targetCopy.NextVisible(true))
            {
                do
                {
                    if (SerializedProperty.EqualContents(sourceCopy, sourceEnd) || 
                        SerializedProperty.EqualContents(targetCopy, targetEnd))
                        break;

                    if (sourceCopy.propertyPath == targetCopy.propertyPath)
                    {
                        if (CopySerializedProperty(sourceCopy, targetCopy))
                        {
                            copied = true;
                        }
                    }
                }
                while (sourceCopy.NextVisible(false) && targetCopy.NextVisible(false));
            }

            return copied;
        }

        /// <summary>
        /// Converts a Unity component to a custom view component for non-inheriting types
        /// Used when the target doesn't inherit from the source (e.g., Button -> ButtonView)
        /// </summary>
        private static void ConvertNonInheritingComponent<TSource, TTarget>(Component sourceComponent)
            where TSource : Component
            where TTarget : Component
        {
            if (sourceComponent == null)
            {
                Debug.LogError("Source component is null");
                return;
            }

            if (!(sourceComponent is TSource))
            {
                Debug.LogError($"Component is not of type {typeof(TSource).Name}");
                return;
            }

            GameObject gameObject = sourceComponent.gameObject;

            // Check if target component already exists
            TTarget existingTarget = gameObject.GetComponent<TTarget>();
            if (existingTarget != null)
            {
                Debug.LogWarning($"{typeof(TTarget).Name} already exists on {gameObject.name}");
                return;
            }

            // Record undo
            Undo.RegisterCompleteObjectUndo(gameObject, $"Convert {typeof(TSource).Name} to {typeof(TTarget).Name}");

            // Add target component
            TTarget targetComponent = Undo.AddComponent<TTarget>(gameObject);

            // Try to copy any compatible properties (mostly GameObject-level properties)
            SerializedObject sourceSerializedObject = new SerializedObject(sourceComponent);
            SerializedObject targetSerializedObject = new SerializedObject(targetComponent);
            
            sourceSerializedObject.Update();
            targetSerializedObject.Update();

            // Copy common MonoBehaviour properties if they exist
            SerializedProperty sourceEnabled = sourceSerializedObject.FindProperty("m_Enabled");
            SerializedProperty targetEnabled = targetSerializedObject.FindProperty("m_Enabled");
            if (sourceEnabled != null && targetEnabled != null)
            {
                targetEnabled.boolValue = sourceEnabled.boolValue;
            }

            targetSerializedObject.ApplyModifiedProperties();

            // Remove source component
            Undo.DestroyObjectImmediate(sourceComponent);

            // Mark scene as dirty
            if (!Application.isPlaying)
            {
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
            }

            Debug.Log($"Successfully converted {typeof(TSource).Name} to {typeof(TTarget).Name} on {gameObject.name}. " +
                     $"Note: Some properties from {typeof(TSource).Name} may not be preserved as {typeof(TTarget).Name} doesn't inherit from it.");
        }

        /// <summary>
        /// Gets the value of a serialized property
        /// </summary>
        private static object GetPropertyValue(SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    return property.intValue;
                case SerializedPropertyType.Boolean:
                    return property.boolValue;
                case SerializedPropertyType.Float:
                    return property.floatValue;
                case SerializedPropertyType.String:
                    return property.stringValue;
                case SerializedPropertyType.Color:
                    return property.colorValue;
                case SerializedPropertyType.ObjectReference:
                    return property.objectReferenceValue;
                case SerializedPropertyType.LayerMask:
                    return property.intValue;
                case SerializedPropertyType.Enum:
                    return property.enumValueIndex;
                case SerializedPropertyType.Vector2:
                    return property.vector2Value;
                case SerializedPropertyType.Vector3:
                    return property.vector3Value;
                case SerializedPropertyType.Vector4:
                    return property.vector4Value;
                case SerializedPropertyType.Rect:
                    return property.rectValue;
                case SerializedPropertyType.ArraySize:
                    return property.arraySize;
                case SerializedPropertyType.Character:
                    return (char)property.intValue;
                case SerializedPropertyType.AnimationCurve:
                    return property.animationCurveValue;
                case SerializedPropertyType.Bounds:
                    return property.boundsValue;
                case SerializedPropertyType.Gradient:
                    return property.gradientValue;
                case SerializedPropertyType.Quaternion:
                    return property.quaternionValue;
                case SerializedPropertyType.ExposedReference:
                    return property.exposedReferenceValue;
                case SerializedPropertyType.FixedBufferSize:
                    return property.fixedBufferSize;
                case SerializedPropertyType.Vector2Int:
                    return property.vector2IntValue;
                case SerializedPropertyType.Vector3Int:
                    return property.vector3IntValue;
                case SerializedPropertyType.RectInt:
                    return property.rectIntValue;
                case SerializedPropertyType.BoundsInt:
                    return property.boundsIntValue;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Sets the value of a serialized property
        /// </summary>
        private static void SetPropertyValue(SerializedProperty property, object value)
        {
            if (value == null) return;

            try
            {
                switch (property.propertyType)
                {
                    case SerializedPropertyType.Integer:
                        property.intValue = (int)value;
                        break;
                    case SerializedPropertyType.Boolean:
                        property.boolValue = (bool)value;
                        break;
                    case SerializedPropertyType.Float:
                        property.floatValue = (float)value;
                        break;
                    case SerializedPropertyType.String:
                        property.stringValue = (string)value;
                        break;
                    case SerializedPropertyType.Color:
                        property.colorValue = (Color)value;
                        break;
                    case SerializedPropertyType.ObjectReference:
                        property.objectReferenceValue = value as UnityEngine.Object;
                        break;
                    case SerializedPropertyType.LayerMask:
                        property.intValue = (int)value;
                        break;
                    case SerializedPropertyType.Enum:
                        property.enumValueIndex = (int)value;
                        break;
                    case SerializedPropertyType.Vector2:
                        property.vector2Value = (Vector2)value;
                        break;
                    case SerializedPropertyType.Vector3:
                        property.vector3Value = (Vector3)value;
                        break;
                    case SerializedPropertyType.Vector4:
                        property.vector4Value = (Vector4)value;
                        break;
                    case SerializedPropertyType.Rect:
                        property.rectValue = (Rect)value;
                        break;
                    case SerializedPropertyType.Character:
                        property.intValue = (char)value;
                        break;
                    case SerializedPropertyType.AnimationCurve:
                        property.animationCurveValue = (AnimationCurve)value;
                        break;
                    case SerializedPropertyType.Bounds:
                        property.boundsValue = (Bounds)value;
                        break;
                    case SerializedPropertyType.Quaternion:
                        property.quaternionValue = (Quaternion)value;
                        break;
                    case SerializedPropertyType.Vector2Int:
                        property.vector2IntValue = (Vector2Int)value;
                        break;
                    case SerializedPropertyType.Vector3Int:
                        property.vector3IntValue = (Vector3Int)value;
                        break;
                    case SerializedPropertyType.RectInt:
                        property.rectIntValue = (RectInt)value;
                        break;
                    case SerializedPropertyType.BoundsInt:
                        property.boundsIntValue = (BoundsInt)value;
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to set property {property.name}: {e.Message}");
            }
        }
    }
}

