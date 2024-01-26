// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using Depra.Inspector.Attributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Depra.Scenes
{
	[Serializable]
	public sealed partial class SceneDefinition
	{
		[Scene] [SerializeField] private string _name;
		[SerializeField] private string _title;
		[SerializeField] private LoadSceneMode _loadMode;
		[TextArea] [SerializeField] private string _description;

		public SceneDefinition(string name, LoadSceneMode loadMode)
		{
			_name = name;
			_loadMode = loadMode;
		}

		public string Name => _name;
		public string Description => _description;
		public LoadSceneMode LoadMode => _loadMode;
		public Scene Scene => SceneManager.GetSceneByName(Name);
		public string Title => string.IsNullOrEmpty(_title) ? Name : _title;
	}

	public sealed partial class SceneDefinition : IEquatable<SceneDefinition>
	{
		public static bool operator ==(SceneDefinition a, SceneDefinition b) => a?.Equals(b) ?? b is null;

		public static bool operator !=(SceneDefinition a, SceneDefinition b) => !(a == b);

		public bool Equals(SceneDefinition other) => other != null && Name == other.Name;

		public override bool Equals(object obj) => obj is SceneDefinition other && Equals(other);

		public override int GetHashCode() => Name.GetHashCode();

		public override string ToString() => Title;
	}
}