using System;
using System.Collections.Generic;
using System.Text;

namespace GTron.Engine.Resources
{
	public static class AssetPaths
	{
		private static readonly string AssetsRoot = Path.Combine(AppContext.BaseDirectory, "Assets");

		private static string FromDirectory(string directory, string relativePath)
		{
			if (string.IsNullOrWhiteSpace(relativePath))
			{
				throw new ArgumentException("La ruta del recurso no puede estar vacía.", nameof(relativePath));
			}

			string directoryPath = Path.Combine(AssetsRoot, directory);
			string fullPath = Path.GetFullPath(Path.Combine(directoryPath, relativePath));

			if (!fullPath.StartsWith(directoryPath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentException($"La ruta '{relativePath}' sale de Assets/{directory}.", nameof(relativePath));
			}

			return fullPath;
		}

		public static string Map(string relativePath)
		{
			return FromDirectory("Maps", relativePath);
		}

		public static string Model(string relativePath)
		{
			return FromDirectory("Models", relativePath);
		}

		public static string Rig(string relativePath)
		{
			return FromDirectory("Rigs", relativePath);
		}

		public static string Texture(string relativePath)
		{
			return FromDirectory("Textures", relativePath);
		}

		public static string Shader(string relativePath)
		{
			return FromDirectory("Shaders", relativePath);
		}
	}
}
