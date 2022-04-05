using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Ulearn_game
{
    public class Player : Entity
    {
        public bool Up, Down, Left, Right;
        public float Angle;
        public Point Mouse;
        public bool isAttacking;
        public int BitaCurrentFrame;
        public Bitmap[] BitaAttack;
        public Player(Bitmap sprite)
        {
            Sprite = sprite;
            Up = false;
            Down = false;
            Left = false;
            Right = false;
            Point.X = 1200;
            Point.Y = 900;
            Angle = 0;
            Weapon = "bita1";
            isAttacking = false;
            BitaCurrentFrame = 0;
            BitaAttack = new Bitmap[]
            {
                Properties.Resources.bita_attack1,
                Properties.Resources.bita_attack2,
                Properties.Resources.bita_attack3,
            };
            
        }
        public void Movement()
        {
            if (Right)
            {
                Point.X += 100;
            }

            if (Left)
            {
                Point.X -= 100;
            }
            
            if (Up)
            {
                Point.Y -= 100;
            }

            if (Down)
            {
                Point.Y += 100;
            }
        }

        public void RotatePlayer(Graphics g)
        {
            var rotated = new Bitmap(Sprite.Width, Sprite.Height);
            using (Graphics fromImage = Graphics.FromImage(rotated))
            {
                fromImage.TranslateTransform(Sprite.Width / 2f, Sprite.Height / 2f);
                fromImage.RotateTransform(Angle);
                fromImage.TranslateTransform(-(Sprite.Width / 2f), -(Sprite.Height / 2f));
                fromImage.DrawImage(Sprite, 0,0);
            }
            g.DrawImage(rotated, Point.X, Point.Y);
        }

        public void PlayAnimation()
        {
            if (Weapon == "bita1" && isAttacking && BitaCurrentFrame < 4)
            {
                Sprite = BitaAttack[BitaCurrentFrame];
                BitaCurrentFrame++;
                if (BitaCurrentFrame == 3)
                {
                    Weapon = "bita2";
                    isAttacking = false;
                }
            }
            else if (Weapon == "bita2" && isAttacking && 0 <= BitaCurrentFrame)
            {
                Sprite = BitaAttack[BitaCurrentFrame - 1];
                BitaCurrentFrame--;
                if (BitaCurrentFrame == 0)
                {
                    Weapon = "bita1";
                    isAttacking = false;
                }
            }
            else if (Weapon == "bita1")
            {
                isAttacking = false;
                Sprite = Properties.Resources.bita_idle;
            }
            else if (Weapon == "bita2")
            {
                isAttacking = false;
                Sprite = Properties.Resources.bita_idle2;
            }
        }

        public void OnPaint(Graphics g)
        {
            PlayAnimation();
            RotatePlayer(g);
        }

        public void UpdateAngle(object sender, MouseEventArgs e)
        {
            if (e != null)
            {
                Angle = (float)Math.Atan2(e.Y - Point.Y - Sprite.Height/2f , e.X - Point.X - Sprite.Width/2f) * 180 / (float)Math.PI;
                Mouse.X = e.X;
                Mouse.Y = e.Y;
            }
            else
            {
                Angle = (float)Math.Atan2(Mouse.Y - Point.Y - Sprite.Height/2f, Mouse.X - Point.X - Sprite.Width/2f) * 180 / (float)Math.PI;
            }
        }

        public void Attack(object sender, MouseEventArgs e)
        {
            isAttacking = true;
        }
        
    }
}
