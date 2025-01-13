// SPDX-License-Identifier: Apache-2.0
// © 2023-2025 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Threading;
using Depra.Loading;
using Depra.Threading;

namespace Depra.Scenes.Operations
{
	public sealed class SceneChangeOperation : ILoadingOperation
	{
		private readonly ISceneActivation _activation;
		private readonly SceneDefinition _desiredScene;
		private readonly SceneDefinition _previousScene;
		private readonly OperationDescription _description;

		public SceneChangeOperation(SceneDefinition from, SceneDefinition to,
			OperationDescription description, ISceneActivation activation)
		{
			_desiredScene = to;
			_previousScene = from;
			_activation = activation;
			_description = description;
		}

		OperationDescription ILoadingOperation.Description => _description;

		public async ITask Load(IProgress<float> progress, CancellationToken token)
		{
			if (_desiredScene.IsActive())
			{
				throw new UnexpectedSceneSwitch(_desiredScene.DisplayName);
			}

			await new SceneLoadOperation(_desiredScene, _description, _activation).Load(progress, token);
			await new SceneUnloadOperation(_previousScene, _description).Load(progress, token);
		}
	}
}