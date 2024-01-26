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
	public sealed class SceneUnloadingOperation : ILoadingOperation
	{
		private readonly SceneDefinition _sceneDefinition;

		public SceneUnloadingOperation(SceneDefinition sceneDefinition, OperationDescription description)
		{
			Description = description;
			_sceneDefinition = sceneDefinition;
		}

		public OperationDescription Description { get; }

		async Task ILoadingOperation.Load(Action<float> onProgress, CancellationToken token)
		{
			onProgress?.Invoke(0);
			var operation = SceneManager.UnloadSceneAsync(_sceneDefinition.Name);
			operation.allowSceneActivation = true;

			while (operation.isDone == false)
			{
				onProgress?.Invoke(operation.progress);
				await Task.Yield();
			}

			onProgress?.Invoke(1);
		}
	}
}