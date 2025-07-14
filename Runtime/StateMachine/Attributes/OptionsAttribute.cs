using System;
#if UNITY_EDITOR
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
#endif
using UnityEngine;

namespace THEBADDEST
{
    /// <summary>
    /// Attribute used to specify state options for a serialized property.
    /// </summary>
    public class OptionsAttribute : PropertyAttribute
    {
        public Type Type { get; }

        public OptionsAttribute(Type type)
        {
            Type = type;
        }
    }

#if UNITY_EDITOR

    /// <summary>
    /// Custom property drawer for the OptionsAttribute.
    /// </summary>
    [CustomPropertyDrawer(typeof(OptionsAttribute))]
    public class OptionsDrawer : PropertyDrawer
    {
        private int selected = 0;
        private static readonly Dictionary<Type, List<string>> optionsCache = new Dictionary<Type, List<string>>();
        private static readonly Dictionary<Type, IEnumerable<Type>> inheritedClassesCache = new Dictionary<Type, IEnumerable<Type>>();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            OptionsAttribute optionsAttribute = attribute as OptionsAttribute;
            Type type = optionsAttribute?.Type;

            if (type == null)
            {
                EditorGUI.LabelField(position, label.text, "Invalid Type");
                return;
            }

            if (!optionsCache.TryGetValue(type, out List<string> options))
            {
                options = new List<string>();
                if (!inheritedClassesCache.TryGetValue(type, out IEnumerable<Type> typesDerived))
                {
                    typesDerived = GetInheritedClasses(type);
                    inheritedClassesCache[type] = typesDerived;
                }

                foreach (var t in typesDerived)
                {
                    options.Add(t.Name);
                }

                optionsCache[type] = options;
            }

            // Ensure options are available
            if (options.Count == 0)
            {
                EditorGUI.LabelField(position, label.text, "No valid options found");
                return;
            }

            // Determine the selected index
            selected = options.IndexOf(property.stringValue);
            if (selected < 0 || selected >= options.Count)
                selected = 0;

            selected = EditorGUI.Popup(position, label.text, selected, options.ToArray());
            property.stringValue = options[selected];
        }

        private IEnumerable<Type> GetInheritedClasses(Type givenType)
        {
            // Exclude abstract classes and find types that are subclasses of the given type.
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic) // Exclude dynamic assemblies for performance
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract && (t.IsSubclassOf(givenType) || givenType.IsInterface && givenType.IsAssignableFrom(t)));
        }
    }
#endif
}
