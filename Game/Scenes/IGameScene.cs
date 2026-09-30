using GTron.Engine.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace GTron.Game.Scenes
{
	public interface IGameScene
	{
		void Load(AssetManager assets);

		void Update(float deltaTime);

		void Draw();
	}
}
