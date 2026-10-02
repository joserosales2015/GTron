using GTron.Engine.Characters;

namespace GTron.Engine.Animation;

public sealed class AnimationClip
{
	private readonly Dictionary<string, JointTrack> _tracks = [];

	public float Duration { get; }

	public AnimationClip(float duration)
	{
		if (!float.IsFinite(duration) || duration <= 0f)
		{
			throw new ArgumentOutOfRangeException(
				nameof(duration),
				duration,
				"La duración de la animación debe ser positiva y finita.");
		}

		Duration = duration;
	}

	public AnimationClip AddKeyframe(string jointName, float time, float angleDegrees)
	{
		if (!_tracks.TryGetValue(jointName, out JointTrack? track))
		{
			track = new JointTrack();
			_tracks.Add(jointName, track);
		}

		track.Add(time, angleDegrees);

		return this;
	}

	public void Apply(CharacterRig rig, float time)
	{
		foreach ((string jointName, JointTrack track) in _tracks)
		{
			rig.SetJointAngleDegrees(jointName,	track.Evaluate(time));
		}
	}

	private sealed class JointTrack
	{
		private readonly List<Keyframe> _keyframes = [];

		public void Add(float time, float angleDegrees)
		{
			_keyframes.Add(new Keyframe(time, angleDegrees));
			_keyframes.Sort((a, b) => a.Time.CompareTo(b.Time));
		}

		public float Evaluate(float time)
		{
			if (_keyframes.Count == 0)
			{
				return 0f;
			}

			if (_keyframes.Count == 1 || time <= _keyframes[0].Time)
			{
				return _keyframes[0].AngleDegrees;
			}

			for (int index = 1; index < _keyframes.Count; index++)
			{
				Keyframe next = _keyframes[index];

				if (time <= next.Time)
				{
					Keyframe previous = _keyframes[index - 1];

					float progress = (time - previous.Time) / (next.Time - previous.Time);

					return previous.AngleDegrees + (next.AngleDegrees - previous.AngleDegrees) * progress;
				}
			}

			return _keyframes[^1].AngleDegrees;
		}

		private readonly record struct Keyframe(
			float Time,
			float AngleDegrees
			);
	}
}