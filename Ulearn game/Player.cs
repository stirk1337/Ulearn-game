using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Ulearn_game
{
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

        public void Movement(PictureBox playerSprite)
        {
            if (this.right)
            {
                playerSprite.Left += 10;
            }

            if (this.left)
            {
                playerSprite.Left -= 10;
            }

            if (this.up)
            {
                playerSprite.Top -= 10;
            }

            if (this.down)
            {
                playerSprite.Top += 10;
            }
        }

    }
}
