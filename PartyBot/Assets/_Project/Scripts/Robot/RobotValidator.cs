using System;
using System.Collections.Generic;
using System.Linq;
using PartyBot.Core;
using PartyBot.Parts;
using UnityEngine;

namespace PartyBot.Robot
{
    public enum ValidationErrorType
    {
        UnknownPart,
        NoCore,
        MultipleCores,
        Overlap,
        OutOfBounds,
        Disconnected,
        OverBudget,
    }

    public readonly struct ValidationError
    {
        /// <summary>Índice en <see cref="RobotData.parts"/>, o -1 si el error es del robot entero.</summary>
        public readonly int PartIndex;
        public readonly ValidationErrorType Type;
        public readonly string Message;

        public ValidationError(ValidationErrorType type, int partIndex, string message)
        {
            Type = type;
            PartIndex = partIndex;
            Message = message;
        }

        public override string ToString() => PartIndex >= 0 ? $"[{Type} #{PartIndex}] {Message}" : $"[{Type}] {Message}";
    }

    public class ValidationResult
    {
        readonly List<ValidationError> errors;

        public ValidationResult(List<ValidationError> errors) => this.errors = errors;

        public IReadOnlyList<ValidationError> Errors => errors;
        public bool IsValid => errors.Count == 0;

        public bool Has(ValidationErrorType type) => errors.Any(e => e.Type == type);
        public bool Has(ValidationErrorType type, int partIndex) => errors.Any(e => e.Type == type && e.PartIndex == partIndex);
        public bool PartHasErrors(int partIndex) => errors.Any(e => e.PartIndex == partIndex);

        public override string ToString() => IsValid ? "Robot válido" : string.Join("\n", errors);
    }

    [Serializable]
    public class ValidationRules
    {
        [Min(1)] public int gridWidth = 9;
        [Min(1)] public int gridHeight = 9;
        [Min(0), Tooltip("Coste máximo total de las piezas. 0 = sin límite.")]
        public int maxCost;
    }

    /// <summary>
    /// Reglas de montaje: piezas conocidas, un solo núcleo, sin solapes, dentro de la rejilla,
    /// todo conectado al núcleo por caras y dentro del presupuesto.
    /// </summary>
    public static class RobotValidator
    {
        public static ValidationResult Validate(RobotData robot, PartCatalog catalog, ValidationRules rules)
        {
            var errors = new List<ValidationError>();
            var bounds = new RectInt(0, 0, rules.gridWidth, rules.gridHeight);
            var occupied = new Dictionary<Vector2Int, int>();
            var partCells = new List<Vector2Int>[robot.parts.Count];
            var flagged = new HashSet<int>(); // piezas con errores propios: no se comprueba su conexión
            var cores = new List<int>();
            int totalCost = 0;

            for (int i = 0; i < robot.parts.Count; i++)
            {
                var placed = robot.parts[i];
                if (placed == null || !catalog.TryGet(placed.partId, out var part))
                {
                    errors.Add(new ValidationError(ValidationErrorType.UnknownPart, i, $"Pieza desconocida '{placed?.partId}'."));
                    flagged.Add(i);
                    continue;
                }

                totalCost += part.Cost;
                if (part.IsCore)
                    cores.Add(i);

                partCells[i] = part.GetCells(placed.Cell, placed.rotation).ToList();
                bool outOfBounds = false;
                int overlapWith = -1;

                foreach (var cell in partCells[i])
                {
                    if (!bounds.Contains(cell))
                        outOfBounds = true;
                    if (occupied.TryGetValue(cell, out int other))
                        overlapWith = other;
                    else
                        occupied[cell] = i;
                }

                if (outOfBounds)
                {
                    errors.Add(new ValidationError(ValidationErrorType.OutOfBounds, i, $"'{part.DisplayName}' se sale de la rejilla."));
                    flagged.Add(i);
                }
                if (overlapWith >= 0)
                {
                    errors.Add(new ValidationError(ValidationErrorType.Overlap, i, $"'{part.DisplayName}' se solapa con la pieza #{overlapWith}."));
                    flagged.Add(i);
                }
            }

            if (cores.Count == 0)
                errors.Add(new ValidationError(ValidationErrorType.NoCore, -1, "El robot necesita un núcleo."));
            for (int c = 1; c < cores.Count; c++)
                errors.Add(new ValidationError(ValidationErrorType.MultipleCores, cores[c], "Solo puede haber un núcleo."));

            // La conexión solo tiene sentido con exactamente un núcleo.
            if (cores.Count == 1)
            {
                var reached = FloodFill(occupied, partCells[cores[0]]);
                for (int i = 0; i < robot.parts.Count; i++)
                {
                    if (partCells[i] == null || flagged.Contains(i))
                        continue;
                    if (!partCells[i].Any(reached.Contains))
                        errors.Add(new ValidationError(ValidationErrorType.Disconnected, i, "Esta pieza no está conectada al núcleo."));
                }
            }

            if (rules.maxCost > 0 && totalCost > rules.maxCost)
                errors.Add(new ValidationError(ValidationErrorType.OverBudget, -1, $"Coste {totalCost} supera el máximo de {rules.maxCost}."));

            return new ValidationResult(errors);
        }

        static HashSet<Vector2Int> FloodFill(Dictionary<Vector2Int, int> occupied, IEnumerable<Vector2Int> start)
        {
            var reached = new HashSet<Vector2Int>(start);
            var queue = new Queue<Vector2Int>(reached);

            while (queue.Count > 0)
            {
                var cell = queue.Dequeue();
                foreach (var dir in GridMath.Neighbours)
                {
                    var next = cell + dir;
                    if (occupied.ContainsKey(next) && reached.Add(next))
                        queue.Enqueue(next);
                }
            }

            return reached;
        }
    }
}
