using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ulearn_game
{
    public partial class Game : Form
    {
        public static Player Player;
        public static Bandit[] Bandits;
        Timer mainTimer = new Timer();
        public static int Level;
        public Game()
        {
            InitializeComponent();

            Player = new Player();
            Bandits = new Bandit[]
            {
                new Bandit(new Point(300,100)),
                new Bandit(new Point(1000 ,500)),
                new Bandit(new Point(500, 600)),
            };
            Level = 1;

            DoubleBuffered = true;
            mainTimer.Interval = 20;
               KeyDown += GameKeyDown;
            KeyUp += GameKeyUp;
            MouseMove += Player.UpdateAngle;
            MouseClick += Player.Attack;
            mainTimer.Tick += MainLoop;
            mainTimer.Start();
        }
        
        public void MainLoop(object sender, EventArgs e)
        {
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            CreateMap(g);
            foreach (var bandit in Bandits)
            {
                if (!bandit.IsDead) { bandit.Alive(g); }
                else { bandit.Dead(g); }
            }
            if(!Player.IsDead) { Player.Alive(g); }
            else
            {
                Player.Dead(g); 
                mainTimer.Stop();
            }
        }

        public void CreateMap(Graphics g)
        {
            if (Level == 1)
            {
                var Level = new int[,]
                {
                    {1,1,1,1,1,1,2,2,2},
                    {1,1,1,1,1,1,1,1,1},
                    {1,1,1,1,1,1,1,1,1},
                    {1,1,1,1,1,1,1,1,1},
                    {1,1,1,1,1,1,1,1,1},
                    {1,1,1,1,1,1,1,1,1},
                    {1,1,1,1,1,1,1,1,1},
                    {1,1,1,1,1,1,1,1,1},
                    {1,1,1,1,1,1,1,1,1},
                    {1,1,1,1,1,1,1,1,1},
                    {1,1,1,1,1,1,1,1,1},
                    {1,1,1,1,1,1,1,1,1},
                    {1,1,1,1,1,1,1,1,1},
                    {1,1,1,1,1,1,1,1,1},
                    {1,1,1,1,1,1,1,1,1},
                    {1,1,1,1,1,1,1,1,1},
                    {1,1,1,1,1,1,1,1,1},
                };
                for (int i = 0; i < Level.GetLength(0); i++)
                {
                    for (int j = 0; j < Level.GetLength(1); j++)
                    {
                        if(Level[i,j] == 1)
                            g.DrawImage(Properties.Resources.brick2, i * 100, j * 100);
                        else if(Level[i,j] == 2)
                            g.DrawImage(Properties.Resources.brick, i * 100, j * 100);
                    }
                }

            }
        }


        public void GameKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D) { Player.Right = true; }
            if (e.KeyCode == Keys.A) { Player.Left = true; }
            if (e.KeyCode == Keys.W) { Player.Up = true; }
            if (e.KeyCode == Keys.S) { Player.Down = true; }
        }

        public void GameKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D) { Player.Right = false; }
            if (e.KeyCode == Keys.A) { Player.Left = false; }
            if (e.KeyCode == Keys.W) { Player.Up = false; }
            if (e.KeyCode == Keys.S) { Player.Down = false; }
        }
    }

}
