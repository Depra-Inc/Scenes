// SPDX-License-Identifier: Apache-2.0
// © 2026 Depra <n.melnikov@depra.org>

using System;
using UnityEditor;
using UnityEngine;

namespace Depra.Scenes.Editor
{
	[CustomPropertyDrawer(typeof(SceneReference))]
	internal sealed class SceneReferenceDrawer : PropertyDrawer
	{
		private const string GUID_FIELD = "_guid";
		private const string PATH_FIELD = "_path";
		private const float BUTTON_WIDTH = 46f;

		private static readonly Color MISSING_TINT = new(1f, 0.35f, 0.3f);
		private static readonly Color EXCLUDED_TINT = new(1f, 0.8f, 0.25f);

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var guidProperty = property.FindPropertyRelative(GUID_FIELD);
			var pathProperty = property.FindPropertyRelative(PATH_FIELD);

			EditorGUI.BeginProperty(position, label, property);

			var path = SceneReferenceEditor.ResolvePath(guidProperty.hash128Value);
			var asset = string.IsNullOrEmpty(path) ? null : AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
			var status = SceneReferenceEditor.GetStatus(guidProperty.hash128Value);

			var fieldRect = position;
			if (status is SceneStatus.NOT_IN_BUILD or SceneStatus.DISABLED_IN_BUILD)
			{
				fieldRect.width -= BUTTON_WIDTH + 2f;
			}

			if (status != SceneStatus.OK && status != SceneStatus.NONE)
			{
				label = new GUIContent(label) { image = EditorGUIUtility.IconContent("console.warnicon.sml").image, tooltip = Describe(status) };
			}

			var tint = GetTint(status);
			var previousBackground = GUI.backgroundColor;
			if (tint.HasValue)
			{
				GUI.backgroundColor = tint.Value;
			}

			var valueRect = EditorGUI.PrefixLabel(fieldRect, GUIUtility.GetControlID(FocusType.Passive), label);
			var indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			EditorGUI.BeginChangeCheck();
			var newAsset = (SceneAsset) EditorGUI.ObjectField(valueRect, asset, typeof(SceneAsset), false);
			GUI.backgroundColor = previousBackground;
			EditorGUI.indentLevel = indent;

			if (EditorGUI.EndChangeCheck())
			{
				var newPath = newAsset ? AssetDatabase.GetAssetPath(newAsset) : string.Empty;
				guidProperty.hash128Value = newAsset ? Hash128.Parse(AssetDatabase.AssetPathToGUID(newPath)) : default;
				pathProperty.stringValue = newPath;
			}

			if (status is SceneStatus.NOT_IN_BUILD or SceneStatus.DISABLED_IN_BUILD)
			{
				var buttonRect = new Rect(fieldRect.xMax + 2f, position.y, BUTTON_WIDTH, EditorGUIUtility.singleLineHeight);
				if (GUI.Button(buttonRect, new GUIContent("Build", "Add the scene to Build Settings and enable it")))
				{
					SceneReferenceEditor.EnableInBuild(path);
				}
			}

			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
			EditorGUIUtility.singleLineHeight;

		private static Color? GetTint(SceneStatus status) => status switch
		{
			SceneStatus.MISSING => MISSING_TINT,
			SceneStatus.NOT_IN_BUILD or SceneStatus.DISABLED_IN_BUILD => EXCLUDED_TINT,
			_ => null
		};

		private static string Describe(SceneStatus status) => status switch
		{
			SceneStatus.MISSING => "Referenced scene asset no longer exists.",
			SceneStatus.NOT_IN_BUILD => "Scene is not in Build Settings.",
			SceneStatus.DISABLED_IN_BUILD => "Scene is disabled in Build Settings.",
			_ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
		};
	}
}
