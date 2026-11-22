// SPDX-License-Identifier: Apache-2.0
// © 2026 Depra <n.melnikov@depra.org>

using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Depra.Scenes.Editor
{
	internal enum SceneStatus
	{
		NONE,
		OK,
		MISSING,
		NOT_IN_BUILD,
		DISABLED_IN_BUILD
	}

	internal static class SceneReferenceEditor
	{
		public static string ResolvePath(Hash128 guid) => guid.isValid
			? AssetDatabase.GUIDToAssetPath(guid.ToString())
			: string.Empty;

		public static SceneStatus GetStatus(Hash128 guid)
		{
			if (!guid.isValid)
			{
				return SceneStatus.NONE;
			}

			var path = ResolvePath(guid);
			if (string.IsNullOrEmpty(path) || AssetDatabase.LoadAssetAtPath<SceneAsset>(path) == null)
			{
				return SceneStatus.MISSING;
			}

			var guidString = guid.ToString();
			var entry = EditorBuildSettings.scenes.FirstOrDefault(scene => scene.guid.ToString() == guidString);
			if (entry == null)
			{
				return SceneStatus.NOT_IN_BUILD;
			}

			return entry.enabled ? SceneStatus.OK : SceneStatus.DISABLED_IN_BUILD;
		}

		public static void EnableInBuild(string path)
		{
			var scenes = EditorBuildSettings.scenes;
			var index = Array.FindIndex(scenes, scene => scene.path == path);
			if (index >= 0)
			{
				scenes[index].enabled = true;
			}
			else
			{
				ArrayUtility.Add(ref scenes, new EditorBuildSettingsScene(path, true));
			}

			EditorBuildSettings.scenes = scenes;
		}
	}
}
