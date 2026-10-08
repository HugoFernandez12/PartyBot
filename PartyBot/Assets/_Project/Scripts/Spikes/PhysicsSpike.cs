using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PartyBot.Spikes
{
    /// <summary>
    /// Prueba de física (tarea 1.1). Monta el mismo robot de dos formas para compararlas:
    /// A = una pieza por Rigidbody2D unidas con FixedJoint2D.
    /// B = un único Rigidbody2D con un collider por pieza; al soltar una pieza se crea un cuerpo nuevo.
    /// Código desechable: se borra cuando elijamos una opción.
    /// </summary>
    public class PhysicsSpike : MonoBehaviour
    {
        [Serializable]
        public class Settings
        {
            [Header("Movimiento (se aplica en vivo)")]
            public float thrustForce = 40f;
            public float turnTorque = 25f;

            [Header("Física (se aplica al reiniciar con R)")]
            public float partMass = 1f;
            public float linearDamping = 2f;
            public float angularDamping = 4f;

            [Header("Solo robot A: joints")]
            [Tooltip("0 = unión rígida. Valores > 0 la hacen elástica.")]
            public float jointFrequency = 0f;
            [Range(0f, 1f)] public float jointDampingRatio = 1f;

            [Header("Piezas sueltas")]
            public float detachImpulse = 4f;
        }

        struct Controls
        {
            public Key forward, back, left, right, detach;
        }

        // Celdas del robot de prueba; la primera es el núcleo.
        static readonly Vector2Int[] Layout =
        {
            new(0, 0), new(1, 0), new(-1, 0), new(0, 1), new(0, -1), new(1, 1), new(-1, 1), new(0, -2),
        };

        static readonly Controls ControlsA = new() { forward = Key.W, back = Key.S, left = Key.A, right = Key.D, detach = Key.E };
        static readonly Controls ControlsB = new() { forward = Key.UpArrow, back = Key.DownArrow, left = Key.LeftArrow, right = Key.RightArrow, detach = Key.RightShift };

        [SerializeField] Sprite partSprite;
        [SerializeField] Settings settings = new();

        SpikeRobot robotA, robotB;
        Transform debris;
        Vector2 inputA, inputB; // x = giro, y = empuje

        void Start() => Spawn();

        void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null)
                return;

            if (keyboard[Key.R].wasPressedThisFrame)
            {
                Despawn();
                Spawn();
                return;
            }

            inputA = ReadInput(keyboard, ControlsA);
            inputB = ReadInput(keyboard, ControlsB);

            if (keyboard[ControlsA.detach].wasPressedThisFrame) robotA.DetachNext();
            if (keyboard[ControlsB.detach].wasPressedThisFrame) robotB.DetachNext();
        }

        void FixedUpdate()
        {
            robotA?.Drive(inputA.y, inputA.x);
            robotB?.Drive(inputB.y, inputB.x);
        }

        void OnGUI()
        {
            GUI.Label(new Rect(10, 10, 900, 80),
                "Robot A (azul, joints): WASD mover · E soltar pieza\n" +
                "Robot B (rojo, cuerpo único): flechas mover · Shift derecho soltar pieza\n" +
                "R reiniciar");
        }

        static Vector2 ReadInput(Keyboard keyboard, Controls c)
        {
            float thrust = (keyboard[c.forward].isPressed ? 1f : 0f) - (keyboard[c.back].isPressed ? 1f : 0f);
            float turn = (keyboard[c.left].isPressed ? 1f : 0f) - (keyboard[c.right].isPressed ? 1f : 0f);
            return new Vector2(turn, thrust);
        }

        void Spawn()
        {
            if (partSprite == null)
            {
                Debug.LogError("[PhysicsSpike] Falta asignar Part Sprite (Art/WhiteSquare).", this);
                enabled = false;
                return;
            }

            debris = new GameObject("Debris").transform;
            robotA = new JointRobot("Robot A (joints)", new Vector2(-6f, 0f), Layout, partSprite, new Color(0.3f, 0.55f, 0.95f), settings, debris);
            robotB = new CompoundRobot("Robot B (compuesto)", new Vector2(6f, 0f), Layout, partSprite, new Color(0.95f, 0.35f, 0.3f), settings, debris);
        }

        void Despawn()
        {
            robotA?.Destroy();
            robotB?.Destroy();
            if (debris != null)
                Destroy(debris.gameObject);
        }
    }
}
