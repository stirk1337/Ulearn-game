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
        private PictureBox playerSprite;
        private Player player;
        public Form1()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            player = new Player(100, Properties.Resources.icon);
            playerSprite = new PictureBox();
            playerSprite.Image = player.Sprite;
            Controls.Add(playerSprite);

            Timer tmr = new Timer();
            tmr.Interval = 50;
            KeyDown += new KeyEventHandler(gameKeyDown);
            KeyUp += new KeyEventHandler(gameKeyUp);
            tmr.Tick += new EventHandler(MainLoop);
            tmr.Start();
        }

        public void MainLoop(object sender, EventArgs e)
        {
            if (player.right)
            {
                playerSprite.Left += 10;
            }

            if (player.left)
            {
                playerSprite.Left -= 10;
            }

            if (player.up)
            {
                playerSprite.Top -= 10;
            }

            if (player.down)
            {
                playerSprite.Top += 10;
            }
        }



        public void gameKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D)
            {
                player.right = true;
            }

            if (e.KeyCode == Keys.A)
            {
                player.left = true;
            }

            if (e.KeyCode == Keys.W)
            {
                player.up = true;
            }

            if (e.KeyCode == Keys.S)
            {
                player.down = true;
            }
        }

        public void gameKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D)
            {
                player.right = false;
            }

            if (e.KeyCode == Keys.A)
            {
                player.left = false;
            }

            if (e.KeyCode == Keys.W)
            {
                player.up = false;
            }

            if (e.KeyCode == Keys.S)
            {
                player.down = false;
            }
        }

    }

    class Entity
    {
        public int Health;
        public int Damage;
        public Image Sprite; 
    } 

    class Player : Entity
    {

        public bool up, down, left, right;
        public Player(int health, Image sprite)
        {
            this.Health = health;
            this.Sprite = sprite;
            up = false;
            down = false;
            left = false;
            right = false;
        }

    }

    class Bandit : Entity
    {
        public Bandit()
        {
            Health = 20;
            Damage = 10;
        }
    }

}
