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
			var nextScene = _sceneDefinition.Handle;
			var operation = SceneManager.LoadSceneAsync(nextScene.handle, _sceneDefinition.LoadMode);
			operation.allowSceneActivation = true;

			while (nextScene.isLoaded == false)
			{
				onProgress?.Invoke(operation.progress);
				await Task.Yield();
			}

			onProgress?.Invoke(1);
			if (_sceneDefinition.ActivateOnLoad && nextScene.IsValid())
			{
				SceneManager.SetActiveScene(nextScene);
			}
		}
	}
}