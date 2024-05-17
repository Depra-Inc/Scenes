﻿// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.Threading;
using System.Threading.Tasks;
using Depra.Loading.Operations;
using Depra.Scenes.Activation;
using Depra.Scenes.Definitions;
using Depra.Scenes.Operations;

namespace Depra.Scenes.Change
{
	public sealed class SceneReloadOperation : ILoadingOperation
	{
		private readonly SceneDefinition _activeScene;
		private readonly OperationDescription _description;
		private readonly ISceneActivation _activation;

		public SceneReloadOperation(SceneDatabase scenes, ISceneActivation activation)
		{
			_activeScene = scenes.Active;
			_activation = activation;
			_description = OperationDescription.Default(_activeScene.DisplayName);
		}

		OperationDescription ILoadingOperation.Description => _description;

		public async Task Load(ProgressCallback onProgress, CancellationToken token)
		{
			await new SceneUnloadOperation(_activeScene, _description).Load(onProgress, token);
			await new SceneLoadOperation(_activeScene, _description, _activation).Load(onProgress, token);
		}
	}
}