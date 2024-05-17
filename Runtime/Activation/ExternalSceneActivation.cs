// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Expectation;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Depra.Scenes.Activation
{
	public sealed class ExternalSceneActivation : ISceneActivation
	{
		private readonly IExpectant _externalExpectant;

		private Scene _loadedScene;
		private Expectant _loadingExpectant;

		public ExternalSceneActivation(IExpectant expectant) => _externalExpectant = expectant;

		private void Activate()
		{
			SceneManager.SetActiveScene(_loadedScene);
			_loadingExpectant?.Dispose();
			_externalExpectant?.Dispose();
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			_loadedScene = scene;
			SceneManager.sceneLoaded -= OnSceneLoaded;
			if (_loadedScene.IsValid())
			{
				_loadingExpectant?.SetReady();
			}
		}

		void ISceneActivation.BeforeLoading(AsyncOperation operation)
		{
			SceneManager.sceneLoaded += OnSceneLoaded;
			new GroupExpectant.And()
				.With(_externalExpectant)
				.With(_loadingExpectant = new Expectant())
				.Build()
				.Subscribe(Activate);
		}

		void ISceneActivation.OnProgress(AsyncOperation operation) { }
	}
}