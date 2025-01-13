// SPDX-License-Identifier: Apache-2.0
// © 2023-2025 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Threading;
using System.Threading.Tasks;
using Depra.Loading;
using Depra.Threading;
using UnityEngine.SceneManagement;

namespace Depra.Scenes.Operations
{
	public sealed class SceneLoadOperation : ILoadingOperation
	{
		private readonly ISceneActivation _activation;
		private readonly SceneDefinition _desiredScene;
		private readonly OperationDescription _description;

		public SceneLoadOperation(SceneDefinition desiredScene, OperationDescription description,
			ISceneActivation activation)
		{
			_description = description;
			_desiredScene = desiredScene;
			_activation = desiredScene.ActivateOnLoad ? activation : new EmptySceneActivation();
		}

		OperationDescription ILoadingOperation.Description => _description;

		public async ITask Load(IProgress<float> progress, CancellationToken token)
		{
			progress.Report(0);
			var operation = SceneManager.LoadSceneAsync(_desiredScene.DisplayName, _desiredScene.LoadMode);
			if (operation == null)
			{
				progress.Report(1);
				return;
			}

			_activation.BeforeLoading(operation);
			while (operation.isDone == false)
			{
				progress.Report(operation.progress);
				_activation.OnProgress(operation);

				await Task.Yield();
			}

			progress.Report(1);
			if (_desiredScene.ActivateOnLoad)
			{
				ActivateScene();
			}
		}

		private void ActivateScene()
		{
			var loadedScene = _desiredScene.Handle;
			if (loadedScene.IsValid())
			{
				SceneManager.SetActiveScene(loadedScene);
			}
		}
	}
}