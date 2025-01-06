using System;


#if UNITY_EDITOR
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
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
	/// Custom property drawer for the StateOptionsAttribute.
	/// </summary>
	[CustomPropertyDrawer(typeof(OptionsAttribute))]
	public class OptionsDrawer : PropertyDrawer
	{
		private int                                 selected              = 0;
		private List<string>                        options               = new List<string>();
		private static Dictionary<Type, IEnumerable<Type>> inheritedClassesCache = new Dictionary<Type, IEnumerable<Type>>();

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			Type type         = (attribute as OptionsAttribute)?.Type;
			if (!inheritedClassesCache.TryGetValue(type, out IEnumerable<Type> typesDerived))
			{
				typesDerived                = GetInheritedClasses(type);
				inheritedClassesCache[type] = typesDerived;
			}

			options.Clear();
			foreach (var t in typesDerived)
			{
				options.Add(t.Name);
			}

			if(!string.IsNullOrEmpty(property.stringValue))
				selected=options.IndexOf(property.stringValue);
			if(selected<0|| selected>=options.Count)
				selected=0;
			selected = EditorGUI.Popup(position, property.displayName, selected, options.ToArray());
			if (options.Count != 0)
				property.stringValue = options[selected];
		}

		private IEnumerable<Type> GetInheritedClasses(Type givenType)
		{
			// Exclude abstract classes and find types that are subclasses of the given type.
			return AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(a => a.GetTypes())
				.Where(t => t.IsClass && !t.IsAbstract && (t.IsSubclassOf(givenType) || givenType.IsInterface && givenType.IsAssignableFrom(t)));
		}
	}
	#endif



}