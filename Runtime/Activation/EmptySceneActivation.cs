// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using UnityEngine;

namespace Depra.Scenes.Activation
{
	public sealed class EmptySceneActivation : ISceneActivation
	{
		void ISceneActivation.BeforeLoading(AsyncOperation operation) { }

		void ISceneActivation.OnProgress(AsyncOperation operation) { }
	}
}