#if DEBUG
using GTron.Diagnostics;
#endif
using Raylib_cs;

namespace GTron.Rendering;

public sealed class RenderQueue
{
	private readonly List<RenderItem> _items = new();

	private readonly Dictionary<GpuCubeDemo, InstanceBatch> _batches = new();

	private readonly List<InstanceBatch> _activeBatches = new();

	public void Submit(RenderItem item)
	{
		_items.Add(item);
	}

	public void Draw(Camera3D camera, int screenWidth, int screenHeight)
	{
		ResetActiveBatches();

#if DEBUG
		FrameStats.AddObjectsInScene(_items.Count);
#endif

		float aspectRatio = screenWidth / (float)screenHeight;

		Frustum frustum = new(
			camera,
			aspectRatio
		);

		foreach (RenderItem item in _items)
		{
			item.GetWorldAabb(
				out System.Numerics.Vector3 minimum,
				out System.Numerics.Vector3 maximum
			);

			if (!frustum.ContainsAabb(minimum, maximum))
				continue;

#if DEBUG
			FrameStats.AddObjectInsideFrustum();
#endif

			InstanceBatch batch = GetOrCreateBatch(item.Mesh);

			if (batch.Count == 0)
				_activeBatches.Add(batch);

			batch.Add(item.WorldMatrix);

#if DEBUG
			FrameStats.AddObjectSubmitted();
#endif
		}

		foreach (InstanceBatch batch in _activeBatches)
		{
			batch.Draw();

#if DEBUG
			FrameStats.AddInstancedDrawCall();
#endif
		}
	}

	public void Clear()
	{
		_items.Clear();
	}

	private InstanceBatch GetOrCreateBatch(GpuCubeDemo renderable)
	{
		if (_batches.TryGetValue(renderable, out InstanceBatch? batch))
		{
			return batch;
		}

		batch = new InstanceBatch(renderable);
		_batches.Add(renderable, batch);

		return batch;
	}

	private void ResetActiveBatches()
	{
		foreach (InstanceBatch batch in _activeBatches)
			batch.Reset();

		_activeBatches.Clear();
	}
}