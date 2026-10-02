using GTron.Engine.Resources;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using GTron.Engine.Characters;
using GTron.Engine.Animation;

namespace GTron.Game.Scenes
{
	public sealed class DemoScene : IGameScene
	{
		private Texture2D _wallTexture;
		private CharacterRig _character = null!;

		private float _shoulderAngleDegrees;
		private float _elbowAngleDegrees;

		// Iluminación direccional
		private Shader _lightingShader;

		private int _lightDirectionLocation;
		private int _lightColorLocation;
		private int _ambientColorLocation;
		private bool _drawWireframe;
		private readonly AnimationPlayer _animationPlayer = new();
		private AnimationClip _walkAnimation = null!;

		private Camera3D _camera = new()
		{
			Position = new Vector3(0f, 1f, 6f),
			Target = new Vector3(0f, 0f, 0f),
			Up = Vector3.UnitY,
			FovY = 50,
			Projection = CameraProjection.Perspective
		};

		private void ApplyLightingShader(ref Model model)
		{
			for (int materialIndex = 0;
				 materialIndex < model.MaterialCount;
				 materialIndex++)
			{
				Raylib.SetMaterialShader(
					ref model,
					materialIndex,
					ref _lightingShader);
			}
		}

		private void ChangeCameraDistance(float deltaDistance)
		{
			Vector3 direction = Vector3.Normalize(_camera.Position - _camera.Target);

			float currentDistance = Vector3.Distance(_camera.Position, _camera.Target);

			float newDistance = MathF.Max(1f, currentDistance + deltaDistance);

			_camera.Position = _camera.Target + direction * newDistance;
		}

		private void ConfigureLighting()
		{
			Vector3 direction = Vector3.Normalize(
				new Vector3(-1.45f, -0.5f, -1.0f));

			Vector4 lightColor = new(1f, 0.95f, 0.82f, 1f);

			Vector4 ambientColor = new(0.24f, 0.28f, 0.36f, 1f);

			Raylib.SetShaderValue(
				_lightingShader,
				_lightDirectionLocation,
				direction,
				ShaderUniformDataType.Vec3);

			Raylib.SetShaderValue(
				_lightingShader,
				_lightColorLocation,
				lightColor,
				ShaderUniformDataType.Vec4);

			Raylib.SetShaderValue(
				_lightingShader,
				_ambientColorLocation,
				ambientColor,
				ShaderUniformDataType.Vec4);
		}

		private static AnimationClip CreateWalkAnimation()
		{
			const float duration = 0.8f;

			return new AnimationClip(duration)

				// Piernas: una avanza mientras la otra retrocede.
				.AddKeyframe("upper-leg-left", 0f, 35f)
				.AddKeyframe("upper-leg-left", 0.4f, -35f)
				.AddKeyframe("upper-leg-left", 0.8f, 35f)

				.AddKeyframe("upper-leg-right", 0f, -35f)
				.AddKeyframe("upper-leg-right", 0.4f, 35f)
				.AddKeyframe("upper-leg-right", 0.8f, -35f)

				// Brazos: movimiento contrario a las piernas.
				.AddKeyframe("upper-arm-left", 0f, -45f)
				.AddKeyframe("upper-arm-left", 0.4f, 45f)
				.AddKeyframe("upper-arm-left", 0.8f, -45f)

				.AddKeyframe("upper-arm-right", 0f, 45f)
				.AddKeyframe("upper-arm-right", 0.4f, -45f)
				.AddKeyframe("upper-arm-right", 0.8f, 45f);
		}

		public void Draw()
		{
			Raylib.ClearBackground(new Color(0, 0, 0, 255));

			// Decoración de fondo.
			//Raylib.DrawTexturePro(
			//	_wallTexture,
			//	new Rectangle(0, 0, _wallTexture.Width, _wallTexture.Height),
			//	new Rectangle(90, 230, 180, 190),
			//	Vector2.Zero,
			//	0f,
			//	Color.White);

			Raylib.BeginMode3D(_camera);

			//Raylib.DrawGrid(20, 1f);

			if (_drawWireframe)
			{
				_character.DrawWireframe(new Color(0, 255, 0, 255));
			}
			else
			{
				_character.Draw();
			}

			Raylib.EndMode3D();

			Raylib.DrawText("ESPACIO: caminar | F3: wireframe | Flechas: mover/cámara",	24, 505, 18, Color.White);

			Raylib.DrawFPS(840, 20);
		}

