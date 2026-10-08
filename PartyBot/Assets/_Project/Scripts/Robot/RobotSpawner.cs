using PartyBot.Controls;
using PartyBot.Parts;
using UnityEngine;

namespace PartyBot.Robot
{
    /// <summary>
    /// Monta un robot al empezar la escena, en la posición de este objeto.
    /// El robot se edita en el Inspector (lista de piezas). Sirve para probar hasta que exista el editor de montaje.
    /// </summary>
    public class RobotSpawner : MonoBehaviour
    {
        [SerializeField] PartCatalog catalog;
        [SerializeField] ValidationRules rules = new();
        [SerializeField] RobotData robot = SampleRobot();
        [SerializeField, Tooltip("Controlarlo con teclado (WASD + Espacio) o mando.")] bool playerControlled = true;

        public RobotBody Spawned { get; private set; }

        void Start()
        {
            if (catalog == null)
            {
                Debug.LogError("[RobotSpawner] Falta asignar el catálogo de piezas.", this);
                return;
            }

            var result = RobotValidator.Validate(robot, catalog, rules);
            if (!result.IsValid)
            {
                Debug.LogError($"[RobotSpawner] El robot no cumple las reglas de montaje:\n{result}", this);
                return;
            }

            Spawned = RobotAssembler.Build(robot, catalog, transform.position);
            if (Spawned != null && playerControlled)
                Spawned.gameObject.AddComponent<LocalRobotInput>();
        }

        /// <summary>Coche 3x3: ruedas en las esquinas, núcleo en el centro y propulsor detrás.</summary>
        static RobotData SampleRobot()
        {
            var data = new RobotData { name = "Robot de prueba" };
            data.parts.Add(new PlacedPart("core", new Vector2Int(4, 4)));
            data.parts.Add(new PlacedPart("block", new Vector2Int(3, 4)));
            data.parts.Add(new PlacedPart("block", new Vector2Int(5, 4)));
            data.parts.Add(new PlacedPart("block", new Vector2Int(4, 5)));
            data.parts.Add(new PlacedPart("thruster", new Vector2Int(4, 3)));
            data.parts.Add(new PlacedPart("wheel", new Vector2Int(3, 5)));
            data.parts.Add(new PlacedPart("wheel", new Vector2Int(5, 5)));
            data.parts.Add(new PlacedPart("wheel", new Vector2Int(3, 3)));
            data.parts.Add(new PlacedPart("wheel", new Vector2Int(5, 3)));
            return data;
        }
    }
}
