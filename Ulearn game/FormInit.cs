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
            var tmr = new Timer();

            DoubleBuffered = true;
            tmr.Interval = 30;
            KeyDown += GameKeyDown;
            KeyUp += GameKeyUp;
            MouseMove += Player.UpdateAngle;
            MouseClick += Player.Attack;
            tmr.Tick += MainLoop;
            tmr.Start();
        }
        
        public void MainLoop(object sender, EventArgs e)
        {
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            Player.Movement();
            Player.OnPaint(g);
            Player.UpdateAngle(null, null);
            foreach (var bandit in Bandits)
            {
                if (!bandit.IsDead)
                {
                    bandit.OnPaint(g);
                    bandit.UpdateAngleToPlayer();
                    bandit.IsGettingMeleeDamage();
                    bandit.UpdateAngleFromPlayer();
                }
                else
                {
                    bandit.Sprite = Properties.Resources.bandit_dead;
                    bandit.Height = 150;
                    bandit.Width = 150;
                    bandit.Speed = 0;
                    bandit.AngleToPlayer = bandit.AngleFromPlayer;
                    bandit.OnPaint(g);
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
