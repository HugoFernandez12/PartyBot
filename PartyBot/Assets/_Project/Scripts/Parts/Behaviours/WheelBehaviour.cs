using PartyBot.Robot;
using UnityEngine;

namespace PartyBot.Parts
{
    /// <summary>
    /// Tracción tipo tanque: empuja hacia donde apunta la rueda. Para girar, las ruedas de un lado
    /// del centro de masas empujan hacia delante y las del otro hacia atrás. Además frena el
    /// deslizamiento lateral, como el agarre de un neumático.
    /// </summary>
    public class WheelBehaviour : PartBehaviour
    {
        // Se ajustará en la tarea 1.8.
        const float Grip = 15f;
        // Ruedas casi alineadas con el centro de masas no ayudan a girar.
        const float CenterDeadZone = 0.05f;

        public override void Tick(RobotCommand command)
        {
            var rb = Body.Rigidbody;
            Vector2 position = transform.position;
            Vector2 forward = transform.up;
            Vector2 right = transform.right;

            float offset = Vector2.Dot(position - rb.worldCenterOfMass, Body.transform.right);
            float side = Mathf.Abs(offset) < CenterDeadZone ? 0f : Mathf.Sign(offset);

            float throttle = command.Move.y;
            float turnLeft = -command.Move.x;
            float drive = Mathf.Clamp(throttle + turnLeft * side, -1f, 1f);
            rb.AddForceAtPosition(forward * (drive * Power), position);

            float lateralSpeed = Vector2.Dot(rb.GetPointVelocity(position), right);
            rb.AddForceAtPosition(-right * (lateralSpeed * Grip), position);
        }
    }
}
