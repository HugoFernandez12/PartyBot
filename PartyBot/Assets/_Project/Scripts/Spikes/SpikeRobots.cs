using System.Collections.Generic;
using UnityEngine;

namespace PartyBot.Spikes
{
    /// <summary>Base común de los dos robots de la prueba de física.</summary>
    abstract class SpikeRobot
    {
        static readonly Color CoreColor = new(1f, 0.8f, 0.2f);
        const float DebrisDim = 0.5f;

        protected readonly PhysicsSpike.Settings settings;
        protected readonly Transform root;
        protected readonly Transform debris;

        protected SpikeRobot(string name, Vector2 position, PhysicsSpike.Settings settings, Transform debris)
        {
            this.settings = settings;
            this.debris = debris;
            root = new GameObject(name).transform;
            root.position = position;
        }

        /// <param name="thrust">-1..1, adelante/atrás.</param>
        /// <param name="turn">-1..1, positivo = izquierda.</param>
        public abstract void Drive(float thrust, float turn);

        /// <summary>Suelta la última pieza montada (siempre es una hoja, así el resto sigue conectado).</summary>
        public abstract void DetachNext();

        public void Destroy() => Object.Destroy(root.gameObject);

        protected static GameObject CreatePart(Transform parent, Vector2Int cell, Sprite sprite, Color color, bool isCore)
        {
            var go = new GameObject(isCore ? "Core" : $"Part {cell.x},{cell.y}");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = (Vector2)cell;

            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = isCore ? CoreColor : color;

            go.AddComponent<BoxCollider2D>();
            return go;
        }

        protected Rigidbody2D AddBody(GameObject go, float mass)
        {
            var rb = go.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.mass = mass;
            rb.linearDamping = settings.linearDamping;
            rb.angularDamping = settings.angularDamping;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            return rb;
        }

        protected void MarkAsDebris(GameObject part)
        {
            part.name += " (suelta)";
            var renderer = part.GetComponent<SpriteRenderer>();
            renderer.color *= new Color(DebrisDim, DebrisDim, DebrisDim, 1f);
        }

        /// <summary>Orden BFS desde el núcleo (layout[0]) y padre de cada celda en ese árbol.</summary>
        protected static List<(Vector2Int cell, Vector2Int? parent)> BuildTree(Vector2Int[] layout)
        {
            var remaining = new HashSet<Vector2Int>(layout);
            var result = new List<(Vector2Int, Vector2Int?)>();
            var queue = new Queue<Vector2Int>();

            remaining.Remove(layout[0]);
            result.Add((layout[0], null));
            queue.Enqueue(layout[0]);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                foreach (var dir in new[] { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down })
                {
                    var next = current + dir;
                    if (remaining.Remove(next))
                    {
                        result.Add((next, current));
                        queue.Enqueue(next);
                    }
                }
            }

            if (remaining.Count > 0)
                Debug.LogWarning($"[PhysicsSpike] {remaining.Count} celdas no conectan con el núcleo y se ignoran.");
            return result;
        }
    }

    /// <summary>Opción A: cada pieza es un Rigidbody2D, unida a su padre con FixedJoint2D. La fuerza se aplica al núcleo.</summary>
    class JointRobot : SpikeRobot
    {
        readonly List<Rigidbody2D> parts = new(); // en orden BFS; [0] = núcleo

        public JointRobot(string name, Vector2 position, Vector2Int[] layout, Sprite sprite, Color color,
            PhysicsSpike.Settings settings, Transform debris)
            : base(name, position, settings, debris)
        {
            var bodies = new Dictionary<Vector2Int, Rigidbody2D>();

            foreach (var (cell, parent) in BuildTree(layout))
            {
                var go = CreatePart(root, cell, sprite, color, parent == null);
                var rb = AddBody(go, settings.partMass);

                if (parent is Vector2Int parentCell)
                {
                    var joint = go.AddComponent<FixedJoint2D>();
                    joint.connectedBody = bodies[parentCell];
                    joint.frequency = settings.jointFrequency;
                    joint.dampingRatio = settings.jointDampingRatio;
                }

                bodies[cell] = rb;
                parts.Add(rb);
            }

            // Las piezas del mismo robot se tocan pero no deben chocar entre sí.
            SetSelfCollision(ignore: true);
        }

        Rigidbody2D Core => parts[0];

        public override void Drive(float thrust, float turn)
        {
            Core.AddForce((Vector2)Core.transform.up * (thrust * settings.thrustForce));
            Core.AddTorque(turn * settings.turnTorque);
        }

        public override void DetachNext()
        {
            if (parts.Count <= 1)
                return;

            var rb = parts[^1];
            SetSelfCollision(ignore: false);
            parts.RemoveAt(parts.Count - 1);
            SetSelfCollision(ignore: true);

            Object.Destroy(rb.GetComponent<FixedJoint2D>());
            rb.transform.SetParent(debris, true);
            MarkAsDebris(rb.gameObject);

            var away = ((Vector2)rb.transform.position - Core.position).normalized;
            rb.AddForce(away * settings.detachImpulse, ForceMode2D.Impulse);
        }

        void SetSelfCollision(bool ignore)
        {
            for (int i = 0; i < parts.Count; i++)
            for (int j = i + 1; j < parts.Count; j++)
                Physics2D.IgnoreCollision(parts[i].GetComponent<Collider2D>(), parts[j].GetComponent<Collider2D>(), ignore);
        }
    }

    /// <summary>Opción B: un único Rigidbody2D en la raíz; cada pieza es solo un collider hijo.</summary>
    class CompoundRobot : SpikeRobot
    {
        readonly Rigidbody2D body;
        readonly List<GameObject> parts = new(); // en orden BFS; [0] = núcleo

        public CompoundRobot(string name, Vector2 position, Vector2Int[] layout, Sprite sprite, Color color,
            PhysicsSpike.Settings settings, Transform debris)
            : base(name, position, settings, debris)
        {
            foreach (var (cell, parent) in BuildTree(layout))
                parts.Add(CreatePart(root, cell, sprite, color, parent == null));

            body = AddBody(root.gameObject, settings.partMass * parts.Count);
        }

        public override void Drive(float thrust, float turn)
        {
            body.AddForce((Vector2)root.up * (thrust * settings.thrustForce));
            body.AddTorque(turn * settings.turnTorque);
        }

        public override void DetachNext()
        {
            if (parts.Count <= 1)
                return;

            var part = parts[^1];
            parts.RemoveAt(parts.Count - 1);

            // La pieza conserva la velocidad que tenía como parte del robot.
            Vector2 velocity = body.GetPointVelocity(part.transform.position);
            float angularVelocity = body.angularVelocity;

            part.transform.SetParent(debris, true);
            body.mass = settings.partMass * parts.Count;
            MarkAsDebris(part);

            var rb = AddBody(part, settings.partMass);
            rb.linearVelocity = velocity;
            rb.angularVelocity = angularVelocity;

            var away = ((Vector2)part.transform.position - body.worldCenterOfMass).normalized;
            rb.AddForce(away * settings.detachImpulse, ForceMode2D.Impulse);
        }
    }
}
