// SPDX-License-Identifier: Apache-2.0
// © 2026 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using UnityEngine;

namespace Depra.Scenes.Operations
{
	internal sealed class ProgressRange : IProgress<float>
	{
		private readonly float _from;
		private readonly float _to;
		private readonly IProgress<float> _parent;

		public ProgressRange(IProgress<float> parent, float from, float to)
		{
			_parent = parent;
			_from = from;
			_to = to;
		}

		void IProgress<float>.Report(float value) => _parent?.Report(Mathf.Lerp(_from, _to, value));
	}
}
