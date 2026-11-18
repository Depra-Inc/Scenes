// SPDX-License-Identifier: Apache-2.0
// © 2023-2026 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Threading;
using Depra.Loading;
using Depra.Threading;

namespace Depra.Scenes.Operations
{
	public sealed class SceneUnloadOperation : ILoadingOperation
	{
		private readonly SceneReference _scene;
		private readonly OperationDescription _description;

		public SceneUnloadOperation(SceneReference scene, OperationDescription description)
		{
			_scene = scene;
			_description = description;
		}

		OperationDescription ILoadingOperation.Description => _description;

		public ITask Load(IProgress<float> progress, CancellationToken token) =>
			SceneLoading.Unload(_scene, progress);
	}
}