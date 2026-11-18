// SPDX-License-Identifier: Apache-2.0
// © 2023-2026 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Depra.Scenes
{
	[Serializable]
	public struct SceneReference : IEquatable<SceneReference>, ISerializationCallbackReceiver
	{
		[SerializeField] private Hash128 _guid;
		[SerializeField] private string _path;

		public static SceneReference Of(Scene scene)
		{
			if (!scene.IsValid() || string.IsNullOrEmpty(scene.path))
			{
				throw new ArgumentException($"Scene '{scene.name}' is not a saved scene asset.", nameof(scene));
			}

			var reference = new SceneReference { _path = scene.path };
#if UNITY_EDITOR
			reference._guid = Hash128.Parse(UnityEditor.AssetDatabase.AssetPathToGUID(scene.path));
#endif
			return reference;
		}

		public readonly bool IsValid => !string.IsNullOrEmpty(Path);

		public readonly string Path
		{
			get
			{
#if UNITY_EDITOR
				return ResolvePath(_guid);
#else
				return _path ?? string.Empty;
#endif
			}
		}

		public readonly string Name => System.IO.Path.GetFileNameWithoutExtension(Path);

		public readonly Scene Handle => SceneManager.GetSceneByPath(Path);

		public readonly bool Is(Scene scene) => IsValid && scene.IsValid() && scene.path == Path;

		public readonly bool IsActive() => Is(SceneManager.GetActiveScene());

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
#if UNITY_EDITOR
			_path = ResolvePath(_guid);
#endif
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize() { }

#if UNITY_EDITOR
		private static string ResolvePath(Hash128 guid) => guid.isValid
			? UnityEditor.AssetDatabase.GUIDToAssetPath(guid.ToString())
			: string.Empty;
#endif

		public readonly bool Equals(SceneReference other) => string.Equals(Path, other.Path, StringComparison.Ordinal);

		public override readonly bool Equals(object obj) => obj is SceneReference other && Equals(other);

		public override readonly int GetHashCode() => StringComparer.Ordinal.GetHashCode(Path);

		public override readonly string ToString() => Path;

		public static bool operator ==(SceneReference left, SceneReference right) => left.Equals(right);

		public static bool operator !=(SceneReference left, SceneReference right) => !left.Equals(right);
	}
}
