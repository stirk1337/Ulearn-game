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
    class Player : Entity
    {
        public bool Up, Down, Left, Right;
        public Point Point;
        public float Angle;
        public Point Mouse;
        public bool isAttacking;
        public int BitaCurrentFrame;
        public Bitmap[] BitaAttack;
        public Player(int health, Bitmap sprite)
        {
            Health = health;
            Sprite = sprite;
            Up = false;
            Down = false;
            Left = false;
            Right = false;
            Point.X = 0;
            Point.Y = 0;
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
                Point.X += 10;
            }

            if (Left)
            {
                Point.X -= 10;
            }
            
            if (Up)
            {
                Point.Y -= 10;
            }

            if (Down)
            {
                Point.Y += 10;
            }
        }

        public void RotatePlayer(Graphics g)
        {
            g.TranslateTransform(Point.X + Sprite.Width / 2f, Point.Y + Sprite.Height / 2f);
            g.RotateTransform(Angle);
            g.TranslateTransform(-(Point.X + Sprite.Width / 2f), -(Point.Y + Sprite.Height / 2f));
        }

        public void PlayAnimation(Graphics g)
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
            else if(Weapon == "bita1")
            {
                isAttacking = false;
                Sprite = Properties.Resources.bita_idle;
            }
            else if (Weapon == "bita2")
            {
                isAttacking = false;
                Sprite = Properties.Resources.bita_idle2;
            }
            g.DrawImage(Sprite, Point.X, Point.Y); 
        }

        public void OnPaint(Graphics g) 
        {
            RotatePlayer(g);
            PlayAnimation(g);
        }

        public void UpdateAngle(object sender, MouseEventArgs e)
        {
            if (e != null)
            {
                Angle = (float)Math.Atan2(e.Y - Point.Y , e.X - Point.X) * 180 / (float)Math.PI;
                Mouse.X = e.X;
                Mouse.Y = e.Y;
            }
            else
            {
                Angle = (float)Math.Atan2(Mouse.Y - Point.Y, Mouse.X - Point.X) * 180 / (float)Math.PI;
            }
        }

        public void Attack(object sender, MouseEventArgs e)
        {
            isAttacking = true;
        }
        
    }
}
