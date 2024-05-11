// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.Threading;
using System.Threading.Tasks;
using Depra.Loading.Operations;
using Depra.Scenes.Definitions;
using Depra.Scenes.Exceptions;

namespace Depra.Scenes.Operations
{
	public sealed class SceneChangeOperation : ILoadingOperation
	{
		private readonly SceneDefinition _desiredScene;
		private readonly SceneDefinition _previousScene;
		private readonly OperationDescription _description;

		public SceneChangeOperation(SceneDefinition from, SceneDefinition to)
		{
			_desiredScene = to;
			_previousScene = from;
			_description = OperationDescription.Default(_desiredScene.DisplayName);
		}

		OperationDescription ILoadingOperation.Description => _description;

		public async Task Load(ProgressCallback onProgress, CancellationToken token)
		{
			if (_desiredScene.IsActive())
			{
				throw new UnexpectedSceneSwitch(_desiredScene.DisplayName);
			}

			await new SceneLoadOperation(_desiredScene, _description).Load(onProgress, token);
			await new SceneUnloadOperation(_previousScene, _description).Load(onProgress, token);
		}
	}
}