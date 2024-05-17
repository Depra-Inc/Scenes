// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using UnityEngine;

namespace Depra.Scenes.Activation
{
	public sealed class CompositeSceneActivation : ISceneActivation
	{
		private readonly ISceneActivation[] _activations;

		public CompositeSceneActivation(params ISceneActivation[] activations) => _activations = activations;

		void ISceneActivation.BeforeLoading(AsyncOperation operation)
		{
			foreach (var activation in _activations)
			{
				activation.BeforeLoading(operation);
			}
		}

		void ISceneActivation.OnProgress(AsyncOperation operation)
		{
			foreach (var activation in _activations)
			{
				activation.OnProgress(operation);
			}
		}
	}
}