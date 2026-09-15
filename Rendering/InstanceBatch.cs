using System.Numerics;

namespace GTron.Rendering;

internal sealed class InstanceBatch
{
	private Matrix4x4[] _transforms = Array.Empty<Matrix4x4>();

	public GpuCubeDemo Renderable { get; }

	public int Count { get; private set; }

	public InstanceBatch(GpuCubeDemo renderable)
	{
		Renderable = renderable;
	}

	public void Reset()
	{
		Count = 0;
	}

	public void Add(Matrix4x4 transform)
	{
		EnsureCapacity(Count + 1);

		_transforms[Count] = transform;
		Count++;
	}

	public void Draw()
	{
		Renderable.DrawInstanced(_transforms, Count);
	}

	private void EnsureCapacity(int requiredCapacity)
	{
		if (_transforms.Length >= requiredCapacity)
			return;

		int newCapacity = Math.Max(
			requiredCapacity,
			Math.Max(16, _transforms.Length * 2)
		);

		Array.Resize(ref _transforms, newCapacity);
	}
}