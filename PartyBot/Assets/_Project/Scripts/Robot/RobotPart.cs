using PartyBot.Parts;
using UnityEngine;

namespace PartyBot.Robot
{
    /// <summary>Una pieza montada en un robot. Sus celdas son hijos con SpriteRenderer y BoxCollider2D.</summary>
    public class RobotPart : MonoBehaviour
    {
        public PartDefinition Definition { get; private set; }
        /// <summary>Índice de la pieza en <see cref="RobotData.parts"/>.</summary>
        public int Index { get; private set; }
        public Vector2Int Cell { get; private set; }
        public int Rotation { get; private set; }

        internal void Init(PartDefinition definition, int index, PlacedPart placed)
        {
            Definition = definition;
            Index = index;
            Cell = placed.Cell;
            Rotation = placed.rotation;
        }
    }
}
