using System;
using System.Collections.Generic;
using UnityEngine;

namespace PartyBot.Robot
{
    /// <summary>Una pieza colocada en la rejilla de montaje.</summary>
    [Serializable]
    public class PlacedPart
    {
        public string partId;
        public int x;
        public int y;
        [Tooltip("Cuartos de vuelta en sentido antihorario (0..3).")]
        public int rotation;

        public PlacedPart() { }

        public PlacedPart(string partId, Vector2Int cell, int rotation = 0)
        {
            this.partId = partId;
            Cell = cell;
            this.rotation = rotation;
        }

        public Vector2Int Cell
        {
            get => new(x, y);
            set { x = value.x; y = value.y; }
        }
    }

    /// <summary>
    /// Descripción de un robot: qué piezas lleva y dónde. Es lo que se guarda en JSON,
    /// lo que valida <see cref="RobotValidator"/> y lo que se instanciará en la arena.
    /// </summary>
    [Serializable]
    public class RobotData
    {
        public const int CurrentVersion = 1;

        public int version = CurrentVersion;
        public string name = string.Empty;
        public List<PlacedPart> parts = new();

        public string ToJson(bool prettyPrint = true) => JsonUtility.ToJson(this, prettyPrint);

        /// <exception cref="ArgumentException">Si el JSON está vacío o mal formado.</exception>
        public static RobotData FromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentException("El JSON del robot está vacío.", nameof(json));

            var data = JsonUtility.FromJson<RobotData>(json);
            data.parts ??= new List<PlacedPart>();
            data.name ??= string.Empty;
            return data;
        }
    }
}
