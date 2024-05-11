// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.Threading;
using System.Threading.Tasks;
using Depra.Loading.Operations;
using Depra.Scenes.Definitions;
using UnityEngine.SceneManagement;

namespace Depra.Scenes.Operations
{
	public sealed class SceneLoadOperation : ILoadingOperation
	{
		private readonly SceneDefinition _desiredScene;
		private readonly OperationDescription _description;

		public SceneLoadOperation(SceneDefinition desiredScene, OperationDescription description)
		{
			_description = description;
			_desiredScene = desiredScene;
		}

		OperationDescription ILoadingOperation.Description => _description;

		public async Task Load(ProgressCallback onProgress, CancellationToken token)
		{
			onProgress?.Invoke(0);
			SceneManager.sceneLoaded += OnSceneLoaded;
			var operation = SceneManager.LoadSceneAsync(_desiredScene.DisplayName, _desiredScene.LoadMode);
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

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			SceneManager.sceneLoaded -= OnSceneLoaded;
			if (_desiredScene.ActivateOnLoad && scene.IsValid())
			{
				SceneManager.SetActiveScene(scene);
			}
		}
	}
}