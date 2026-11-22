// SPDX-License-Identifier: Apache-2.0
// © 2023-2026 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Threading.Tasks;
using Depra.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Depra.Scenes.Operations
{
	internal static class SceneLoading
	{
		private const float ACTIVATION_THRESHOLD = 0.9f;

		public static async ITask Load(SceneReference scene, ISceneActivation activation, IProgress<float> progress)
		{
			if (!scene.IsValid)
			{
				throw new ArgumentException("Scene is not assigned.", nameof(scene));
			}

			progress?.Report(0);
			var operation = SceneManager.LoadSceneAsync(scene.Path, LoadSceneMode.Additive)
				?? throw new InvalidOperationException($"Scene '{scene.Path}' could not be loaded.");

			activation.BeforeLoading(operation);
			while (operation.isDone == false)
			{
				progress?.Report(Mathf.Clamp01(operation.progress / ACTIVATION_THRESHOLD));
				activation.OnProgress(operation);

				await Task.Yield();
			}

			progress?.Report(1);
		}

		public static void Activate(SceneReference scene)
		{
			var handle = scene.Handle;
			if (handle.IsValid())
			{
				SceneManager.SetActiveScene(handle);
			}
		}

		public static async ITask Unload(SceneReference scene, IProgress<float> progress)
		{
			progress?.Report(0);
			var handle = scene.Handle;
			if (!handle.IsValid() || !handle.isLoaded)
			{
				progress?.Report(1);
				return;
			}

			var operation = SceneManager.UnloadSceneAsync(handle)
				?? throw new InvalidOperationException($"Scene '{scene.Path}' could not be unloaded.");

			while (operation.isDone == false)
			{
				progress?.Report(operation.progress);
				await Task.Yield();
			}

			progress?.Report(1);
		}
	}
}
