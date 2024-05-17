// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

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
		private readonly IExpectant _activationExpectant;
		private readonly OperationDescription _description;

		private Expectant _loadingExpectant;

		public SceneLoadOperation(SceneDefinition desiredScene, OperationDescription description,
			ISceneActivation activation, IExpectant activationExpectant = null)
		{
			_activation = activation;
			_description = description;
			_desiredScene = desiredScene;
			_activationExpectant = activationExpectant;
		}

		OperationDescription ILoadingOperation.Description => _description;

		public async Task Load(ProgressCallback onProgress, CancellationToken token)
		{
			onProgress?.Invoke(0);
			if (_desiredScene.ActivateOnLoad)
			{
				SetupActivation();
			}

			var operation = SceneManager.LoadSceneAsync(_desiredScene.DisplayName, _desiredScene.LoadMode);
			if (operation == null)
			{
				onProgress?.Invoke(1);
				return;
			}

			_activation.BeforeLoading(operation);
			while (operation.isDone == false)
			{
				onProgress?.Invoke(operation.progress);
				_activation.OnProgress(operation);

				await Task.Yield();
			}

			onProgress?.Invoke(1);
		}

		private void SetupActivation()
		{
			SceneManager.sceneLoaded += OnSceneLoaded;
			new GroupExpectant.And()
				.With(_loadingExpectant = new Expectant())
				.With(_activationExpectant)
				.Build()
				.Subscribe(Activate);
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			SceneManager.sceneLoaded -= OnSceneLoaded;
			if (scene.IsValid())
			{
				_loadingExpectant?.SetReady();
			}
		}

		private void Activate()
		{
			var scene = SceneManager.GetSceneByName(_desiredScene.DisplayName);
			SceneManager.SetActiveScene(scene);
			Dispose();
		}

		private void Dispose()
		{
			_loadingExpectant?.Dispose();
			_activationExpectant?.Dispose();
		}
	}
}