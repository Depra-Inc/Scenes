// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.Threading;
using System.Threading.Tasks;
using Depra.Loading.Operations;
using Depra.Scenes.Definitions;
using Depra.Scenes.Operations;
using UnityEngine.SceneManagement;

namespace Depra.Scenes.Change
{
	public sealed class SceneReloadOperation : ILoadingOperation
	{
		private readonly SceneDefinition _activeScene;

		public SceneReloadOperation(SceneDatabase scenes)
		{
			_activeScene = scenes.Find(SceneManager.GetActiveScene().name);
			Description = OperationDescription.Default(_activeScene.Name);
		}

		public OperationDescription Description { get; }

		public async Task Load(ProgressCallback onProgress, CancellationToken token)
		{
			await new SceneUnloadOperation(_activeScene, Description).Load(onProgress, token);
			await new SceneLoadOperation(_activeScene, Description).Load(onProgress, token);
		}
	}
}