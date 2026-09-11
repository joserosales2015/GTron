using System.Numerics;
using Raylib_cs;

namespace GTron.Rendering;

public static class Frustum
{
	public static bool ContainsSphere(
		Camera3D camera,
		Vector3 center,
		float radius,
		int screenWidth,
		int screenHeight)
	{
		Vector3 forward = Vector3.Normalize(camera.Target - camera.Position);
		Vector3 right = Vector3.Normalize(Vector3.Cross(forward, camera.Up));
		Vector3 up = Vector3.Normalize(Vector3.Cross(right, forward));

		Vector3 toCenter = center - camera.Position;

		float depth = Vector3.Dot(toCenter, forward);

		if (depth + radius < 0.1f)
			return false;

		float aspect = screenWidth / (float)screenHeight;
		float verticalHalfSize =
			MathF.Tan(camera.FovY * MathF.PI / 360.0f) * depth;

		float horizontalHalfSize =
			verticalHalfSize * aspect;

		float horizontalDistance =
			Vector3.Dot(toCenter, right);

		float verticalDistance =
			Vector3.Dot(toCenter, up);

		if (MathF.Abs(horizontalDistance) >
			horizontalHalfSize + radius)
		{
			return false;
		}

		if (MathF.Abs(verticalDistance) >
			verticalHalfSize + radius)
		{
			return false;
		}

		return true;
	}
}