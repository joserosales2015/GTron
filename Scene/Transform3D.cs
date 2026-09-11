using System.Numerics;
using Raylib_cs;

namespace GTron.Scene;

public struct Transform3D
{
	public Vector3 Position;
	public Vector3 Rotation;
	public Vector3 Scale;

	public Transform3D(Vector3 position)
	{
		Position = position;
		Rotation = Vector3.Zero;
		Scale = Vector3.One;
	}

	public Matrix4x4 ToMatrix()
	{
		Matrix4x4 scale = Raymath.MatrixScale(
			Scale.X,
			Scale.Y,
			Scale.Z
		);

		Matrix4x4 rotation =
			Raymath.MatrixRotateX(Rotation.X);

		rotation = Raymath.MatrixMultiply(
			rotation,
			Raymath.MatrixRotateY(Rotation.Y)
		);

		rotation = Raymath.MatrixMultiply(
			rotation,
			Raymath.MatrixRotateZ(Rotation.Z)
		);

		Matrix4x4 translation = Raymath.MatrixTranslate(
			Position.X,
			Position.Y,
			Position.Z
		);

		return Raymath.MatrixMultiply(
			Raymath.MatrixMultiply(scale, rotation),
			translation
		);
	}
}