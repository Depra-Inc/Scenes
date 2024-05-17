// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Threading;
using System.Threading.Tasks;
using Depra.Loading.Operations;
using Depra.Scenes.Definitions;
using UnityEngine.SceneManagement;

namespace Depra.Scenes.Operations
{
	public sealed class SceneUnloadOperation : ILoadingOperation
	{
		private readonly SceneDefinition _scene;
		private readonly OperationDescription _description;

		public SceneUnloadOperation(SceneDefinition scene, OperationDescription description)
		{
			_scene = scene;
			_description = description;
		}

		OperationDescription ILoadingOperation.Description => _description;

		public async Task Load(IProgress<float> progress, CancellationToken token)
		{
			if (_scene.CanBeUnloaded == false)
			{
				return;
			}

			progress.Report(0);
			var operation = SceneManager.UnloadSceneAsync(_scene.DisplayName);
			if (operation == null)
			{
				progress.Report(1);
				return;
			}

			while (operation.isDone == false)
			{
				progress.Report(operation.progress);
				await Task.Yield();
			}

			progress.Report(1);
		}
	}
}