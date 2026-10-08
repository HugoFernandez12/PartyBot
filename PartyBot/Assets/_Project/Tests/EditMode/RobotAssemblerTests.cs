using System.Text.RegularExpressions;
using NUnit.Framework;
using PartyBot.Parts;
using PartyBot.Robot;
using UnityEngine;
using UnityEngine.TestTools;

namespace PartyBot.Tests
{
    public class RobotAssemblerTests
    {
        PartDefinition core, block, bar;
        PartCatalog catalog;
        RobotBody built;

        [SetUp]
        public void SetUp()
        {
            core = PartDefinition.Create("core", PartCategory.Core, mass: 2f);
            block = PartDefinition.Create("block", PartCategory.Structure, mass: 1f);
            bar = PartDefinition.Create("bar", PartCategory.Structure, new[] { Vector2Int.zero, Vector2Int.right, new Vector2Int(2, 0) }, mass: 3f);
            catalog = PartCatalog.Create(core, block, bar);
        }

        [TearDown]
        public void TearDown()
        {
            if (built != null)
                Object.DestroyImmediate(built.gameObject);
            Object.DestroyImmediate(catalog);
            Object.DestroyImmediate(core);
            Object.DestroyImmediate(block);
            Object.DestroyImmediate(bar);
        }

        static RobotData Robot(params PlacedPart[] parts)
        {
            var robot = new RobotData();
            robot.parts.AddRange(parts);
            return robot;
        }

        [Test]
        public void Build_CreatesOnePartPerEntry_WithCoreAtPosition()
        {
            built = RobotAssembler.Build(
                Robot(new PlacedPart("block", new Vector2Int(5, 4)), new PlacedPart("core", new Vector2Int(4, 4))),
                catalog, new Vector2(10f, 3f));

            Assert.IsNotNull(built);
            Assert.AreEqual(2, built.Parts.Count);
            Assert.AreSame(core, built.Core.Definition);
            Assert.AreEqual(new Vector3(10f, 3f, 0f), built.Core.transform.position);
            Assert.AreEqual(new Vector3(11f, 3f, 0f), built.Parts[0].transform.position);
        }

        [Test]
        public void Build_RotatedPart_PlacesCellsRotated()
        {
            // Barra girada 90º encima del núcleo: celdas en (0,1), (0,2), (0,3) respecto al núcleo.
            built = RobotAssembler.Build(
                Robot(new PlacedPart("core", new Vector2Int(4, 4)), new PlacedPart("bar", new Vector2Int(4, 5), 1)),
                catalog, Vector2.zero);

            var colliders = built.Parts[1].GetComponentsInChildren<BoxCollider2D>();
            Assert.AreEqual(3, colliders.Length);
            for (int i = 0; i < 3; i++)
            {
                var p = colliders[i].transform.position;
                Assert.AreEqual(0f, p.x, 1e-4f);
                Assert.AreEqual(i + 1f, p.y, 1e-4f);
            }
        }

        [Test]
        public void Build_SetsMassAndCenterOfMass()
        {
            // Núcleo (masa 2) en x=0 y bloque (masa 1) en x=1: centro en x = 1/3.
            built = RobotAssembler.Build(
                Robot(new PlacedPart("core", new Vector2Int(4, 4)), new PlacedPart("block", new Vector2Int(5, 4))),
                catalog, Vector2.zero);

            Assert.AreEqual(3f, built.Rigidbody.mass, 1e-4f);
            Assert.AreEqual(1f / 3f, built.Rigidbody.centerOfMass.x, 1e-4f);
            Assert.AreEqual(0f, built.Rigidbody.centerOfMass.y, 1e-4f);
        }

        [Test]
        public void Build_ActiveParts_GetBehaviourRegisteredInController()
        {
            var wheel = PartDefinition.Create("wheel", PartCategory.Locomotion, behaviour: PartBehaviourKind.Wheel);
            var thruster = PartDefinition.Create("thruster", PartCategory.Locomotion, behaviour: PartBehaviourKind.Thruster);
            var withActive = PartCatalog.Create(core, block, wheel, thruster);
            try
            {
                built = RobotAssembler.Build(
                    Robot(new PlacedPart("core", new Vector2Int(4, 4)), new PlacedPart("wheel", new Vector2Int(5, 4)),
                        new PlacedPart("thruster", new Vector2Int(4, 3)), new PlacedPart("block", new Vector2Int(3, 4))),
                    withActive, Vector2.zero);

                var controller = built.GetComponent<RobotController>();
                Assert.IsNotNull(controller);
                Assert.AreEqual(2, controller.Behaviours.Count);
                Assert.IsNotNull(built.Parts[1].GetComponent<WheelBehaviour>());
                Assert.IsNotNull(built.Parts[2].GetComponent<ThrusterBehaviour>());
                Assert.IsFalse(built.Parts[3].TryGetComponent<PartBehaviour>(out _));
            }
            finally
            {
                Object.DestroyImmediate(withActive);
                Object.DestroyImmediate(wheel);
                Object.DestroyImmediate(thruster);
            }
        }

        [Test]
        public void Build_WithoutCore_ReturnsNull()
        {
            LogAssert.Expect(LogType.Error, new Regex("no tiene núcleo"));
            built = RobotAssembler.Build(Robot(new PlacedPart("block", new Vector2Int(4, 4))), catalog, Vector2.zero);
            Assert.IsNull(built);
        }
    }
}
