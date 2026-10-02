using System.Numerics;
using System.Text.Json;
using GTron.Engine.Resources;

namespace GTron.Engine.Characters
{

	public static class CharacterRigLoader
	{
		public static CharacterRig Load(AssetManager assets, string rigRelativePath)
		{
			string json = assets.LoadRigText(rigRelativePath);

			RigDefinition definition =
				JsonSerializer.Deserialize<RigDefinition>(
					json,
					new JsonSerializerOptions
					{
						PropertyNameCaseInsensitive = true
					})
				?? throw new InvalidOperationException(
					$"No se pudo leer el rig '{rigRelativePath}'.");

			ValidateRigDefinition(definition);

			var parts = new Dictionary<string, RigidPart>(
				StringComparer.Ordinal);

			foreach (RigPartDefinition partDefinition in definition.Parts)
			{
				var part = new RigidPart(
					partDefinition.Name,
					assets.LoadModel(partDefinition.Model),
					ToVector3(partDefinition.RestPosition),
					partDefinition.JointAxis is null ? Vector3.Zero : ToVector3(partDefinition.JointAxis),
					partDefinition.MinAngleDegrees,
					partDefinition.MaxAngleDegrees);

				if (!parts.TryAdd(part.Name, part))
				{
					throw new InvalidOperationException(
						$"El rig contiene dos piezas llamadas '{part.Name}'.");
				}
			}

			var roots = new List<RigidPart>();

			foreach (RigPartDefinition partDefinition in definition.Parts)
			{
				RigidPart part = parts[partDefinition.Name];

				if (string.IsNullOrWhiteSpace(partDefinition.Parent))
				{
					roots.Add(part);
					continue;
				}

				if (!parts.TryGetValue(
						partDefinition.Parent,
						out RigidPart? parent))
				{
					throw new InvalidOperationException(
						$"La pieza '{part.Name}' referencia al padre " +
						$"inexistente '{partDefinition.Parent}'.");
				}

				part.SetParent(parent);
			}

			foreach (RigidPart part in parts.Values)
			{
				part.CalculateLocalPosition();
			}

			var rig = new CharacterRig(parts, roots);

			rig.UpdateTransforms();

			return rig;
		}

		private static void ValidateDefinition(RigPartDefinition definition)
		{
			if (string.IsNullOrWhiteSpace(definition.Name))
			{
				throw new InvalidOperationException(
					"Cada pieza del rig necesita un nombre.");
			}

			if (string.IsNullOrWhiteSpace(definition.Model))
			{
				throw new InvalidOperationException(
					$"La pieza '{definition.Name}' no tiene modelo.");
			}

			if (definition.RestPosition is null || definition.RestPosition.Length != 3)
			{
				throw new InvalidOperationException(
					$"restPosition de '{definition.Name}' debe tener 3 valores.");
			}

			if (definition.JointAxis is not null &&
				definition.JointAxis.Length != 3)
			{
				throw new InvalidOperationException(
					$"jointAxis de '{definition.Name}' debe tener 3 valores.");
			}

			if (definition.MinAngleDegrees.HasValue &&
				definition.MaxAngleDegrees.HasValue &&
				definition.MinAngleDegrees > definition.MaxAngleDegrees)
			{
				throw new InvalidOperationException(
					$"Los límites de '{definition.Name}' son inválidos.");
			}
		}

		private static void ValidateRigDefinition(RigDefinition definition)
		{
			if (definition.Parts is null || definition.Parts.Count == 0)
			{
				throw new InvalidOperationException("El rig debe contener al menos una pieza.");
			}

			var definitionsByName = new Dictionary<string, RigPartDefinition>(
				StringComparer.Ordinal);

			foreach (RigPartDefinition part in definition.Parts)
			{
				if (part is null)
				{
					throw new InvalidOperationException("El rig contiene una pieza nula.");
				}

				ValidateDefinition(part);

				if (!definitionsByName.TryAdd(part.Name, part))
				{
					throw new InvalidOperationException($"El rig contiene dos piezas llamadas '{part.Name}'.");
				}
			}

			int rootCount = 0;

			foreach (RigPartDefinition part in definition.Parts)
			{
				if (string.IsNullOrWhiteSpace(part.Parent))
				{
					rootCount++;
					continue;
				}

				if (part.Parent == part.Name)
				{
					throw new InvalidOperationException($"La pieza '{part.Name}' no puede ser su propio padre.");
				}

				if (!definitionsByName.ContainsKey(part.Parent))
				{
					throw new InvalidOperationException(
						$"La pieza '{part.Name}' referencia al padre " +
						$"inexistente '{part.Parent}'.");
				}
			}

			if (rootCount == 0)
			{
				throw new InvalidOperationException("El rig debe contener al menos una pieza raíz.");
			}

			var validated = new HashSet<string>(StringComparer.Ordinal);
			var currentPath = new HashSet<string>(StringComparer.Ordinal);

			foreach (RigPartDefinition part in definition.Parts)
			{
				currentPath.Clear();
				RigPartDefinition current = part;

				while (!validated.Contains(current.Name))
				{
					if (!currentPath.Add(current.Name))
					{
						throw new InvalidOperationException(
							$"El rig contiene un ciclo de parentesco " +
							$"que incluye la pieza '{current.Name}'.");
					}

					if (string.IsNullOrWhiteSpace(current.Parent))
					{
						break;
					}

					current = definitionsByName[current.Parent];
				}

				validated.UnionWith(currentPath);
			}
		}

		private static Vector3 ToVector3(float[] values)
		{
			return new Vector3(values[0], values[1], values[2]);
		}
	}
}