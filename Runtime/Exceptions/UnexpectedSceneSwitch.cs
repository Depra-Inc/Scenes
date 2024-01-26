// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;

namespace Depra.Scenes.Exceptions
{
	internal sealed class UnexpectedSceneSwitch : Exception
	{
		public UnexpectedSceneSwitch(string sceneName) : base($"Unexpected switch to same scene '{sceneName}'!") { }
	}
}