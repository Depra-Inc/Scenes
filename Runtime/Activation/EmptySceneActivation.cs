// SPDX-License-Identifier: Apache-2.0
// © 2023-2025 Nikolay Melnikov <n.melnikov@depra.org>

using UnityEngine;

namespace Depra.Scenes
{
	public sealed class EmptySceneActivation : ISceneActivation
	{
		void ISceneActivation.BeforeLoading(AsyncOperation operation) { }

		void ISceneActivation.OnProgress(AsyncOperation operation) { }
	}
}