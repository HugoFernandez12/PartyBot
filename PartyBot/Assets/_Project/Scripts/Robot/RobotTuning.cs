using UnityEngine;

namespace PartyBot.Robot
{
    /// <summary>
    /// Ajustes globales de conducción, compartidos por todos los robots. Se leen en cada paso de física,
    /// así que se pueden cambiar en el Inspector durante Play y el efecto es inmediato.
    /// Ojo: al ser un asset, los cambios hechos en Play se quedan al salir.
    /// </summary>
    [CreateAssetMenu(fileName = "RobotTuning", menuName = "PartyBot/Ajustes de robot", order = 2)]
    public class RobotTuning : ScriptableObject
    {
        [Header("Frenado")]
        [SerializeField, Min(0f), Tooltip("Cuánto frena el robot solo al soltar los controles. Más alto = para antes y velocidad máxima menor.")]
        float linearDamping = 2f;
        [SerializeField, Min(0f), Tooltip("Cuánto frena el giro. Más alto = gira menos y deja de girar antes.")]
        float angularDamping = 4f;

        [Header("Ruedas")]
        [SerializeField, Min(0f), Tooltip("Multiplica la potencia de todas las ruedas.")]
        float wheelPowerMultiplier = 1f;
        [SerializeField, Min(0f), Tooltip("Agarre lateral. 0 = patina como sobre hielo. Alto = va sobre raíles.")]
        float wheelGrip = 15f;

        [Header("Propulsores")]
        [SerializeField, Min(0f), Tooltip("Multiplica la potencia de todos los propulsores.")]
        float thrusterPowerMultiplier = 1f;

        static RobotTuning defaults;

        public float LinearDamping => linearDamping;
        public float AngularDamping => angularDamping;
        public float WheelPowerMultiplier => wheelPowerMultiplier;
        public float WheelGrip => wheelGrip;
        public float ThrusterPowerMultiplier => thrusterPowerMultiplier;

        /// <summary>Valores por defecto para cuando no se asigna ningún asset (p. ej. en tests).</summary>
        public static RobotTuning Defaults
        {
            get
            {
                if (defaults == null)
                {
                    defaults = CreateInstance<RobotTuning>();
                    defaults.hideFlags = HideFlags.HideAndDontSave;
                }
                return defaults;
            }
        }
    }
}
