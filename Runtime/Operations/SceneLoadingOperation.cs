// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Threading;
using System.Threading.Tasks;
using Depra.Loading.Operations;
using Depra.Scenes.Definitions;
using UnityEngine.SceneManagement;

namespace Depra.Scenes.Operations
{
	public sealed class SceneLoadingOperation : ILoadingOperation
	{
		private readonly SceneDefinition _sceneDefinition;

		public SceneLoadingOperation(SceneDefinition sceneDefinition, OperationDescription description)
		{
			Description = description;
			_sceneDefinition = sceneDefinition;
		}

		public OperationDescription Description { get; }

		async Task ILoadingOperation.Load(Action<float> onProgress, CancellationToken token)
		{
			onProgress?.Invoke(0);
			SceneManager.sceneLoaded += OnSceneLoaded;
			var operation = SceneManager.LoadSceneAsync(_sceneDefinition.Name, _sceneDefinition.LoadMode);

			while (operation.isDone == false)
			{
				onProgress?.Invoke(operation.progress);
				await Task.Yield();
			}

			onProgress?.Invoke(1);
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			SceneManager.sceneLoaded -= OnSceneLoaded;
			if (_sceneDefinition.ActivateOnLoad && scene.IsValid())
			{
				SceneManager.SetActiveScene(scene);
			}
		}
	}
}