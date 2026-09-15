#if DEBUG
namespace GTron.Diagnostics;

internal static class FrameStats
{
	public static int ObjectsInScene { get; private set; }
	public static int ObjectsInsideFrustum { get; private set; }
	public static int ObjectsSubmitted { get; private set; }
	public static int InstancedDrawCalls { get; private set; }

	public static void BeginFrame()
	{
		ObjectsInScene = 0;
		ObjectsInsideFrustum = 0;
		ObjectsSubmitted = 0;
		InstancedDrawCalls = 0;
	}

	public static void AddObjectsInScene(int count)
	{
		ObjectsInScene += count;
	}

	public static void AddObjectInsideFrustum()
	{
		ObjectsInsideFrustum++;
	}

	public static void AddObjectSubmitted()
	{
		ObjectsSubmitted++;
	}

	public static void AddInstancedDrawCall()
	{
		InstancedDrawCalls++;
	}
}
#endif