// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using UnityEngine;

namespace Depra.Scenes.Activation
{
	public sealed class PercentageSceneActivation : ISceneActivation
	{
		private readonly float _desiredProgress;

		public PercentageSceneActivation(float desiredProgress) =>
			_desiredProgress = desiredProgress;

		void ISceneActivation.BeforeLoading(AsyncOperation operation) =>
			operation.allowSceneActivation = false;

		void ISceneActivation.OnProgress(AsyncOperation operation)
		{
			if (operation.progress >= _desiredProgress)
			{
				operation.allowSceneActivation = true;
			}
		}

		void ISceneActivation.AfterLoading() { }
	}
}