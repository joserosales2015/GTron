using Raylib_cs;
using System.Numerics;

namespace GTron.Rendering;

public sealed class RenderQueue
{
	private readonly List<RenderItem> _items = new();

	public bool DrawBounds { get; set; }

	public void Submit(RenderItem item)
	{
		_items.Add(item);
	}

	public int Draw(Camera3D camera, int screenWidth, int screenHeight)
	{
		float aspectRatio = screenWidth / (float)screenHeight;

		Frustum frustum = new Frustum(
			camera,
			aspectRatio
		);

		var visibleTransforms = new List<Matrix4x4>();

		GpuCubeDemo? sharedMesh = null;

		foreach (RenderItem item in _items)
		{
			item.GetWorldAabb(
				out Vector3 minimum,
				out Vector3 maximum
			);

			if (!frustum.ContainsAabb(minimum, maximum))
				continue;
			
			sharedMesh ??= item.Mesh;
			visibleTransforms.Add(item.WorldMatrix);

			if (DrawBounds)
			{
				Vector3 padding = new Vector3(0.08f);

				Rlgl.DisableDepthTest();

				Raylib.DrawBoundingBox(
					new BoundingBox
					{
						Min = minimum - padding,
						Max = maximum + padding
					},
					Color.Yellow
				);

				Rlgl.EnableDepthTest();
			}
		}

		if (sharedMesh is not null)
		{
			Matrix4x4[] transforms = visibleTransforms.ToArray();

			sharedMesh.DrawInstanced(
				transforms,
				transforms.Length
			);
		}

		return visibleTransforms.Count;
	}

	public void Clear()
	{
		_items.Clear();
	}
}