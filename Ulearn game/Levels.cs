using System.Collections.Generic;
using System.Drawing;

namespace Ulearn_game
{
    public static class Levels
    {
        public static readonly int[,] Level1 = new int[,]
        {
            {2, 2, 2, 2, 2, 2, 2, 2, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 2, 2, 2, 2, 2, 2, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 2, 2, 2, 2, 2, 1, 2, 2},
            {2, 1, 1, 1, 1, 2, 1, 1, 2},
            {2, 1, 1, 1, 1, 2, 1, 1, 2},
            {2, 1, 1, 1, 1, 2, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 2, 1, 1, 2},
            {2, 1, 1, 1, 1, 2, 1, 1, 2},
            {2, 1, 1, 1, 1, 2, 1, 1, 2},
            {2, 1, 1, 1, 1, 2, 1, 1, 2},
            {2, 2, 2, 2, 2, 2, 2, 2, 2},
        };

        public static readonly List<Bandit> Bandits1 = new List<Bandit>()
        {
            new Bandit(new Point(550, 550), new Point(3, 1), "fist", 10),
            new Bandit(new Point(1550, 650), new Point(7, 6), "fist", 10),
            new Bandit(new Point(850, 150), new Point(11, 5), "fist", 10),
            new Bandit(new Point(1450, 150), new Point(11, 6), "fist", 10),
            new Bandit(new Point(850, 350), new Point(11, 5), "fist", 10),
            new Bandit(new Point(1450, 350), new Point(11, 5), "fist", 10),
        };

        public static readonly int[,] Level2 = new int[,]
        {
            {2, 4, 4, 2, 2, 2, 2, 2, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 2, 2, 2, 1, 2, 2, 2, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 2, 2, 2, 2, 2, 2, 2, 2},
        };

        public static readonly List<Bandit> Bandits2 = new List<Bandit>()
        {
            new Bandit(new Point(1500, 100), new Point(4, 4), "rifle", 0),
            new Bandit(new Point(1500, 700), new Point(4, 4), "rifle", 0),
            new Bandit(new Point(550, 100), new Point(4,4), "fist", 10),
            new Bandit(new Point(550, 700), new Point(4,4), "fist", 10),
        };

        public static readonly int[,] Level3 = new int[,]
        {
            {2, 2, 2, 2, 4, 2, 2, 2, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 2, 1, 2, 1, 2, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 2, 1, 2, 1, 2, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 2, 1, 2, 1, 2, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 5, 5, 2, 1, 2, 5, 5, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 5, 5, 1, 1, 1, 5, 5, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 2, 2, 2, 2, 2, 2, 2, 2},
        };

        public static readonly  List<Bandit> Bandits3 = new List<Bandit>()
        {
            new Bandit(new Point(1500, 700), new Point(1, 4), "rifle", 0),
            new Bandit(new Point(1500, 100), new Point(1,4), "rifle", 0),
            new Bandit(new Point(100,100), new Point(1,4), "fist", 10),
            new Bandit(new Point(100, 700), new Point(1,4), "fist", 10),
            new Bandit(new Point(1100, 700), new Point(9,4), "fist", 10),
            new Bandit(new Point(1100, 100), new Point(9,4), "fist", 10),
        };

        public static readonly int[,] Level4 = new int[,]
        {
            {2, 2, 2, 2, 2, 2, 2, 2, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 4},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 2, 2, 2, 2, 2, 2, 2, 2},
        };

        public static readonly  List<Bandit> Bandits4 = new List<Bandit>()
        {
            new Bandit(new Point(100, 100), new Point(7,7), "fist", 10),
            new Bandit(new Point(300, 100), new Point(7,7), "fist", 10),
            new Bandit(new Point(500, 100), new Point(7,7), "fist", 10),
            new Bandit(new Point(700, 100), new Point(7,7), "fist", 10),
            new Bandit(new Point(900, 100), new Point(7,7), "fist", 10),
            new Bandit(new Point(1100, 100), new Point(7,7), "fist", 10),
            new Bandit(new Point(1300, 100), new Point(7,7), "fist", 10),
            new Bandit(new Point(1300, 100), new Point(7,7), "fist", 10),
            new Bandit(new Point(1300, 100), new Point(7,7), "fist", 10),
            new Bandit(new Point(1500, 100), new Point(7,7), "fist", 10),
            new Bandit(new Point(100, 300), new Point(7,7), "fist", 10),
            new Bandit(new Point(100, 500), new Point(7,7), "fist", 10),
            new Bandit(new Point(100, 700), new Point(7,7), "fist", 10),
            new Bandit(new Point(1500, 100), new Point(7,7), "fist", 10),
            new Bandit(new Point(1300, 300), new Point(7,7), "fist", 10),
            new Bandit(new Point(1300, 500), new Point(7,7), "fist", 10),
            new Bandit(new Point(1300, 100), new Point(7,7), "fist", 10),
            new Bandit(new Point(300, 500), new Point(7,7), "fist", 10),
            new Bandit(new Point(500, 500), new Point(7,7), "fist", 10),
            new Bandit(new Point(900, 500), new Point(7,7), "fist", 10),
            new Bandit(new Point(1100, 500), new Point(7,7), "fist", 10),
        };

