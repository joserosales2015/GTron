using GTron.Engine.Core;
using GTron.Game.Scenes;

namespace GTron
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			var application = new GameApplication();
			var scene = new DemoScene();

			application.Run(scene);
		}
	}
}
