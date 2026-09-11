using GTron.Scene;
using Raylib_cs;
using System.Numerics;

namespace GTron.Rendering;

public sealed class RenderItem
{
	public required GpuCubeDemo Mesh;

	public Transform3D Transform;

	public float BoundingSphereRadius { get; set; } = 1.732f;

	public Vector3 LocalHalfExtents { get; set; } = Vector3.One;

	public Matrix4x4 WorldMatrix =>
		Transform.ToMatrix();

	public Vector3 WorldPosition =>
		Transform.Position;

	public void GetWorldAabb(out Vector3 minimum, out Vector3 maximum)
	{
		Matrix4x4 world = WorldMatrix;

		Vector3[] corners =
		{
			new(-LocalHalfExtents.X, -LocalHalfExtents.Y, -LocalHalfExtents.Z),
			new( LocalHalfExtents.X, -LocalHalfExtents.Y, -LocalHalfExtents.Z),
			new(-LocalHalfExtents.X,  LocalHalfExtents.Y, -LocalHalfExtents.Z),
			new( LocalHalfExtents.X,  LocalHalfExtents.Y, -LocalHalfExtents.Z),
			new(-LocalHalfExtents.X, -LocalHalfExtents.Y,  LocalHalfExtents.Z),
			new( LocalHalfExtents.X, -LocalHalfExtents.Y,  LocalHalfExtents.Z),
			new(-LocalHalfExtents.X,  LocalHalfExtents.Y,  LocalHalfExtents.Z),
			new( LocalHalfExtents.X,  LocalHalfExtents.Y,  LocalHalfExtents.Z)
		};

		minimum = new Vector3(float.PositiveInfinity);
		maximum = new Vector3(float.NegativeInfinity);

		foreach (Vector3 corner in corners)
		{
			Vector3 worldCorner = Raymath.Vector3Transform(corner, world);

			minimum = Vector3.Min(minimum, worldCorner);
			maximum = Vector3.Max(maximum, worldCorner);
		}
	}
}