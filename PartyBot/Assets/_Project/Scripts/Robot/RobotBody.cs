using System.Collections.Generic;
using UnityEngine;

namespace PartyBot.Robot
{
    /// <summary>
    /// Raíz de un robot montado: un único Rigidbody2D (cuerpo compuesto) con las piezas como hijos.
    /// Lo crea <see cref="RobotAssembler"/>.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class RobotBody : MonoBehaviour
    {
        readonly List<RobotPart> parts = new();

        public Rigidbody2D Rigidbody { get; private set; }
        public RobotData Data { get; private set; }
        public RobotTuning Tuning { get; private set; }
        public RobotPart Core { get; private set; }
        public IReadOnlyList<RobotPart> Parts => parts;

        internal void Init(RobotData data, Rigidbody2D body, RobotTuning tuning)
        {
            Data = data;
            Rigidbody = body;
            Tuning = tuning != null ? tuning : RobotTuning.Defaults;
            ApplyTuning();
        }

        void FixedUpdate() => ApplyTuning();

        // Cada paso, para que los cambios en el Inspector se noten al momento.
        void ApplyTuning()
        {
            Rigidbody.linearDamping = Tuning.LinearDamping;
            Rigidbody.angularDamping = Tuning.AngularDamping;
        }

        internal void AddPart(RobotPart part)
        {
            parts.Add(part);
            if (part.Definition.IsCore)
                Core = part;
        }

        /// <summary>Masa = suma de las piezas; centro de masas = media de sus celdas ponderada por masa.</summary>
        public void RecalculateMass()
        {
            float totalMass = 0f;
            Vector2 weighted = Vector2.zero;

            foreach (var part in parts)
            {
                var colliders = part.GetComponentsInChildren<BoxCollider2D>();
                if (colliders.Length == 0)
                    continue;

                float massPerCell = part.Definition.Mass / colliders.Length;
                foreach (var collider in colliders)
                    weighted += (Vector2)transform.InverseTransformPoint(collider.transform.position) * massPerCell;
                totalMass += part.Definition.Mass;
            }

            if (totalMass <= 0f)
                return;

            Rigidbody.mass = totalMass;
            Rigidbody.centerOfMass = weighted / totalMass;
        }
    }
}