		public void Load(AssetManager assets)
		{
			_wallTexture = assets.LoadTexture("wall.png");
			_character = CharacterRigLoader.Load(assets, "basic_character.rig.json");
			_walkAnimation = CreateWalkAnimation();

			// Cargar el shader de iluminación direccional
			_lightingShader = assets.LoadShader("lighting.vs", "directional_light.fs");
			_lightDirectionLocation = Raylib.GetShaderLocation(_lightingShader,	"lightDirection");
			_lightColorLocation = Raylib.GetShaderLocation(_lightingShader,	"lightColor");
			_ambientColorLocation = Raylib.GetShaderLocation(_lightingShader, "ambientColor");

			ConfigureLighting();

			_character.ApplyShader(ref _lightingShader);
		}
			
		private void ResetCharacterPose()
		{
			_character.ResetPose();

			_shoulderAngleDegrees = 0f;
			_elbowAngleDegrees = 0f;
		}

		public void Update(float deltaTime)
		{
			const float cameraSpeed = 4f;
			const float rotationSpeed = 120f;

			//if (Raylib.IsKeyDown(KeyboardKey.Q))
			//{
			//	_camera.FovY = MathF.Max(1f, _camera.FovY - deltaTime * 10f);
			//}

			//if (Raylib.IsKeyDown(KeyboardKey.E))
			//{
			//	_camera.FovY = MathF.Min(80f, _camera.FovY + deltaTime * 10f);
			//}

			if (Raylib.IsKeyPressed(KeyboardKey.Space))
			{
				ResetCharacterPose();

				if (_animationPlayer.IsPlaying)
				{
					_animationPlayer.Stop();
				}
				else
				{
					_animationPlayer.Play(_walkAnimation);
				}
			}

			if (Raylib.IsKeyPressed(KeyboardKey.F3))
			{
				_drawWireframe = !_drawWireframe;
			}

			if (!_animationPlayer.IsPlaying)
			{
				if (Raylib.IsKeyDown(KeyboardKey.A))
				{
					_shoulderAngleDegrees += rotationSpeed * deltaTime;
				}

				if (Raylib.IsKeyDown(KeyboardKey.D))
				{
					_shoulderAngleDegrees -= rotationSpeed * deltaTime;
				}

				if (Raylib.IsKeyDown(KeyboardKey.W))
				{
					_elbowAngleDegrees += rotationSpeed * deltaTime;
				}

				if (Raylib.IsKeyDown(KeyboardKey.S))
				{
					_elbowAngleDegrees -= rotationSpeed * deltaTime;
				}
			}

			const float characterSpeed = 3f;

			if (Raylib.IsKeyDown(KeyboardKey.Up))
			{
				ChangeCameraDistance(-cameraSpeed * deltaTime);
			}

			if (Raylib.IsKeyDown(KeyboardKey.Down))
			{
				ChangeCameraDistance(cameraSpeed * deltaTime);
			}

			if (Raylib.IsKeyDown(KeyboardKey.Left))
			{
				_character.Position -= Vector3.UnitX * characterSpeed * deltaTime;
			}

			if (Raylib.IsKeyDown(KeyboardKey.Right))
			{
				_character.Position += Vector3.UnitX * characterSpeed * deltaTime;
			}

			if (_animationPlayer.IsPlaying)
			{
				_animationPlayer.Update(deltaTime, _character);
			}
			else
			{
				_shoulderAngleDegrees = _character.SetJointAngleDegrees("upper-arm-left", _shoulderAngleDegrees);
				_elbowAngleDegrees = _character.SetJointAngleDegrees("lower-arm-left", _elbowAngleDegrees);
				_shoulderAngleDegrees = _character.SetJointAngleDegrees("upper-arm-right", _shoulderAngleDegrees);
				_elbowAngleDegrees = _character.SetJointAngleDegrees("lower-arm-right", _elbowAngleDegrees);
			}

			_character.UpdateTransforms();
		}
	}
}