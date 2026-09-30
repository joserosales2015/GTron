namespace GTron.Engine.Characters;

public sealed class RigDefinition
{
	public List<RigPartDefinition> Parts { get; set; } = [];

	public float? MinAngleDegrees { get; set; }

	public float? MaxAngleDegrees { get; set; }
}

public sealed class RigPartDefinition
{
	public string Name { get; set; } = "";

	public string Model { get; set; } = "";

	public string? Parent { get; set; }

	public float[] RestPosition { get; set; } = [];

	public float[]? JointAxis { get; set; }

	public float? MinAngleDegrees { get; set; }

	public float? MaxAngleDegrees { get; set; }
}