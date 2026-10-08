using System.Collections.Generic;
using UnityEngine;

namespace PartyBot.Core
{
    /// <summary>Utilidades de la rejilla de montaje. Las rotaciones son cuartos de vuelta en sentido antihorario.</summary>
    public static class GridMath
    {
        /// <summary>Vecinos por cara (sin diagonales).</summary>
        public static readonly IReadOnlyList<Vector2Int> Neighbours = new[]
        {
            Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left,
        };

        /// <summary>Lleva cualquier número de cuartos de vuelta al rango 0..3.</summary>
        public static int NormalizeRotation(int quarterTurns) => ((quarterTurns % 4) + 4) % 4;

        /// <summary>Rota una celda alrededor del origen (0,0).</summary>
        public static Vector2Int Rotate(Vector2Int cell, int quarterTurns)
        {
            switch (NormalizeRotation(quarterTurns))
            {
                case 1: return new Vector2Int(-cell.y, cell.x);
                case 2: return new Vector2Int(-cell.x, -cell.y);
                case 3: return new Vector2Int(cell.y, -cell.x);
                default: return cell;
            }
        }
    }
}
