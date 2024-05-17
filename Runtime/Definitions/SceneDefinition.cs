// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using Depra.Inspector.Attributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Depra.Scenes.Module;

namespace Depra.Scenes.Definitions
{
	public sealed partial class SceneDefinition : ScriptableObject
	{
		[Scene] [SerializeField] private string _displayName;
		[field: SerializeField] public LoadSceneMode LoadMode { get; private set; }
		[field: SerializeField] internal bool CanBeUnloaded { get; private set; } = true;
		[field: SerializeField] internal bool ActivateOnLoad { get; private set; } = true;

		public int Index => Handle.buildIndex;
		public string DisplayName => _displayName;
		internal Scene Handle => SceneManager.GetSceneByName(_displayName);

		public bool IsActive() => Handle.name == DisplayName;
	}

	[CreateAssetMenu(fileName = FILE_NAME, menuName = MENU_PATH + FILE_NAME, order = DEFAULT_ORDER)]
	public sealed partial class SceneDefinition : IEquatable<SceneDefinition>
	{
		private const string FILE_NAME = nameof(SceneDefinition);

		public static bool operator ==(SceneDefinition a, SceneDefinition b) => a?.Equals(b) ?? b is null;

		public static bool operator !=(SceneDefinition a, SceneDefinition b) => !(a == b);

		public bool Equals(SceneDefinition other) => other?.DisplayName == DisplayName;

		public override bool Equals(object obj) => obj is SceneDefinition other && Equals(other);

		public override int GetHashCode() => DisplayName.GetHashCode();

		public override string ToString() => DisplayName;
	}
}