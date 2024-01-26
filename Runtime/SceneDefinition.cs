// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using Depra.Inspector.Attributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Depra.Scenes
{
	[Serializable]
	public sealed class SceneDefinition : IEquatable<SceneDefinition>
	{
		[Scene] [SerializeField] private string _name;
		[SerializeField] private LoadSceneMode _loadMode;

		public static bool operator ==(SceneDefinition a, SceneDefinition b) => a?.Equals(b) ?? b is null;

		public static bool operator !=(SceneDefinition a, SceneDefinition b) => !(a == b);

		public string Name => _name;
		public LoadSceneMode LoadMode => _loadMode;

		public bool Equals(SceneDefinition other) => other != null && Name == other.Name;

		public override bool Equals(object obj) => obj is SceneDefinition other && Equals(other);

		public override int GetHashCode() => Name.GetHashCode();

		public override string ToString() => Name;
	}
}