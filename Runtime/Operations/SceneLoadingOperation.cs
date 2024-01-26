// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Runtime.CompilerServices;
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ActivateIfNeeded(SceneDefinition scene)
		{
			if (scene.ActivateOnLoad)
			{
				SceneManager.SetActiveScene(scene.Handle);
			}
		}

		async Task ILoadingOperation.Load(Action<float> onProgress, CancellationToken token)
		{
			onProgress?.Invoke(0);
			var operation = SceneManager.LoadSceneAsync(_sceneDefinition.Name, _sceneDefinition.LoadMode);
			operation.allowSceneActivation = true;

			while (operation.isDone == false)
			{
				onProgress?.Invoke(operation.progress);
				await Task.Yield();
			}

			onProgress?.Invoke(1);
			ActivateIfNeeded(_sceneDefinition);
		}
	}
}