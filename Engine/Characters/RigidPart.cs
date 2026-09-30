
using System.Numerics;
using Raylib_cs;

namespace GTron.Engine.Characters
{

	public sealed class RigidPart
	{
		private readonly List<RigidPart> _children = [];

		private float _jointAngleRadians;

		public string Name { get; }

		public Model Model;

		public Vector3 RestWorldPosition { get; }

		public Vector3 LocalPosition { get; private set; }

		public Vector3 JointAxis { get; }

		public RigidPart? Parent { get; private set; }

		public Matrix4x4 WorldTransform { get; private set; }

		public IReadOnlyList<RigidPart> Children => _children;

		public float? MinAngleDegrees { get; }

		public float? MaxAngleDegrees { get; }

		public RigidPart(
			string name,
			Model model,
			Vector3 restWorldPosition,
			Vector3 jointAxis,
			float? minAngleDegrees,
			float? maxAngleDegrees)
		{
			Name = name;
			Model = model;
			RestWorldPosition = restWorldPosition;
			JointAxis = jointAxis;
			MinAngleDegrees = minAngleDegrees;
			MaxAngleDegrees = maxAngleDegrees;
		}

		internal void ApplyShader(ref Shader shader)
		{
			for (int materialIndex = 0;
				 materialIndex < Model.MaterialCount;
				 materialIndex++)
			{
				Raylib.SetMaterialShader(
					ref Model,
					materialIndex,
					ref shader);
			}
		}

		internal void CalculateLocalPosition()
		{
			LocalPosition = Parent is null
				? RestWorldPosition
				: RestWorldPosition - Parent.RestWorldPosition;
		}

		internal void Draw()
		{
			Model model = Model;

			model.Transform = Matrix4x4.Transpose(WorldTransform);

			Raylib.DrawModel(
				model,
				Vector3.Zero,
				1f,
				Color.White);

			foreach (RigidPart child in _children)
			{
				child.Draw();
			}
		}

		internal void DrawWireframe(Color color)
		{
			Model model = Model;

			model.Transform = Matrix4x4.Transpose(WorldTransform);

			Raylib.DrawModelWires(
				model,
				Vector3.Zero,
				1f,
				color);

			foreach (RigidPart child in _children)
			{
				child.DrawWireframe(color);
			}
		}

		public float SetJointAngleDegrees(float degrees)
		{
			if (MinAngleDegrees.HasValue)
			{
				degrees = MathF.Max(degrees, MinAngleDegrees.Value);
			}

			if (MaxAngleDegrees.HasValue)
			{
				degrees = MathF.Min(degrees, MaxAngleDegrees.Value);
			}

			_jointAngleRadians = degrees * Raylib.DEG2RAD;

			return degrees;
		}

		internal void SetParent(RigidPart parent)
		{
			Parent = parent;
			parent._children.Add(this);

		}
		internal void UpdateWorldTransform(Matrix4x4 parentWorldTransform)
		{
			Quaternion rotation = JointAxis.LengthSquared() == 0f
				? Quaternion.Identity
				: Quaternion.CreateFromAxisAngle(
					Vector3.Normalize(JointAxis),
					_jointAngleRadians);

			Matrix4x4 localTransform =
				Matrix4x4.CreateFromQuaternion(rotation) *
				Matrix4x4.CreateTranslation(LocalPosition);

			WorldTransform = localTransform * parentWorldTransform;

			foreach (RigidPart child in _children)
			{
				child.UpdateWorldTransform(WorldTransform);
			}
		}
	}
}