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

		public SceneChange(ILoadingCurtain loadingCurtain) : this(
			new SceneDefinition(SceneManager.GetActiveScene().name, LoadSceneMode.Single), loadingCurtain) { }

		public SceneChange(SceneDefinition initialScene, ILoadingCurtain loadingCurtain)
		{
			ActiveScene = initialScene;
			_loadingCurtain = loadingCurtain;
		}

		public SceneDefinition ActiveScene { get; private set; }

		private async Task SwitchAsyncInternal(SceneDefinition scene, IEnumerable<ILoadingOperation> addOperations,
			CancellationToken token)
		{
			var operations = addOperations.Concat(new[]
				{ new SceneLoadingOperation(scene, OperationDescription.Default(scene.Name)) });

			await _loadingCurtain.Load(operations, token);
			_loadingCurtain.Unload();
		}

		public bool IsActive(SceneDefinition scene) =>
			scene == ActiveScene || scene.Name == SceneManager.GetActiveScene().name;

		void ISceneChange.Switch(SceneDefinition scene)
		{
			if (IsActive(scene))
			{
				throw new UnexpectedSceneSwitch(scene.Name);
			}

			ActiveScene = scene;
			SceneManager.LoadScene(ActiveScene.Name, ActiveScene.LoadMode);
		}

		Task ISceneChange.SwitchAsync(SceneDefinition scene, IEnumerable<ILoadingOperation> addOperations,
			CancellationToken token)
		{
			if (IsActive(scene))
			{
				throw new UnexpectedSceneSwitch(scene.Name);
			}

			return SwitchAsyncInternal(ActiveScene = scene, addOperations, token);
		}

		Task ISceneChange.Reload(IEnumerable<ILoadingOperation> addOperations, CancellationToken token) =>
			SwitchAsyncInternal(ActiveScene, addOperations, token);
	}
}