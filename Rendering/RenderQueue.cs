using Raylib_cs;
using System.Numerics;

namespace GTron.Rendering;

public sealed class RenderQueue
{
	private readonly List<RenderItem> _items = new();

	public void Submit(RenderItem item)
	{
		_items.Add(item);
	}

	public int Draw(Camera3D camera, int screenWidth, int screenHeight)
	{
		float aspectRatio =
			screenWidth / (float)screenHeight;

		Frustum frustum = new Frustum(
			camera,
			aspectRatio
		);

		int visibleCount = 0;

		foreach (RenderItem item in _items)
		{
			if (!frustum.ContainsSphere(
					item.WorldPosition,
					item.BoundingSphereRadius))
			{
				continue;
			}

			item.Mesh.Draw(item.WorldMatrix);
			visibleCount++;
		}

		return visibleCount;
	}

	public void Clear()
	{
		_items.Clear();
	}
}