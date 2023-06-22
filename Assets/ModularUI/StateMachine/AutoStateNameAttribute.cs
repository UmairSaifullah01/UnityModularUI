using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;


namespace THEBADDEST
{


	public class AutoStateNameAttribute : PropertyAttribute
	{

		public AutoStateNameAttribute()
		{
		}

	}

	#if UNITY_EDITOR
	[CustomPropertyDrawer(typeof(AutoStateNameAttribute))]
	public class AutoStateNameDrawer : PropertyDrawer
	{

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			property.stringValue = property.serializedObject.targetObject.GetType().Name;
			GUI.enabled          = false;
			EditorGUI.PropertyField(position, property, label, true);
			GUI.enabled = true;
		}

	}

	#endif


}