using UnityEditor;
using UnityEngine;
using SardineTools;

[CustomPropertyDrawer(typeof(Interval))]
public class IntervalDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		// Calculate rects :
		// X = 0-35 ; 40-55 ; 60-95
		Rect minRect = new Rect(position.x, position.y, position.width / 2 - 15, position.height);
		Rect toRect = new Rect(position.x + position.width / 2 - 10, position.y, 15, position.height);
		Rect maxRect = new Rect(position.x + position.width / 2 + 10, position.y, position.width / 2 - 10, position.height);

		// Draw fields - passs GUIContent.none to each so they are drawn without labels
		EditorGUI.PropertyField(minRect, property.FindPropertyRelative("_min"), GUIContent.none);
		EditorGUI.LabelField(toRect, "to");
		EditorGUI.PropertyField(maxRect, property.FindPropertyRelative("_max"), GUIContent.none);
		
		EditorGUI.indentLevel = indent;

		EditorGUI.EndProperty();
	}
}
