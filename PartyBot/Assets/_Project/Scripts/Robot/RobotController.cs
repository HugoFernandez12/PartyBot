using System.Collections.Generic;
using PartyBot.Parts;
using UnityEngine;

namespace PartyBot.Robot
{
    /// <summary>Lo que el jugador pide al robot en este momento. No sabe de teclados ni mandos.</summary>
    public struct RobotCommand
    {
        /// <summary>x: girar (-1 izquierda, 1 derecha). y: avanzar (1) o retroceder (-1).</summary>
        public Vector2 Move;
        public bool Boost;
    }

    /// <summary>
    /// Reparte el <see cref="Command"/> entre las piezas activas en cada paso de física.
    /// Quien rellene Command (teclado, mando, IA, red) da igual.
    /// </summary>
    [RequireComponent(typeof(RobotBody))]
    public class RobotController : MonoBehaviour
    {
        readonly List<PartBehaviour> behaviours = new();

        public RobotCommand Command { get; set; }
        public IReadOnlyList<PartBehaviour> Behaviours => behaviours;

        internal void Register(PartBehaviour behaviour) => behaviours.Add(behaviour);

        void FixedUpdate()
        {
            var command = Command;
            foreach (var behaviour in behaviours)
                if (behaviour != null && behaviour.isActiveAndEnabled)
                    behaviour.Tick(command);
        }
    }
}
