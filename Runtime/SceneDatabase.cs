// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.Collections.Generic;
using UnityEngine;
using static Depra.Scenes.Module;

namespace Depra.Scenes
{
	[CreateAssetMenu(fileName = FILE_NAME, menuName = MENU_PATH + FILE_NAME, order = DEFAULT_ORDER)]
	public sealed class SceneDatabase : ScriptableObject
	{
		[SerializeField] private SceneDefinition[] _scenes;

		private const string FILE_NAME = nameof(SceneDatabase);

		public IEnumerable<SceneDefinition> Scenes => _scenes;
	}
}