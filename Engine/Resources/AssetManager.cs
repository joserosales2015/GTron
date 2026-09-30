using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Text;

namespace GTron.Engine.Resources
{
	public sealed class AssetManager : IDisposable
	{
		private readonly Dictionary<string, Texture2D> _textures = new();
		private readonly Dictionary<string, Model> _models = new();
		private readonly Dictionary<string, Shader> _shaders = new();
		private readonly Dictionary<string, string> _rigTexts = new();

		public Model LoadModel(string relativePath)
		{
			if (_models.TryGetValue(relativePath, out Model model))
			{
				return model;
			}

			string fullPath = AssetPaths.Model(relativePath);

			if (!File.Exists(fullPath))
			{
				throw new FileNotFoundException($"No se encontró el modelo '{relativePath}'.", fullPath);
			}

			model = Raylib.LoadModel(fullPath);

			if (model.MeshCount == 0)
			{
				throw new InvalidOperationException($"Raylib no pudo cargar ninguna malla desde '{fullPath}'.");
			}

			_models.Add(relativePath, model);

			return model;
		}

		public string LoadRigText(string relativePath)
		{
			if (_rigTexts.TryGetValue(relativePath, out string? text))
			{
				return text;
			}

			string fullPath = AssetPaths.Rig(relativePath);

			if (!File.Exists(fullPath))
			{
				throw new FileNotFoundException(
					$"No se encontró el rig '{relativePath}'.",
					fullPath);
			}

			text = File.ReadAllText(fullPath);
			_rigTexts.Add(relativePath, text);

			return text;
		}

		public Shader LoadShader(string vertexRelativePath,	string fragmentRelativePath)
		{
			string key = $"{vertexRelativePath}|{fragmentRelativePath}";

			if (_shaders.TryGetValue(key, out Shader shader))
			{
				return shader;
			}

			string vertexPath = AssetPaths.Shader(vertexRelativePath);
			string fragmentPath = AssetPaths.Shader(fragmentRelativePath);

			if (!File.Exists(vertexPath))
			{
				throw new FileNotFoundException(
					$"No se encontró el shader de vértices '{vertexRelativePath}'.",
					vertexPath);
			}

			if (!File.Exists(fragmentPath))
			{
				throw new FileNotFoundException(
					$"No se encontró el shader de fragmentos '{fragmentRelativePath}'.",
					fragmentPath);
			}

			shader = Raylib.LoadShader(vertexPath, fragmentPath);

			if (shader.Id == 0)
			{
				throw new InvalidOperationException(
					$"Raylib no pudo cargar el shader '{key}'.");
			}

			_shaders.Add(key, shader);

			return shader;
		}

		public Texture2D LoadTexture(string relativePath)
		{
			if (_textures.TryGetValue(relativePath, out Texture2D texture))
			{
				return texture;
			}

			string fullPath = AssetPaths.Texture(relativePath);

			if (!File.Exists(fullPath))
			{
				throw new FileNotFoundException($"No se encontró la textura '{relativePath}'.", fullPath);
			}

			texture = Raylib.LoadTexture(fullPath);
			_textures.Add(relativePath, texture);

			return texture;
		}

		public void Dispose()
		{
			foreach (Model model in _models.Values)
			{
				Raylib.UnloadModel(model);
			}

			_models.Clear();

			foreach (Shader shader in _shaders.Values)
			{
				Raylib.UnloadShader(shader);
			}

			_shaders.Clear();

			foreach (Texture2D texture in _textures.Values)
			{
				Raylib.UnloadTexture(texture);
			}

			_textures.Clear();
		}
	}
}
