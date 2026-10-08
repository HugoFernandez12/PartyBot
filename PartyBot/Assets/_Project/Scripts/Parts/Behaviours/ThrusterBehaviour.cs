using PartyBot.Robot;
using UnityEngine;

namespace PartyBot.Parts
{
    /// <summary>Mientras se mantiene Boost, empuja hacia donde apunta. Si está descentrado, hace girar al robot.</summary>
    public class ThrusterBehaviour : PartBehaviour
    {
        static readonly Color FiringColor = new(1f, 0.95f, 0.6f);

        SpriteRenderer[] renderers;
        Color[] baseColors;
        bool firing;

        protected override void OnInit()
        {
            renderers = GetComponentsInChildren<SpriteRenderer>();
            baseColors = new Color[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
                baseColors[i] = renderers[i].color;
        }

        public override void Tick(RobotCommand command)
        {
            if (command.Boost)
                Body.Rigidbody.AddForceAtPosition((Vector2)transform.up * Power, transform.position);

            if (command.Boost != firing)
            {
                firing = command.Boost;
                for (int i = 0; i < renderers.Length; i++)
                    renderers[i].color = firing ? FiringColor : baseColors[i];
            }
        }
    }
}
