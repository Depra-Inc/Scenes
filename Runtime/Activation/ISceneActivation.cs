// SPDX-License-Identifier: Apache-2.0
// © 2023-2025 Nikolay Melnikov <n.melnikov@depra.org>

using UnityEngine;

namespace Depra.Scenes
{
	public interface ISceneActivation
	{
		void BeforeLoading(AsyncOperation operation);

		void OnProgress(AsyncOperation operation);
	}
}