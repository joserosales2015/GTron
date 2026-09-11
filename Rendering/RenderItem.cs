using System.Numerics;
using GTron.Scene;

namespace GTron.Rendering;

public sealed class RenderItem
{
	public required GpuCubeDemo Mesh;

	public Transform3D Transform;

	public Matrix4x4 WorldMatrix =>
		Transform.ToMatrix();
}