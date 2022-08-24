using System;
using System.Collections.Generic;
using System.Drawing;
using NUnit.Framework;

namespace Ulearn_game
{
    [TestFixture]
    public class PlayerTests: Game
    {
        public static readonly int[,] TestLevel = 
        {
            {2, 2, 2, 2, 2, 2, 2, 2, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 2, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 2, 2, 2, 2, 2, 2, 2, 2},
        };

        public static readonly List<Bandit> TestBandits = new List<Bandit>()
        {
            new Bandit(new Point(200, 200), new Point(3, 1), "fist", 10),
            
        };

        public void InitializeTest()
        {
            Level = TestLevel;
            Bandits = TestBandits;
            Player.IsDead = false;
        }

        [Test]
        public void StartAmmo()
        {
            InitializeTest();
            Assert.AreEqual(Player.Ammo,0);
        }

        [Test]
        public void StartHitPoints()
        {
            InitializeTest();
            Assert.AreEqual(Player.IsDead, false);
        }
        [Test]
        public void PlayerStuckInWall()
        {
            InitializeTest();
            Player.Point.X = 200;
            Player.Point.Y = 200;
            Player.Up = true;
            Player.Speed = 100;
            Player.Point.Y -= Player.Speed;
            Player.Point.Y -= Player.Speed;
            Assert.True(Player.IsWall());
        }
        [Test]
        public void PlayerAvoidWalls()
        {
            InitializeTest();
            Player.Point.X = 200;
            Player.Point.Y = 200;
            Player.Left = true;
            Player.Speed = 100;
            Player.Move();
            Player.Move();
            Assert.False(Player.IsWall());
        }

        [Test]
        public void PlayerDiesFromFist()
        {
            InitializeTest();
            Player.Point.X = 202;
            Player.Point.Y = 200;
            Player.IsGettingMeleeDamage();
            Assert.True(Player.IsDead);
        }

        [Test]
        public void PlayerFarFromMeleeBandit()
        {
            InitializeTest();
            Player.Point.X = 222;
            Player.Point.Y = 200;
            Player.IsGettingMeleeDamage();
            Assert.False(Player.IsDead);
        }
    }
}