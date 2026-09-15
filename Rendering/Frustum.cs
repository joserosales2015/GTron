using System.Numerics;
using Raylib_cs;

namespace GTron.Rendering;

public readonly struct FrustumPlane
{
	public readonly Vector3 Normal;
	public readonly float Distance;

	public FrustumPlane(Vector3 normal, Vector3 point)
	{
		Normal = Vector3.Normalize(normal);
		Distance = -Vector3.Dot(Normal, point);
	}

	public float SignedDistance(Vector3 point)
	{
		return Vector3.Dot(Normal, point) + Distance;
	}
}

public readonly struct Frustum
{
	private readonly FrustumPlane _near;
	private readonly FrustumPlane _far;
	private readonly FrustumPlane _left;
	private readonly FrustumPlane _right;
	private readonly FrustumPlane _top;
	private readonly FrustumPlane _bottom;

	public Frustum(Camera3D camera, float aspectRatio, float nearDistance = 0.1f, float farDistance = 100.0f)
	{
		Vector3 forward = Vector3.Normalize(camera.Target - camera.Position);

		Vector3 right = Vector3.Normalize(Vector3.Cross(forward, camera.Up));

		Vector3 up = Vector3.Normalize(Vector3.Cross(right, forward));

		float verticalAngle = camera.FovY * MathF.PI / 180.0f;

		float horizontalAngle =
			2.0f * MathF.Atan(
				MathF.Tan(verticalAngle * 0.5f) * aspectRatio
			);

		float halfVertical = verticalAngle * 0.5f;

		float halfHorizontal = horizontalAngle * 0.5f;

		Vector3 nearPoint = camera.Position + forward * nearDistance;

		Vector3 farPoint = camera.Position + forward * farDistance;

		_near = new FrustumPlane(forward, nearPoint);
		_far = new FrustumPlane(-forward, farPoint);

		float horizontalSlope =	MathF.Tan(halfHorizontal);

		float verticalSlope = MathF.Tan(halfVertical);

		_left = new FrustumPlane(
			right + forward * horizontalSlope,
			camera.Position
		);

		_right = new FrustumPlane(
			-right + forward * horizontalSlope,
			camera.Position
		);

		_bottom = new FrustumPlane(
			up + forward * verticalSlope,
			camera.Position
		);

		_top = new FrustumPlane(
			-up + forward * verticalSlope,
			camera.Position
		);
	}

	public bool ContainsAabb(Vector3 minimum, Vector3 maximum)
	{
		FrustumPlane[] planes =
		{
			_near,
			_far,
			_left,
			_right,
			_top,
			_bottom
		};

		foreach (FrustumPlane plane in planes)
		{
			Vector3 positiveVertex = new(
				plane.Normal.X >= 0.0f ? maximum.X : minimum.X,
				plane.Normal.Y >= 0.0f ? maximum.Y : minimum.Y,
				plane.Normal.Z >= 0.0f ? maximum.Z : minimum.Z
			);

			if (plane.SignedDistance(positiveVertex) < 0.0f)
				return false;
		}

		return true;
	}

	public bool ContainsSphere(Vector3 center, float radius)
	{
		return IsInside(_near, center, radius)
			&& IsInside(_far, center, radius)
			&& IsInside(_left, center, radius)
			&& IsInside(_right, center, radius)
			&& IsInside(_top, center, radius)
			&& IsInside(_bottom, center, radius);
	}

	private static bool IsInside(FrustumPlane plane, Vector3 center, float radius)
	{
		return plane.SignedDistance(center) >= -radius;
	}
}