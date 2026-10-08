using PartyBot.Robot;
using UnityEngine;

namespace PartyBot.Parts
{
    /// <summary>
    /// Lógica de una pieza activa. <see cref="RobotController"/> llama a <see cref="Tick"/> en cada paso de física
    /// con los controles del jugador. Las fuerzas se aplican al Rigidbody2D del robot en la posición de la pieza.
    /// </summary>
    public abstract class PartBehaviour : MonoBehaviour
    {
        protected RobotPart Part { get; private set; }
        protected RobotBody Body { get; private set; }
        protected float Power => Part.Definition.Power;

        internal void Init(RobotPart part, RobotBody body)
        {
            Part = part;
            Body = body;
            OnInit();
        }

        protected virtual void OnInit() { }

        public abstract void Tick(RobotCommand command);
    }
}
