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
            KeyDown += new KeyEventHandler(Movement);
            tmr.Start();
        }
        
        public void Movement(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D)
            {
                playerSprite.Left += 1;
            }

            if (e.KeyCode == Keys.A)
            {
                playerSprite.Left -= 1;
            }

            if (e.KeyCode == Keys.W)
            {
                playerSprite.Top -= 1;
            }

            if (e.KeyCode == Keys.S)
            {
                playerSprite.Top += 1;
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
        public Player(int health, Image sprite)
        {
            this.Health = health;
            this.Sprite = sprite;
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
