using System.Numerics;
using GTron.Scene;

namespace GTron.Rendering;

public sealed class RenderItem
{
	public required GpuCubeDemo Mesh;

	public Transform3D Transform;

	public float BoundingSphereRadius { get; set; } = 1.732f;

	public Matrix4x4 WorldMatrix =>
		Transform.ToMatrix();

	public Vector3 WorldPosition =>
		Transform.Position;
}