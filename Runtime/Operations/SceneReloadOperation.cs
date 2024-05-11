// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.Threading;
using System.Threading.Tasks;
using Depra.Loading.Operations;
using Depra.Scenes.Definitions;
using Depra.Scenes.Operations;

namespace Depra.Scenes.Change
{
	public sealed class SceneReloadOperation : ILoadingOperation
	{
		private readonly SceneDefinition _activeScene;
		private readonly OperationDescription _description;

		public SceneReloadOperation(SceneDatabase scenes)
		{
			_activeScene = scenes.Active;
			_description = OperationDescription.Default(_activeScene.DisplayName);
		}

		OperationDescription ILoadingOperation.Description => _description;

		public async Task Load(ProgressCallback onProgress, CancellationToken token)
		{
			await new SceneUnloadOperation(_activeScene, _description).Load(onProgress, token);
			await new SceneLoadOperation(_activeScene, _description).Load(onProgress, token);
		}
	}
}