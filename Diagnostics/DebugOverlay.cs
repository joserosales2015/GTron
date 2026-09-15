#if DEBUG
using Raylib_cs;

namespace GTron.Diagnostics;

internal sealed class DebugOverlay
{
	private readonly int _x;
	private readonly int _y;
	private readonly int _fontSize;
	private readonly int _lineHeight;

	public DebugOverlay(
		int x = 20,
		int y = 50,
		int fontSize = 20,
		int lineHeight = 25)
	{
		_x = x;
		_y = y;
		_fontSize = fontSize;
		_lineHeight = lineHeight;
	}

	public void Draw()
	{
		Raylib.DrawFPS(_x, _y);

		Raylib.DrawText(
			$"Objetos en escena: {FrameStats.ObjectsInScene}",
			_x,
			_y + _lineHeight,
			_fontSize,
			Color.White
		);

		Raylib.DrawText(
			$"Dentro del frustum: {FrameStats.ObjectsInsideFrustum}",
			_x,
			_y + _lineHeight * 2,
			_fontSize,
			Color.White
		);

		Raylib.DrawText(
			$"Instancias enviadas: {FrameStats.ObjectsSubmitted}",
			_x,
			_y + _lineHeight * 3,
			_fontSize,
			Color.White
		);

		Raylib.DrawText(
			$"Draw calls instanciadas: {FrameStats.InstancedDrawCalls}",
			_x,
			_y + _lineHeight * 4,
			_fontSize,
			Color.White
		);
	}
}
#endif