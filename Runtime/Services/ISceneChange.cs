// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Depra.Loading.Operations;
using Depra.Scenes.Definitions;

namespace Depra.Scenes.Services
{
	public interface ISceneChange
	{
		bool IsActive(SceneDefinition scene);

		Task Unload(SceneDefinition scene, CancellationToken token);

		Task Reload(IEnumerable<ILoadingOperation> addOperations, CancellationToken token);

		Task Load(SceneDefinition scene, IEnumerable<ILoadingOperation> addOperations, CancellationToken token);
	}
}