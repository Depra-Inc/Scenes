// SPDX-License-Identifier: Apache-2.0
// © 2026 Depra <n.melnikov@depra.org>

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Depra.Scenes.Editor
{
	internal sealed class SceneReferenceValidation : IPreprocessBuildWithReport
	{
		private const string ROOT = "Assets";
		private const string MENU_ITEM = "Tools/Depra/Scenes/Validate Scene References";

		private static readonly string[] EXTENSIONS = { ".asset", ".prefab", ".unity" };

		private static readonly Regex REFERENCE = new(
			@"^(?<indent>[ -]*)_guid:[ \t]*\r?\n[ \t]*serializedVersion: 2[ \t]*\r?\n[ \t]*Hash: (?<hash>[0-9a-f]{32})[ \t]*\r?\n[ \t]*_path:",
			RegexOptions.Multiline | RegexOptions.Compiled);

		int IOrderedCallback.callbackOrder => 0;

		void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report)
		{
			var issues = Collect();
			LogWarnings(issues);

			var errors = issues.Where(issue => issue.Status == SceneStatus.MISSING).ToArray();
			if (errors.Length > 0)
			{
				throw new BuildFailedException(Format("Scene references point to deleted scenes:", errors));
			}
		}

		[MenuItem(MENU_ITEM)]
		private static void ValidateFromMenu()
		{
			var issues = Collect();
			if (issues.Count == 0)
			{
				Debug.Log("All scene references are valid.");
				return;
			}

			LogWarnings(issues);
			var errors = issues.Where(issue => issue.Status == SceneStatus.MISSING).ToArray();
			if (errors.Length > 0)
			{
				Debug.LogError(Format("Scene references point to deleted scenes:", errors));
			}
		}

		private static List<Issue> Collect()
		{
			var issues = new List<Issue>();
			var files = Directory
				.EnumerateFiles(ROOT, "*.*", SearchOption.AllDirectories)
				.Where(path => EXTENSIONS.Contains(Path.GetExtension(path)));

			foreach (var file in files)
			{
				var text = File.ReadAllText(file);
				if (!text.Contains("_path:"))
				{
					continue;
				}

				Inspect(file.Replace('\\', '/'), text, issues);
			}

			return issues;
		}

		private static void Inspect(string path, string text, List<Issue> issues)
		{
			foreach (Match match in REFERENCE.Matches(text))
			{
				var status = SceneReferenceEditor.GetStatus(Hash128.Parse(match.Groups["hash"].Value));
				if (status is SceneStatus.NONE or SceneStatus.OK)
				{
					continue;
				}

				var line = CountLines(text, match.Index);
				issues.Add(new Issue(path, line, FindField(text, match.Index, match.Groups["indent"].Length), status));
			}
		}

		private static int CountLines(string text, int index)
		{
			var line = 1;
			for (var i = 0; i < index; i++)
			{
				if (text[i] == '\n')
				{
					line++;
				}
			}

			return line;
		}

		private static string FindField(string text, int index, int indent)
		{
			var end = index;
			while (end > 1)
			{
				var start = text.LastIndexOf('\n', end - 2) + 1;
				var line = text.Substring(start, end - start).TrimEnd('\r', '\n');
				var trimmed = line.TrimStart(' ', '-');
				var lineIndent = line.Length - trimmed.Length;
				if (trimmed.EndsWith(":") && lineIndent < indent)
				{
					return trimmed.TrimEnd(':');
				}

				if (start == 0)
				{
					break;
				}

				end = start;
			}

			return string.Empty;
		}

		private static void LogWarnings(IEnumerable<Issue> issues)
		{
			var warnings = issues.Where(issue => issue.Status != SceneStatus.MISSING).ToArray();
			if (warnings.Length > 0)
			{
				Debug.LogWarning(Format("Scene references point to scenes excluded from the build:", warnings));
			}
		}

		private static string Format(string header, IEnumerable<Issue> issues)
		{
			var builder = new StringBuilder(header);
			foreach (var issue in issues)
			{
				builder.AppendLine().Append($"  {issue.AssetPath}:{issue.Line} → {issue.Field} ({issue.Status})");
			}

			return builder.ToString();
		}

		private readonly struct Issue
		{
			public readonly string AssetPath;
			public readonly int Line;
			public readonly string Field;
			public readonly SceneStatus Status;

			public Issue(string assetPath, int line, string field, SceneStatus status)
			{
				AssetPath = assetPath;
				Line = line;
				Field = field;
				Status = status;
			}
		}
	}
}
