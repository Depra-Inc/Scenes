// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Depra.Scenes.Module;

namespace Depra.Scenes.Definitions
{
	[CreateAssetMenu(fileName = FILE_NAME, menuName = MENU_PATH + FILE_NAME, order = DEFAULT_ORDER)]
	public sealed class SceneDatabase : ScriptableObject
	{
		[SerializeField] private SceneDefinition[] _scenes;

		private const string FILE_NAME = nameof(SceneDatabase);

		public IEnumerable<SceneDefinition> All => _scenes;

		[ContextMenu(nameof(RemoveDuplicates))]
		public void RemoveDuplicates() => _scenes = _scenes.Distinct().ToArray();

		public SceneDefinition Find(string sceneName) => Array.Find(_scenes, x => x.Name == sceneName);
	}
}