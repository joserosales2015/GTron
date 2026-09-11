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
		float aspectRatio = screenWidth / (float)screenHeight;

		Frustum frustum = new Frustum(
			camera,
			aspectRatio
		);

		int visibleCount = 0;

		foreach (RenderItem item in _items)
		{
			item.GetWorldAabb(
				out Vector3 minimum,
				out Vector3 maximum
			);

			if (!frustum.ContainsAabb(minimum, maximum))
				continue;

			item.Mesh.Draw(item.WorldMatrix);

			Raylib.DrawBoundingBox(
				new BoundingBox
				{
					Min = minimum,
					Max = maximum
				},
				Color.Yellow
			);

			visibleCount++;
		}

		return visibleCount;
	}

	public void Clear()
	{
		_items.Clear();
	}
}