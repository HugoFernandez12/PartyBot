using System;
using NUnit.Framework;
using PartyBot.Robot;
using UnityEngine;

namespace PartyBot.Tests
{
    public class RobotDataTests
    {
        [Test]
        public void Json_RoundTrip_KeepsEverything()
        {
            var original = new RobotData { name = "Tostadora" };
            original.parts.Add(new PlacedPart("core", new Vector2Int(4, 4)));
            original.parts.Add(new PlacedPart("wheel", new Vector2Int(5, 4), 3));

            var copy = RobotData.FromJson(original.ToJson());

            Assert.AreEqual("Tostadora", copy.name);
            Assert.AreEqual(RobotData.CurrentVersion, copy.version);
            Assert.AreEqual(2, copy.parts.Count);
            Assert.AreEqual("wheel", copy.parts[1].partId);
            Assert.AreEqual(new Vector2Int(5, 4), copy.parts[1].Cell);
            Assert.AreEqual(3, copy.parts[1].rotation);
        }

        [Test]
        public void Json_IsHumanReadable()
        {
            var robot = new RobotData();
            robot.parts.Add(new PlacedPart("core", new Vector2Int(1, 2)));

            var json = robot.ToJson();

            StringAssert.Contains("\"partId\": \"core\"", json);
            StringAssert.Contains("\"x\": 1", json);
        }

        [Test]
        public void FromJson_MissingFields_UsesDefaults()
        {
            var robot = RobotData.FromJson("{\"version\":1}");

            Assert.IsNotNull(robot.parts);
            Assert.IsEmpty(robot.parts);
            Assert.AreEqual(string.Empty, robot.name);
        }

        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void FromJson_Empty_Throws(string json)
        {
            Assert.Throws<ArgumentException>(() => RobotData.FromJson(json));
        }
    }
}
