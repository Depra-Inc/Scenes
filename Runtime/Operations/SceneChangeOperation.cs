// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Threading;
using System.Threading.Tasks;
using Depra.Expectation;
using Depra.Loading.Operations;
using Depra.Scenes.Activation;
using Depra.Scenes.Definitions;
using Depra.Scenes.Exceptions;

namespace Depra.Scenes.Operations
{
	public sealed class SceneChangeOperation : ILoadingOperation
	{
		private readonly IExpectant _finishExpectant;
		private readonly ISceneActivation _activation;
		private readonly SceneDefinition _desiredScene;
		private readonly SceneDefinition _previousScene;
		private readonly OperationDescription _description;

		public SceneChangeOperation(SceneDefinition from, SceneDefinition to,
			OperationDescription description, ISceneActivation activation, IExpectant finishExpectant = null)
		{
			_desiredScene = to;
			_previousScene = from;
			_activation = activation;
			_description = description;
			_finishExpectant = finishExpectant;
		}

		OperationDescription ILoadingOperation.Description => _description;

		public async Task Load(IProgress<float> progress, CancellationToken token)
		{
			if (_desiredScene.IsActive())
			{
				throw new UnexpectedSceneSwitch(_desiredScene.DisplayName);
			}

			await new SceneLoadOperation(_desiredScene, _description, _activation, _finishExpectant)
				.Load(progress, token);
			await new SceneUnloadOperation(_previousScene, _description)
				.Load(progress, token);
		}
	}
}