// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.Threading;
using System.Threading.Tasks;
using Depra.Expectation;
using Depra.Loading.Operations;
using Depra.Scenes.Definitions;
using UnityEngine.SceneManagement;
using AsyncOperation = UnityEngine.AsyncOperation;

namespace Depra.Scenes.Operations
{
	public sealed class SceneLoadOperation : ILoadingOperation
	{
		private readonly SceneDefinition _desiredScene;
		private readonly OperationDescription _description;
		private readonly IExpectant _externalActivationExpectant;

		private AsyncOperation _operation;
		private Expectant _loadingExpectant;
		private IExpectant _activationExpectant;

		public SceneLoadOperation(SceneDefinition desiredScene, OperationDescription description,
			IExpectant activationExpectant = null)
		{
			_description = description;
			_desiredScene = desiredScene;
			_externalActivationExpectant = activationExpectant;
		}

		OperationDescription ILoadingOperation.Description => _description;

		public async Task Load(ProgressCallback onProgress, CancellationToken token)
		{
			onProgress?.Invoke(0);
			if (_desiredScene.ActivateOnLoad)
			{
				SetupActivation();
			}

			_operation = SceneManager.LoadSceneAsync(_desiredScene.DisplayName, _desiredScene.LoadMode);
			if (_operation == null)
			{
				onProgress?.Invoke(1);
				return;
			}

			_operation.allowSceneActivation = false;
			while (_operation.isDone == false)
			{
				onProgress?.Invoke(_operation.progress);
				await Task.Yield();
			}

			onProgress?.Invoke(1);
		}

		private void SetupActivation()
		{
			_loadingExpectant = new Expectant();
			_activationExpectant = new GroupExpectant.And()
				.With(_loadingExpectant)
				.With(_externalActivationExpectant)
				.Build();

			_activationExpectant.Subscribe(Activate);
			SceneManager.sceneLoaded += OnSceneLoaded;
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
			_operation.allowSceneActivation = true;
			var scene = SceneManager.GetSceneByName(_desiredScene.DisplayName);
			SceneManager.SetActiveScene(scene);
			Dispose();
		}

		private void Dispose()
		{
			_operation = null;
			_loadingExpectant?.Dispose();
			_activationExpectant?.Dispose();
		}
	}
}