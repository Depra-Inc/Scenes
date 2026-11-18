// SPDX-License-Identifier: Apache-2.0
// © 2023-2026 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Threading;
using Depra.Loading;
using Depra.Threading;

namespace Depra.Scenes.Operations
{
	public sealed class SceneReloadOperation : ILoadingOperation
	{
		private readonly SceneReference _scene;
		private readonly ISceneActivation _activation;
		private readonly OperationDescription _description;

		public SceneReloadOperation(SceneReference scene, OperationDescription description,
			ISceneActivation activation = null)
		{
			_scene = scene;
			_description = description;
			_activation = activation ?? new EmptySceneActivation();
		}

		OperationDescription ILoadingOperation.Description => _description;

		public async ITask Load(IProgress<float> progress, CancellationToken token)
		{
			await SceneLoading.Unload(_scene, new ProgressRange(progress, 0f, 0.5f));
			await SceneLoading.Load(_scene, _activation, new ProgressRange(progress, 0.5f, 1f));
			SceneLoading.Activate(_scene);
		}
	}
}