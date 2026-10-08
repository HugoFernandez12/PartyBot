using NUnit.Framework;
using PartyBot.Parts;
using PartyBot.Robot;
using UnityEngine;

namespace PartyBot.Tests
{
    public class RobotValidatorTests
    {
        PartDefinition core, block, bar;
        PartCatalog catalog;
        ValidationRules rules;

        [SetUp]
        public void SetUp()
        {
            core = PartDefinition.Create("core", PartCategory.Core, cost: 3);
            block = PartDefinition.Create("block", PartCategory.Structure);
            bar = PartDefinition.Create("bar", PartCategory.Structure, new[] { Vector2Int.zero, Vector2Int.right, new Vector2Int(2, 0) });
            catalog = PartCatalog.Create(core, block, bar);
            rules = new ValidationRules { gridWidth = 9, gridHeight = 9 };
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(catalog);
            Object.DestroyImmediate(core);
            Object.DestroyImmediate(block);
            Object.DestroyImmediate(bar);
        }

        static PlacedPart P(string id, int x, int y, int rotation = 0) => new(id, new Vector2Int(x, y), rotation);

        static RobotData Robot(params PlacedPart[] parts)
        {
            var robot = new RobotData();
            robot.parts.AddRange(parts);
            return robot;
        }

        ValidationResult Validate(RobotData robot) => RobotValidator.Validate(robot, catalog, rules);

        [Test]
        public void ConnectedRobot_IsValid()
        {
            var result = Validate(Robot(P("core", 4, 4), P("block", 5, 4), P("block", 4, 5), P("block", 4, 6)));
            Assert.IsTrue(result.IsValid, result.ToString());
        }

        [Test]
        public void EmptyRobot_HasNoCore()
        {
            var result = Validate(Robot());
            Assert.IsTrue(result.Has(ValidationErrorType.NoCore));
        }

        [Test]
        public void WithoutCore_OnlyReportsNoCore()
        {
            var result = Validate(Robot(P("block", 1, 1), P("block", 7, 7)));
            Assert.IsTrue(result.Has(ValidationErrorType.NoCore));
            Assert.IsFalse(result.Has(ValidationErrorType.Disconnected));
        }

        [Test]
        public void TwoCores_FlagsTheSecond()
        {
            var result = Validate(Robot(P("core", 4, 4), P("core", 5, 4)));
            Assert.IsTrue(result.Has(ValidationErrorType.MultipleCores, 1));
            Assert.IsFalse(result.Has(ValidationErrorType.MultipleCores, 0));
        }

        [Test]
        public void SameCell_IsOverlap()
        {
            var result = Validate(Robot(P("core", 4, 4), P("block", 4, 4)));
            Assert.IsTrue(result.Has(ValidationErrorType.Overlap, 1));
        }

        [Test]
        public void MultiCellPart_OverlapsOnAnyCell()
        {
            // La barra ocupa (2,4),(3,4),(4,4) y pisa el núcleo.
            var result = Validate(Robot(P("core", 4, 4), P("bar", 2, 4)));
            Assert.IsTrue(result.Has(ValidationErrorType.Overlap, 1));
        }

        [Test]
        public void OutsideGrid_IsOutOfBounds()
        {
            var result = Validate(Robot(P("core", 0, 0), P("block", -1, 0)));
            Assert.IsTrue(result.Has(ValidationErrorType.OutOfBounds, 1));
        }

        [Test]
        public void RotatedBar_CanLeaveTheGrid()
        {
            // Girada 90º ocupa (4,7),(4,8),(4,9): la última se sale de un 9x9.
            var result = Validate(Robot(P("core", 4, 6), P("bar", 4, 7, 1)));
            Assert.IsTrue(result.Has(ValidationErrorType.OutOfBounds, 1));
        }

        [Test]
        public void Gap_IsDisconnected()
        {
            var result = Validate(Robot(P("core", 4, 4), P("block", 6, 4)));
            Assert.IsTrue(result.Has(ValidationErrorType.Disconnected, 1));
        }

        [Test]
        public void DiagonalOnly_IsDisconnected()
        {
            var result = Validate(Robot(P("core", 4, 4), P("block", 5, 5)));
            Assert.IsTrue(result.Has(ValidationErrorType.Disconnected, 1));
        }

        [Test]
        public void MultiCellPart_BridgesConnection()
        {
            // Núcleo (1,4) – barra (2..4,4) – bloque (5,4).
            var result = Validate(Robot(P("core", 1, 4), P("bar", 2, 4), P("block", 5, 4)));
            Assert.IsTrue(result.IsValid, result.ToString());
        }

        [Test]
        public void ChainThroughOtherParts_IsConnected()
        {
            var result = Validate(Robot(P("core", 4, 4), P("block", 6, 4), P("block", 5, 4)));
            Assert.IsTrue(result.IsValid, result.ToString());
        }

        [Test]
        public void UnknownId_IsReported()
        {
            var result = Validate(Robot(P("core", 4, 4), P("laser", 5, 4)));
            Assert.IsTrue(result.Has(ValidationErrorType.UnknownPart, 1));
        }

        [Test]
        public void OverBudget_IsReported()
        {
            rules.maxCost = 3;
            var result = Validate(Robot(P("core", 4, 4), P("block", 5, 4)));
            Assert.IsTrue(result.Has(ValidationErrorType.OverBudget));
        }

        [Test]
        public void ZeroMaxCost_MeansNoLimit()
        {
            rules.maxCost = 0;
            var result = Validate(Robot(P("core", 4, 4), P("block", 5, 4), P("block", 3, 4)));
            Assert.IsTrue(result.IsValid, result.ToString());
        }
    }
}
