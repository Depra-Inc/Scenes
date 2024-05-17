// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Threading;
using System.Threading.Tasks;
using Depra.Expectation;
using Depra.Loading.Operations;
using Depra.Scenes.Activation;
using Depra.Scenes.Definitions;
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

		public async Task Load(IProgress<float> progress, CancellationToken token)
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
			await AfterLoading();
		}

		private async Task AfterLoading()
		{
			if (_desiredScene.ActivateOnLoad)
			{
				await Task.Yield();
				ActivateScene();
			}
		}

		private void ActivateScene()
		{
			var loadedScene = SceneManager.GetSceneByName(_desiredScene.DisplayName);
			if (loadedScene.IsValid())
			{
				SceneManager.SetActiveScene(loadedScene);
			}
		}
	}
}