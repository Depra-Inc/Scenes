// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Depra.Loading.Curtain;
using Depra.Loading.Operations;
using Depra.Scenes.Definitions;
using Depra.Scenes.Exceptions;
using Depra.Scenes.Operations;
using UnityEngine.SceneManagement;

namespace Depra.Scenes.Change
{
	public sealed class SceneChange : ISceneChange
	{
		private readonly ILoadingCurtain _loadingCurtain;

		public SceneChange(SceneDatabase scenes, ILoadingCurtain loadingCurtain) : this(
			scenes.Find(SceneManager.GetActiveScene().name), loadingCurtain) { }

		public SceneChange(SceneDefinition initialScene, ILoadingCurtain loadingCurtain)
		{
			ActiveScene = initialScene;
			_loadingCurtain = loadingCurtain;
		}

		public SceneDefinition ActiveScene { get; private set; }

		public bool IsActive(SceneDefinition scene) =>
			scene == ActiveScene || scene.Name == SceneManager.GetActiveScene().name;

		public Task Load(SceneDefinition scene, CancellationToken token, params ILoadingOperation[] addOperations) =>
			IsActive(scene)
				? throw new UnexpectedSceneSwitch(scene.Name)
				: LoadInternal(ActiveScene = scene, token, addOperations);

		public async Task Unload(SceneDefinition scene, CancellationToken token)
		{
			var operations = new[] { new SceneUnloadingOperation(scene, OperationDescription.Default(scene.Name)) };
			await new CleanLoadingCurtain().Load(operations, token);
		}

		public async Task Reload(CancellationToken token, params ILoadingOperation[] addOperations)
		{
			await Unload(ActiveScene, token);
			await LoadInternal(ActiveScene, token, addOperations);
		}

		private async Task LoadInternal(SceneDefinition scene, CancellationToken token,
			params ILoadingOperation[] addOperations)
		{
			addOperations ??= Array.Empty<ILoadingOperation>();
			var operations = addOperations.Concat(new[]
				{ new SceneLoadingOperation(scene, OperationDescription.Default(scene.Name)) });

			await _loadingCurtain.Load(operations, token);
			_loadingCurtain.Unload();
		}
	}
}