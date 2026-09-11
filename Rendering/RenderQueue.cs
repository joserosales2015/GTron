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
		int visibleCount = 0;

		foreach (RenderItem item in _items)
		{
			bool visible = Frustum.ContainsSphere(
				camera,
				item.WorldPosition,
				item.BoundingSphereRadius,
				screenWidth,
				screenHeight
			);

			if (!visible)
				continue;

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