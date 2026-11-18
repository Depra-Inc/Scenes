// SPDX-License-Identifier: Apache-2.0
// © 2023-2026 Depra <n.melnikov@depra.org>

using System;
using System.Threading;
using Depra.Loading;
using Depra.Threading;

namespace Depra.Scenes.Operations
{
	public sealed class SceneChangeOperation : ILoadingOperation
	{
		private readonly SceneReference _to;
		private readonly SceneReference _from;
		private readonly ISceneActivation _activation;
		private readonly OperationDescription _description;

		public SceneChangeOperation(SceneReference from, SceneReference to, OperationDescription description,
			ISceneActivation activation = null)
		{
			_to = to;
			_from = from;
			_description = description;
			_activation = activation ?? new EmptySceneActivation();
		}

		OperationDescription ILoadingOperation.Description => _description;

		public async ITask Load(IProgress<float> progress, CancellationToken token)
		{
			if (_from == _to)
			{
				throw new UnexpectedSceneSwitch(_to.Name);
			}

			await SceneLoading.Load(_to, _activation, new ProgressRange(progress, 0f, 0.5f));
			SceneLoading.Activate(_to);
			await SceneLoading.Unload(_from, new ProgressRange(progress, 0.5f, 1f));
		}
	}
}