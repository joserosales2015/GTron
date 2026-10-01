using GTron.Engine.Characters;

namespace GTron.Engine.Animation;

public sealed class AnimationPlayer
{
	private AnimationClip? _clip;
	private float _time;

	public bool IsPlaying => _clip is not null;

	public void Play(AnimationClip clip)
	{
		_clip = clip;
		_time = 0f;
	}

	public void Stop()
	{
		_clip = null;
		_time = 0f;
	}

	public void Update(float deltaTime, CharacterRig rig)
	{
		if (_clip is null)
		{
			return;
		}

		_time += deltaTime;

		while (_time >= _clip.Duration)
		{
			_time -= _clip.Duration;
		}

		_clip.Apply(rig, _time);
		rig.UpdateTransforms();
	}
}