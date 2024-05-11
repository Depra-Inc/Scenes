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

		public SceneChangeOperation(SceneDefinition desiredScene)
		{
			_desiredScene = desiredScene;
			Description = OperationDescription.Default(_desiredScene.Name);
		}

		public OperationDescription Description { get; }

		public async Task Load(ProgressCallback onProgress, CancellationToken token)
		{
			if (_desiredScene.IsActive())
			{
				throw new UnexpectedSceneSwitch(_desiredScene.Name);
			}

			await new SceneLoadOperation(_desiredScene, Description).Load(onProgress, token);
		}
	}
}