﻿// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Depra.Loading.Operations;

namespace Depra.Scenes
{
	public interface ISceneChange
	{
		bool IsActive(SceneDefinition scene);

		void Switch(SceneDefinition scene);

		Task Reload(IEnumerable<ILoadingOperation> addOperations, CancellationToken token);

		Task SwitchAsync(SceneDefinition scene, IEnumerable<ILoadingOperation> addOperations, CancellationToken token);
	}
}