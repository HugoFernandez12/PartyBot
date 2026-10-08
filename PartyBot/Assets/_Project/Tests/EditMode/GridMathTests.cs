using System.Linq;
using NUnit.Framework;
using PartyBot.Core;
using PartyBot.Parts;
using UnityEngine;

namespace PartyBot.Tests
{
    public class GridMathTests
    {
        [TestCase(0, 1, 0)]
        [TestCase(1, 0, 1)]
        [TestCase(2, -1, 0)]
        [TestCase(3, 0, -1)]
        [TestCase(4, 1, 0)]
        [TestCase(-1, 0, -1)]
        public void Rotate_RightCell_TurnsCounterClockwise(int quarterTurns, int expectedX, int expectedY)
        {
            Assert.AreEqual(new Vector2Int(expectedX, expectedY), GridMath.Rotate(Vector2Int.right, quarterTurns));
        }

        [TestCase(0, 0)]
        [TestCase(5, 1)]
        [TestCase(-1, 3)]
        [TestCase(-6, 2)]
        public void NormalizeRotation_WrapsToZeroToThree(int input, int expected)
        {
            Assert.AreEqual(expected, GridMath.NormalizeRotation(input));
        }

        [Test]
        public void PartDefinition_GetCells_AppliesRotationAndOrigin()
        {
            var part = PartDefinition.Create("bar", PartCategory.Structure, new[] { Vector2Int.zero, Vector2Int.right });
            try
            {
                var cells = part.GetCells(new Vector2Int(3, 3), 1).ToArray();
                CollectionAssert.AreEquivalent(new[] { new Vector2Int(3, 3), new Vector2Int(3, 4) }, cells);
            }
            finally
            {
                Object.DestroyImmediate(part);
            }
        }
    }
}
