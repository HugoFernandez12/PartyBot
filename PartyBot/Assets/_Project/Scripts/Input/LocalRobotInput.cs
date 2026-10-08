using PartyBot.Robot;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PartyBot.Controls
{
    /// <summary>
    /// Controla un robot con teclado o cualquier mando conectado.
    /// Provisional: en la Fase 4 se cambia por PlayerInput para asignar un dispositivo a cada jugador.
    /// </summary>
    [RequireComponent(typeof(RobotController))]
    public class LocalRobotInput : MonoBehaviour
    {
        RobotController controller;
        InputAction move;
        InputAction boost;

        void Awake()
        {
            controller = GetComponent<RobotController>();

            move = new InputAction("Move", InputActionType.Value, expectedControlType: "Vector2");
            move.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w").With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a").With("Right", "<Keyboard>/d");
            move.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/upArrow").With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/leftArrow").With("Right", "<Keyboard>/rightArrow");
            move.AddBinding("<Gamepad>/leftStick");

            boost = new InputAction("Boost", InputActionType.Button);
            boost.AddBinding("<Keyboard>/space");
            boost.AddBinding("<Gamepad>/rightTrigger");
        }

        void OnEnable()
        {
            move.Enable();
            boost.Enable();
        }

        void OnDisable()
        {
            move.Disable();
            boost.Disable();
            controller.Command = default;
        }

        void OnDestroy()
        {
            move.Dispose();
            boost.Dispose();
        }

        void Update()
        {
            controller.Command = new RobotCommand
            {
                Move = Vector2.ClampMagnitude(move.ReadValue<Vector2>(), 1f),
                Boost = boost.IsPressed(),
            };
        }
    }
}
