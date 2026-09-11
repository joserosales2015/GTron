namespace GTron.Rendering;

public sealed class RenderQueue
{
	private readonly List<RenderItem> _items = new();

	public void Submit(RenderItem item)
	{
		_items.Add(item);
	}

	public void Draw()
	{
		foreach (RenderItem item in _items)
		{
			item.Mesh.Draw(item.WorldMatrix);
		}
	}

	public void Clear()
	{
		_items.Clear();
	}
}