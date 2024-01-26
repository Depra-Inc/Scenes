// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
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

		public bool IsActive(SceneDefinition scene) =>
			scene == ActiveScene || scene.Name == SceneManager.GetActiveScene().name;

		private async Task LoadInternal(SceneDefinition scene, IEnumerable<ILoadingOperation> addOperations,
			CancellationToken token)
		{
			var operations = addOperations.Concat(new[]
				{ new SceneLoadingOperation(scene, OperationDescription.Default(scene.Name)) });

			await _loadingCurtain.Load(operations, token);
			_loadingCurtain.Unload();
		}

		Task ISceneChange.Load(SceneDefinition scene, IEnumerable<ILoadingOperation> addOperations,
			CancellationToken token) => IsActive(scene)
			? throw new UnexpectedSceneSwitch(scene.Name)
			: LoadInternal(ActiveScene = scene, addOperations, token);

		async Task ISceneChange.Unload(SceneDefinition scene, CancellationToken token)
		{
			var operations = new[] { new SceneLoadingOperation(scene, OperationDescription.Default(scene.Name)) };
			ILoadingCurtain cleanCurtain = new CleanLoadingCurtain();
			await cleanCurtain.Load(operations, token);
		}

		Task ISceneChange.Reload(IEnumerable<ILoadingOperation> addOperations, CancellationToken token) =>
			LoadInternal(ActiveScene, addOperations, token);
	}
}