// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Depra.Loading.Curtain;
using Depra.Loading.Operations;
using UnityEngine.SceneManagement;

namespace Depra.Scenes
{
	public sealed class SceneChange : ISceneChange
	{
		private readonly ILoadingCurtain _loadingCurtain;
		private SceneDefinition _currentScene;

		public SceneChange(ILoadingCurtain loadingCurtain) : this(
			new SceneDefinition(SceneManager.GetActiveScene().name, LoadSceneMode.Single), loadingCurtain) { }

		public SceneChange(SceneDefinition initialScene, ILoadingCurtain loadingCurtain)
		{
			_currentScene = initialScene;
			_loadingCurtain = loadingCurtain;
		}

		private async Task SwitchAsyncInternal(SceneDefinition scene, IEnumerable<ILoadingOperation> addOperations,
			CancellationToken token)
		{
			var operations = addOperations.Concat(new[]
				{ new SceneLoadingOperation(scene, OperationDescription.Default(scene.Name)) });

			await _loadingCurtain.Load(operations, token);
			_loadingCurtain.Unload();
		}

		public bool IsActive(SceneDefinition scene) =>
			scene == _currentScene || scene.Name == SceneManager.GetActiveScene().name;

		void ISceneChange.Switch(SceneDefinition scene)
		{
			if (IsActive(scene))
			{
				throw new UnexpectedSceneSwitch(scene.Name);
			}

			_currentScene = scene;
			SceneManager.LoadScene(_currentScene.Name, _currentScene.LoadMode);
		}

		Task ISceneChange.SwitchAsync(SceneDefinition scene, IEnumerable<ILoadingOperation> addOperations,
			CancellationToken token) => IsActive(scene)
			? throw new UnexpectedSceneSwitch(scene.Name)
			: SwitchAsyncInternal(_currentScene = scene, addOperations, token);

		Task ISceneChange.Reload(IEnumerable<ILoadingOperation> addOperations, CancellationToken token) =>
			SwitchAsyncInternal(_currentScene, addOperations, token);
	}
}