using GTron.Rendering;
using Raylib_cs;
using System.Numerics;
using GTron.Scene;
using System.Collections.Generic;

namespace GTron
{
	internal class Program
	{
		public const int InternalWidth = 960;//720;//480;//
		public const int InternalHeight = 540;//405;//270;//

		static void Main(string[] args)
		{
			Raylib.SetConfigFlags(ConfigFlags.Msaa4xHint);
			Raylib.InitWindow(
				InternalWidth,
				InternalHeight,
				"GTron - Shader GPU"
			);

			Raylib.SetTargetFPS(60);

			var camera = new Camera3D
			{
				Position = new Vector3(6.0f, 4.0f, 6.0f),
				Target = new Vector3(0.0f, 4.0f, 0.0f),
				Up = Vector3.UnitY,
				FovY = 45.0f,
				Projection = CameraProjection.Perspective
			};

			using var cube = new GpuCubeDemo(
				"Assets/Shaders/basic.vs",
				"Assets/Shaders/basic.fs"
			);

			var renderQueue = new RenderQueue
			{
				DrawBounds = false
			};

			var objects = new List<RenderItem>();

			for (int x = -20; x <= 20; x++)
			{
				for (int z = -20; z <= 20; z++)
				{
					objects.Add(new RenderItem
					{
						Mesh = cube,
						Transform = new Transform3D(
							new Vector3(x * 2.5f, 1.0f, z * 2.5f)
						)
					});
				}
			}

			while (!Raylib.WindowShouldClose())
			{
				UpdateCamera(ref camera);

				renderQueue.Clear();

				foreach (RenderItem item in objects)
				{
					item.Transform.Rotation.Y += Raylib.GetFrameTime();
					renderQueue.Submit(item);
				}

				Raylib.BeginDrawing();
				Raylib.ClearBackground(Color.Black);

				Raylib.BeginMode3D(camera);

				Raylib.DrawGrid(20, 1.0f);
				
				int visibleCount = renderQueue.Draw(
					camera,
					InternalWidth,
					InternalHeight
				);

				Raylib.EndMode3D();

				Raylib.DrawText("Malla GPU + shader GLSL", 20, 20, 20, Color.White);
				Raylib.DrawFPS(20, 50);
				Raylib.DrawText(
					$"Objetos visibles: {visibleCount} / {objects.Count}",
					20,
					80,
					20,
					Color.White
				);

				Raylib.EndDrawing();
			}

			Raylib.CloseWindow();
		}

		private static void UpdateCamera(ref Camera3D camera)
		{
			float speed = 5.0f * Raylib.GetFrameTime();

			Vector3 forward =
				Vector3.Normalize(camera.Target - camera.Position);

			Vector3 right =
				Vector3.Normalize(Vector3.Cross(forward, camera.Up));

			Vector3 movement = Vector3.Zero;

			if (Raylib.IsKeyDown(KeyboardKey.W))
				movement += forward * speed;

			if (Raylib.IsKeyDown(KeyboardKey.S))
				movement -= forward * speed;

			if (Raylib.IsKeyDown(KeyboardKey.D))
				movement += right * speed;

			if (Raylib.IsKeyDown(KeyboardKey.A))
				movement -= right * speed;

			camera.Position += movement;
			camera.Target += movement;
		}
	}
}
