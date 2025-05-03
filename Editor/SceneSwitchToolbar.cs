// SPDX-License-Identifier: Apache-2.0
// © 2025 Depra <n.melnikov@depra.org>

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Depra.Scenes.Editor
{
	/// <summary>
	/// A custom toolbar for switching between scenes in the Unity Editor.
	/// It allows users to select and load scenes from a dropdown menu.
	/// </summary>
	/// <remarks>
	/// This class is initialized when the Unity Editor loads and listens for scene changes and play mode state changes.
	/// </remarks>
	[InitializeOnLoad]
	internal static class SceneSwitchToolbar
	{
		private const float DROPDOWN_BOX_HEIGHT = 20f;

		private static int _selectedIndex;
		private static string _lastActiveScene = "";
		private static string[] _sceneNames = Array.Empty<string>();
		private static VisualElement _toolbarUI;

		private static bool FetchAllScenesSetting
		{
			get => EditorPrefs.GetBool("SceneSwitch_FetchAllScenes", false);
			set => EditorPrefs.SetBool("SceneSwitch_FetchAllScenes", value);
		}

		static SceneSwitchToolbar()
		{
			RefreshSceneList();
			SelectCurrentScene();

			EditorSceneManager.activeSceneChangedInEditMode += UpdateSceneSelection;
			EditorApplication.playModeStateChanged += OnPlayModeChanged;
			EditorApplication.delayCall += AddToolbarUI;
		}

		private static void AddToolbarUI()
		{
			var toolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");
			if (toolbarType == null)
			{
				return;
			}

			var toolbars = Resources.FindObjectsOfTypeAll(toolbarType);
			if (toolbars.Length == 0)
			{
				return;
			}

			var toolbar = toolbars[0];
			var rootField = toolbarType.GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
			if (rootField == null)
			{
				return;
			}

			if (rootField.GetValue(toolbar) is not VisualElement root)
			{
				return;
			}

			var leftContainer = root.Q("ToolbarZoneLeftAlign");
			if (leftContainer == null)
			{
				return;
			}

			if (_toolbarUI != null)
			{
				leftContainer.Remove(_toolbarUI);
			}

			_toolbarUI = new IMGUIContainer(OnGUI);
			leftContainer.Add(_toolbarUI);
		}

		private static void OnGUI()
		{
			CheckAndRefreshScenes();

			if (_selectedIndex >= _sceneNames.Length)
			{
				_selectedIndex = 0;
			}

			var isPlaying = EditorApplication.isPlaying;

			GUILayout.BeginHorizontal();
			EditorGUI.BeginDisabledGroup(isPlaying);

			var newFetchAllScenes = GUILayout.Toggle(FetchAllScenesSetting, "All Scenes", "Button",
				GUILayout.Height(DROPDOWN_BOX_HEIGHT));
			if (newFetchAllScenes != FetchAllScenesSetting)
			{
				FetchAllScenesSetting = newFetchAllScenes;
				RefreshSceneList();
				SelectCurrentScene();
			}

			EditorGUI.EndDisabledGroup();
			EditorGUI.BeginDisabledGroup(isPlaying);
			var popupStyle = new GUIStyle(EditorStyles.popup) { fixedHeight = DROPDOWN_BOX_HEIGHT };
			var newIndex = EditorGUILayout.Popup(_selectedIndex, _sceneNames, popupStyle,
				GUILayout.Width(150), GUILayout.Height(DROPDOWN_BOX_HEIGHT));

			if (newIndex != _selectedIndex)
			{
				_selectedIndex = newIndex;
				LoadScene(_sceneNames[_selectedIndex]);
			}

			EditorGUI.EndDisabledGroup();
			GUILayout.EndHorizontal();
		}

		private static void RefreshSceneList()
		{
			if (FetchAllScenesSetting)
			{
				_sceneNames = Directory.GetFiles("Assets", "*.unity", SearchOption.AllDirectories)
					.Select(Path.GetFileNameWithoutExtension)
					.ToArray();
			}
			else
			{
				_sceneNames = EditorBuildSettings.scenes
					.Where(scene => scene.enabled)
					.Select(scene => Path.GetFileNameWithoutExtension(scene.path))
					.ToArray();
			}
		}

		private static void CheckAndRefreshScenes()
		{
			var currentScenes = FetchAllScenesSetting
				? FetchScenesInAllDirectories()
				: FetchScenesInBuildSettings();

			if (!currentScenes.SequenceEqual(_sceneNames))
			{
				_sceneNames = currentScenes;
				SelectCurrentScene();
			}
		}

		private static void SelectCurrentScene()
		{
			var currentScene = Path.GetFileNameWithoutExtension(SceneManager.GetActiveScene().path);
			var index = Array.IndexOf(_sceneNames, currentScene);
			if (index != -1)
			{
				_selectedIndex = index;
				_lastActiveScene = currentScene;
			}
		}

		private static void UpdateSceneSelection(Scene previous, Scene next)
		{
			var currentScene = Path.GetFileNameWithoutExtension(SceneManager.GetActiveScene().path);
			if (currentScene != _lastActiveScene)
			{
				_lastActiveScene = currentScene;
				SelectCurrentScene();
			}
		}

		private static void LoadScene(string sceneName)
		{
			string scenePath;
			if (FetchAllScenesSetting)
			{
				scenePath = Directory.GetFiles("Assets", "*.unity", SearchOption.AllDirectories)
					.FirstOrDefault(path => Path.GetFileNameWithoutExtension(path) == sceneName);
			}
			else
			{
				scenePath = EditorBuildSettings.scenes
					.FirstOrDefault(scene => scene.enabled && scene.path.Contains(sceneName))?.path;
			}

			if (!string.IsNullOrEmpty(scenePath))
			{
				if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
				{
					EditorSceneManager.OpenScene(scenePath);
				}
			}
			else
			{
				Debug.LogError("Scene not found: " + sceneName);
			}
		}

		private static string[] FetchScenesInAllDirectories() => Directory
			.GetFiles("Assets", "*.unity", SearchOption.AllDirectories)
			.Select(Path.GetFileNameWithoutExtension)
			.ToArray();

		private static string[] FetchScenesInBuildSettings() => EditorBuildSettings.scenes
			.Where(scene => scene.enabled)
			.Select(scene => Path.GetFileNameWithoutExtension(scene.path))
			.ToArray();

		private static void OnPlayModeChanged(PlayModeStateChange state)
		{
			if (state is PlayModeStateChange.EnteredPlayMode or PlayModeStateChange.ExitingPlayMode)
			{
				EditorApplication.delayCall += AddToolbarUI;
			}
		}
	}
}