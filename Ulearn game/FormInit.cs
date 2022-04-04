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
    public partial class Form1 : Form
    {
        private Player _player;
        public Form1()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            _player = new Player(100, Properties.Resources.bita_idle);
            var tmr = new Timer();

            DoubleBuffered = true;
            tmr.Interval = 40;
            KeyDown += GameKeyDown;
            KeyUp += GameKeyUp;
            MouseMove += _player.UpdateAngle;
            MouseClick += _player.Attack;
            tmr.Tick += MainLoop;
            tmr.Start();
        }

        public void MainLoop(object sender, EventArgs e)
        {
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var playerSprite = e.Graphics;
            _player.Movement();
            _player.OnPaint(playerSprite);
            _player.UpdateAngle(null, null);

        }

        public void GameKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D) { _player.Right = true; }
            if (e.KeyCode == Keys.A) { _player.Left = true; }
            if (e.KeyCode == Keys.W) { _player.Up = true; }
            if (e.KeyCode == Keys.S) { _player.Down = true; }
        }

        public void GameKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D) { _player.Right = false; }
            if (e.KeyCode == Keys.A) { _player.Left = false; }
            if (e.KeyCode == Keys.W) { _player.Up = false; }
            if (e.KeyCode == Keys.S) { _player.Down = false; }
        }
    }

}
