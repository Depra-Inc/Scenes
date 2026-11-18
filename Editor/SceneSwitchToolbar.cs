// SPDX-License-Identifier: Apache-2.0
// © 2025 Depra <n.melnikov@depra.org>

using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Depra.Scenes.Editor
{
	[InitializeOnLoad]
	internal static class SceneSwitchToolbar
	{
		private const string PATH = "Depra/Scene Switch";
		private const string ALL_SCENES = "All Scenes/";

		static SceneSwitchToolbar()
		{
			EditorSceneManager.activeSceneChangedInEditMode += (_, _) => Refresh();
			EditorSceneManager.sceneOpened += (_, _) => Refresh();
			EditorSceneManager.sceneSaved += _ => Refresh();
			EditorApplication.playModeStateChanged += _ => Refresh();
		}

		[MainToolbarElement(PATH, defaultDockPosition = MainToolbarDockPosition.Middle)]
		private static MainToolbarElement Create()
		{
			var scene = SceneManager.GetActiveScene();
			var name = string.IsNullOrEmpty(scene.name) ? "Untitled" : scene.name;
			var icon = EditorGUIUtility.IconContent("SceneAsset Icon").image as Texture2D;
			var content = new MainToolbarContent(name, icon, "Open scene");

			return new MainToolbarDropdown(content, ShowMenu) { enabled = !EditorApplication.isPlayingOrWillChangePlaymode };
		}

		private static void Refresh() => MainToolbar.Refresh(PATH);

		private static void ShowMenu(Rect rect)
		{
			var activePath = SceneManager.GetActiveScene().path;
			var menu = new GenericMenu();

			foreach (var scene in EditorBuildSettings.scenes.Where(scene => scene.enabled))
			{
				AddItem(menu, Path.GetFileNameWithoutExtension(scene.path), scene.path, activePath);
			}

			menu.AddSeparator(string.Empty);

			var allScenes = AssetDatabase.FindAssets("t:SceneAsset", new[] { "Assets" })
				.Select(AssetDatabase.GUIDToAssetPath)
				.OrderBy(path => path);

			foreach (var path in allScenes)
			{
				var label = ALL_SCENES + Path.ChangeExtension(path["Assets/".Length..], null);
				AddItem(menu, label, path, activePath);
			}

			menu.DropDown(rect);
		}

		private static void AddItem(GenericMenu menu, string label, string path, string activePath) =>
			menu.AddItem(new GUIContent(label), path == activePath, () => Open(path));

		private static void Open(string path)
		{
			if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
			{
				EditorSceneManager.OpenScene(path);
			}
		}
	}
}
