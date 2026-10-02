using Raylib_cs;
using System.Numerics;

namespace GTron.Engine.Characters
{

	public sealed class CharacterRig
	{
		public Vector3 Position { get; set; }
		public float FacingAngleDegrees { get; set; }
		public Vector3 Scale { get; set; } = Vector3.One;
		private readonly Dictionary<string, RigidPart> _parts;
		private readonly List<RigidPart> _roots;
		public IEnumerable<RigidPart> Parts => _parts.Values;

		internal CharacterRig(Dictionary<string, RigidPart> parts, List<RigidPart> roots)
		{
			_parts = parts;
			_roots = roots;
		}

		public void ApplyShader(ref Shader shader)
		{
			foreach (RigidPart part in _parts.Values)
			{
				part.ApplyShader(ref shader);
			}
		}

		public void Draw()
		{
			foreach (RigidPart root in _roots)
			{
				root.Draw();
			}
		}

		public void DrawWireframe(Color color)
		{
			foreach (RigidPart root in _roots)
			{
				root.DrawWireframe(color);
			}
		}

		private RigidPart GetPart(string partName)
		{
			if (!_parts.TryGetValue(partName, out RigidPart? part))
			{
				throw new KeyNotFoundException(
					$"El rig no contiene la pieza '{partName}'.");
			}

			return part;
		}

		public void ResetPose()
		{
			foreach (RigidPart part in _parts.Values)
			{
				part.SetJointAngleDegrees(0f);
			}
		}

		public float SetJointAngleDegrees(string partName, float degrees)
		{
			return GetPart(partName).SetJointAngleDegrees(degrees);
		}

		public void UpdateTransforms()
		{
			float facingRadians = FacingAngleDegrees * Raylib.DEG2RAD;

			Matrix4x4 characterTransform =
				Matrix4x4.CreateScale(Scale) *
				Matrix4x4.CreateRotationY(facingRadians) *
				Matrix4x4.CreateTranslation(Position);

			foreach (RigidPart root in _roots)
			{
				root.UpdateWorldTransform(characterTransform);
			}
		}
	}
}