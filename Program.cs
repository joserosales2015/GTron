using Raylib_cs;
using System.Numerics;

namespace GTron
{
	internal class Program
	{
		public const int InternalWidth = 960;//720;//480;//
		public const int InternalHeight = 540;//405;//270;//

		static void Main(string[] args)
		{
			Raylib.InitWindow(InternalWidth, InternalHeight, "Raylib Motor 3D");
			Raylib.SetTargetFPS(60);

			var camera = new Camera3D
			{
				Position = new Vector3(6.0f, 4.0f, 6.0f),
				Target = new Vector3(0.0f, 1.0f, 0.0f),
				Up = Vector3.UnitY,
				FovY = 45.0f,
				Projection = CameraProjection.Perspective
			};

			while (!Raylib.WindowShouldClose())
			{
				if (Raylib.IsKeyDown(KeyboardKey.W))
					camera.Position.Z -= 0.05f;

				if (Raylib.IsKeyDown(KeyboardKey.S))
					camera.Position.Z += 0.05f;

				if (Raylib.IsKeyDown(KeyboardKey.A))
					camera.Position.X -= 0.05f;

				if (Raylib.IsKeyDown(KeyboardKey.D))
					camera.Position.X += 0.05f;

				Raylib.BeginDrawing();
				Raylib.ClearBackground(Color.Black);

				Raylib.BeginMode3D(camera);
				{
					Raylib.DrawGrid(20, 1.0f);

					Raylib.DrawCube(
						new Vector3(0.0f, 1.0f, 0.0f),
						2.0f,
						2.0f,
						2.0f,
						Color.Black
					);

					Raylib.DrawCubeWires(
						new Vector3(0.0f, 1.0f, 0.0f),
						2.0f,
						2.0f,
						2.0f,
						Color.Lime
					);
				}
				Raylib.EndMode3D();

				Raylib.DrawText("WASD - mover camara", 20, 20, 20, Color.White);
				Raylib.DrawFPS(20, 50);

				Raylib.EndDrawing();
			}

			Raylib.CloseWindow();
		}
	}
}