        public static readonly int[,] Level5 = new int[,]
        {
            {2, 2, 2, 2, 2, 2, 2, 2, 2},
            {2, 1, 1, 1, 5, 1, 1, 1, 2},
            {2, 1, 1, 1, 5, 1, 1, 1, 2},
            {2, 1, 1, 1, 5, 1, 1, 1, 2},
            {2, 1, 1, 1, 5, 1, 1, 1, 2},
            {2, 1, 1, 1, 5, 1, 1, 1, 2},
            {2, 2, 1, 2, 2, 1, 2, 2, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 4},
            {2, 1, 1, 1, 1, 1, 1, 1, 4},
            {2, 2, 1, 2, 2, 1, 2, 2, 2},
            {2, 1, 1, 1, 5, 1, 1, 1, 2},
            {2, 1, 1, 1, 5, 1, 1, 1, 2},
            {2, 1, 1, 1, 5, 1, 1, 1, 2},
            {2, 1, 1, 1, 5, 1, 1, 1, 2},
            {2, 1, 1, 1, 5, 1, 1, 1, 2},
            {2, 1, 1, 1, 5, 1, 1, 1, 2},
            {2, 2, 2, 2, 2, 2, 2, 2, 2},
        };

        public static readonly List<Bandit> Bandits5 = new List<Bandit>()
        {
            new Bandit(new Point(700,100), new Point(8,7), "fist", 10),
            new Bandit(new Point(800,100), new Point(8,7), "fist", 10),
            new Bandit(new Point(1000,100), new Point(9,2), "fist", 10),
            new Bandit(new Point(1500, 100), new Point(8,7), "rifle", 3),
            new Bandit(new Point(500,100), new Point(6,2), "fist", 10),
            new Bandit(new Point(500,700), new Point(7,5), "fist", 10),
            new Bandit(new Point(100, 700), new Point(8,7), "rifle",3),
            new Bandit(new Point(1000,700), new Point(9,5), "fist", 10),
        };

        public static readonly int[,] Level6 = new int[,]
        {
            {2, 2, 2, 2, 4, 2, 2, 2, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 2, 2, 2, 1, 2, 2, 2, 2},
            {2, 1, 1, 5, 1, 5, 1, 1, 2},
            {2, 1, 1, 5, 1, 5, 5, 1, 2},
            {2, 1, 1, 5, 1, 1, 1, 1, 2},
            {2, 1, 1, 5, 1, 5, 5, 1, 2},
            {2, 1, 1, 1, 1, 1, 5, 1, 2},
            {2, 5, 5, 5, 5, 5, 5, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 5, 5, 5, 5, 2, 2, 2},
            {2, 1, 5, 1, 1, 1, 1, 1, 2},
            {2, 1, 5, 5, 1, 5, 1, 5, 2},
            {2, 1, 1, 1, 1, 5, 1, 5, 2},
            {2, 5, 5, 5, 5, 5, 1, 5, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 2, 2, 2, 2, 2, 2, 2, 2},
        };

        public static readonly List<Bandit> Bandits6 = new List<Bandit>()
        {
            new Bandit(new Point(100,100), new Point(1, 4), "fist", 10),
            new Bandit(new Point(100,700), new Point(1, 4), "fist", 10),
            new Bandit(new Point(300, 100), new Point(3,4), "rifle", 3),
            new Bandit(new Point(300, 600), new Point(3,4), "rifle", 3),
            new Bandit(new Point(700, 500), new Point(3,4), "fist", 10),
            new Bandit(new Point(900, 100), new Point(9,6), "rifle", 3),
            new Bandit(new Point(1100, 700), new Point(10,1), "rifle", 3),
            new Bandit(new Point(1500, 100), new Point(10,1), "rifle", 3),
            new Bandit(new Point(1100, 300), new Point(12,4), "fist", 10),
            new Bandit(new Point(1100, 500), new Point(12,4), "fist", 10),
            new Bandit(new Point(1500, 700), new Point(14,6), "fist", 10),
        };

        public static readonly int[,] Level7 = new int[,]
        {
            {2, 2, 2, 2, 2, 2, 2, 2, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 2, 2, 2, 2},
            {2, 1, 1, 1, 1, 2, 1, 2, 2},
            {2, 1, 1, 1, 1, 2, 2, 2, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 4},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 1, 1, 1, 1, 1, 1, 1, 2},
            {2, 2, 2, 2, 2, 2, 2, 2, 2},
        };

        public static readonly List<Bandit> Bandits7 = new List<Bandit>()
       {
           new Bandit(new Point(500, 600), new Point(-1, -1), "fist", 0),
       };
    }
}
