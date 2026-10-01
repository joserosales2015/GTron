using GTron.Engine.Resources;
using GTron.Game.Scenes;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GTron.Engine.Core
{
	public sealed class GameApplication
	{
		public AssetManager Assets { get; } = new();

		private RenderTexture2D _internalRenderTarget;

		public void Run(IGameScene scene)
		{
			Raylib.SetConfigFlags(
				ConfigFlags.Msaa4xHint |
				ConfigFlags.VSyncHint);

			Raylib.InitWindow(
				GameSettings.InternalWidth,
				GameSettings.InternalHeight,
				GameSettings.WindowTitle);

			try
			{
				_internalRenderTarget = Raylib.LoadRenderTexture(GameSettings.InternalWidth, GameSettings.InternalHeight);

				scene.Load(Assets);

				Raylib.SetTargetFPS(GameSettings.TargetFps);

				while (!Raylib.WindowShouldClose())
				{
					float deltaTime = Raylib.GetFrameTime();

					if (Raylib.IsKeyPressed(GameSettings.ToggleFullscreenKey))
					{
						Raylib.ToggleBorderlessWindowed();
					}

					scene.Update(deltaTime);

					DrawSceneToInternalResolution(scene);
					DrawInternalResolutionToScreen();
				}
			}
			finally
			{
				Assets.Dispose();
				Raylib.UnloadRenderTexture(_internalRenderTarget);
				Raylib.CloseWindow();
			}
		}

		private void DrawSceneToInternalResolution(IGameScene scene)
		{
			Raylib.BeginTextureMode(_internalRenderTarget);

			try
			{
				scene.Draw();
			}
			finally
			{
				Raylib.EndTextureMode();
			}
		}

		private void DrawInternalResolutionToScreen()
		{
			Raylib.BeginDrawing();

			try
			{
				Raylib.ClearBackground(Color.Black);

				float scaleX = Raylib.GetScreenWidth() / (float)GameSettings.InternalWidth;
				float scaleY = Raylib.GetScreenHeight() / (float)GameSettings.InternalHeight;
				float scale = MathF.Min(scaleX, scaleY);

				float renderWidth = GameSettings.InternalWidth * scale;
				float renderHeight = GameSettings.InternalHeight * scale;

				float offsetX = (Raylib.GetScreenWidth() - renderWidth) / 2f;
				float offsetY = (Raylib.GetScreenHeight() - renderHeight) / 2f;

				var source = new Rectangle(0, 0, _internalRenderTarget.Texture.Width, -_internalRenderTarget.Texture.Height);
				var destination = new Rectangle(offsetX, offsetY, renderWidth, renderHeight);

				Raylib.DrawTexturePro(
					_internalRenderTarget.Texture,
					source,
					destination,
					Vector2.Zero,
					0f,
					Color.White);
			}
			finally
			{
				Raylib.EndDrawing();
			}
		}
	}
}
