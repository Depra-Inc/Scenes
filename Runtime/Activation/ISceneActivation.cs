// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using UnityEngine;

namespace Depra.Scenes.Activation
{
	public interface ISceneActivation
	{
		void BeforeLoading(AsyncOperation operation);

		void OnProgress(AsyncOperation operation);

		void AfterLoading();
	}
}