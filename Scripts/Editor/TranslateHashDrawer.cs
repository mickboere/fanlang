#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace FanLang
{
	[CustomPropertyDrawer(typeof(TranslateHashData))]
	public class TranslateHashDrawer : PropertyDrawer
	{
		private const float SPACING = 5f;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			// Using BeginProperty / EndProperty on the parent property means that
			// prefab override logic works on the entire property.
			EditorGUI.BeginProperty(position, label, property);

			// Draw label
			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			// Don't make child fields be indented
			var initialIndent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			// Calculate rect
			float width = position.width / 3f - SPACING;
			Rect pos = new Rect(position.x, position.y, width, position.height);
			width += SPACING;

			// Draw fields - passs GUIContent.none to each so they are drawn without labels
			EditorGUI.PropertyField(pos, property.FindPropertyRelative(nameof(TranslateHashData.Input)), GUIContent.none);
			pos.x += width;
			EditorGUI.PropertyField(pos, property.FindPropertyRelative(nameof(TranslateHashData.Output)), GUIContent.none);
			pos.x += width;
			EditorGUI.PropertyField(pos, property.FindPropertyRelative(nameof(TranslateHashData.HashType)), GUIContent.none);

			// Set indent back to what it was
			EditorGUI.indentLevel = initialIndent;

			EditorGUI.EndProperty();
		}
	}
}
#endif
