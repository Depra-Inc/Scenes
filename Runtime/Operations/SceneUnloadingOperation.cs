// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.Threading;
using System.Threading.Tasks;
using Depra.Loading.Operations;
using Depra.Scenes.Definitions;
using UnityEngine.SceneManagement;

namespace Depra.Scenes.Operations
{
	public sealed class SceneUnloadingOperation : ILoadingOperation
	{
		private readonly SceneDefinition _sceneForUnload;

		public SceneUnloadingOperation(SceneDefinition sceneForUnload, OperationDescription description)
		{
			Description = description;
			_sceneForUnload = sceneForUnload;
		}

		public OperationDescription Description { get; }

		public async Task Load(ProgressCallback onProgress, CancellationToken token)
		{
			onProgress?.Invoke(0);
			var operation = SceneManager.UnloadSceneAsync(_sceneForUnload.Name);
			if (operation == null)
			{
				onProgress?.Invoke(1);
				return;
			}

			while (operation.isDone == false)
			{
				onProgress?.Invoke(operation.progress);
				await Task.Yield();
			}

			onProgress?.Invoke(1);
		}
	}
}