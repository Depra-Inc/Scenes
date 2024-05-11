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
		[field: Scene, SerializeField] public string Name { get; private set; }
		[SerializeField] private string _title;
		[field: SerializeField] public LoadSceneMode LoadMode { get; private set; }
		[field: TextArea, SerializeField] public string Description { get; private set; }
		[field: SerializeField] internal bool ActivateOnLoad { get; private set; } = true;

		public string Title => string.IsNullOrEmpty(_title) ? Name : _title;

		public bool IsActive() => SceneManager.GetActiveScene().name == Name;
	}

	[CreateAssetMenu(fileName = FILE_NAME, menuName = MENU_PATH + FILE_NAME, order = DEFAULT_ORDER)]
	public sealed partial class SceneDefinition : IEquatable<SceneDefinition>
	{
		private const string FILE_NAME = nameof(SceneDefinition);

		public static bool operator ==(SceneDefinition a, SceneDefinition b) => a?.Equals(b) ?? b is null;

		public static bool operator !=(SceneDefinition a, SceneDefinition b) => !(a == b);

		public bool Equals(SceneDefinition other) => other?.Name == Name;

		public override bool Equals(object obj) => obj is SceneDefinition other && Equals(other);

		public override int GetHashCode() => Name.GetHashCode();

		public override string ToString() => Title;
	}
}