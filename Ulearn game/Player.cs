using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ulearn_game.Properties;


namespace Ulearn_game
{
    public class Player : Entity
    {
        public bool Up, Down, Left, Right;
        public float Angle;
        public Point Mouse;
        public bool IsAttacking;
        public int CurrentFrame;
        public Bitmap[] Punch1;
        public Bitmap[] Punch2;
        public Player()
        {
            Width = 160;
            Height = 160;
            Sprite = new Bitmap(Properties.Resources.jacket_idle, Width, Height);
            Up = false;
            Down = false;
            Left = false;
            Right = false;
            Point.X = 1200;
            Point.Y = 900;
            Angle = 0;
            Speed = 10;
            Weapon = "punch1";
            IsAttacking = false;
            CurrentFrame = 0;
            Punch1 = new Bitmap[]
            {
                new Bitmap(Properties.Resources.punch1_0, Width, Height),
                new Bitmap(Properties.Resources.punch1_1, Width, Height),
                new Bitmap(Properties.Resources.punch1_2, Width, Height),
                new Bitmap(Properties.Resources.punch1_3, Width, Height),
                new Bitmap(Properties.Resources.punch1_4, Width, Height),
                new Bitmap(Properties.Resources.punch1_5, Width, Height),
                new Bitmap(Properties.Resources.punch1_6, Width, Height),
            };
            Punch2 = new Bitmap[]
            {
                new Bitmap(Properties.Resources.punch2_0, Width, Height),
                new Bitmap(Properties.Resources.punch2_1, Width, Height),
                new Bitmap(Properties.Resources.punch2_2, Width, Height),
                new Bitmap(Properties.Resources.punch2_3, Width, Height),
                new Bitmap(Properties.Resources.punch2_4, Width, Height),
                new Bitmap(Properties.Resources.punch2_5, Width, Height),
                new Bitmap(Properties.Resources.punch2_6, Width, Height),
            };

        }
        public void Movement()
        {
            if (Right) { Point.X += Speed; }
            if (Left) { Point.X -= Speed; }
            if (Up) { Point.Y -= Speed; } 
            if (Down) { Point.Y += Speed; }
        }

        public void RotatePlayer(Graphics g)
        {
            Movement();
            UpdateAngle(null, null);
            var rotated = new Bitmap(Width, Height);
            using (Graphics fromImage = Graphics.FromImage(rotated))
            {
                fromImage.TranslateTransform(Width / 2f, Height / 2f);
                fromImage.RotateTransform(Angle);
                fromImage.TranslateTransform(-(Width / 2f), -(Height / 2f));
                fromImage.DrawImage(Sprite, 0,0, Width, Height);
            }
            g.DrawImage(rotated, Point.X, Point.Y, Width, Height);
        }

        public void PlayAnimation()
        {
            if (Weapon == "punch1" && IsAttacking && CurrentFrame < Punch1.Length)
            {
                Sprite = Punch1[CurrentFrame];
                CurrentFrame++;
                if (CurrentFrame == Punch1.Length - 1)
                {
                    Weapon = "punch2";
                    IsAttacking = false;
                    CurrentFrame = 0;
                }
            }
            else if (Weapon == "punch2" && IsAttacking && CurrentFrame < Punch2.Length)
            {
                Sprite = Punch2[CurrentFrame];
                CurrentFrame++;
                if (CurrentFrame == Punch2.Length - 1)
                {
                    Weapon = "punch1";
                    IsAttacking = false;
                    CurrentFrame = 0;
                }
            }
            else if (Weapon == "punch1")
            {
                IsAttacking = false;
                Sprite = new Bitmap(Properties.Resources.jacket_idle, Width, Height);
            }
            else if (Weapon == "punch2")
            {
                IsAttacking = false;
                Sprite = new Bitmap(Properties.Resources.jacket_idle, Width, Height);
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
                Angle = (float)Math.Atan2(e.Y - Point.Y - Height/2f , e.X - Point.X - Width/2f) * 180 / (float)Math.PI;
                Mouse.X = e.X;
                Mouse.Y = e.Y;
            }
            else
            {
                Angle = (float)Math.Atan2(Mouse.Y - Point.Y - Height/2f, Mouse.X - Point.X - Width/2f) * 180 / (float)Math.PI;
            }
        }

        public void Attack(object sender, MouseEventArgs e)
        {
            IsAttacking = true;
        }
    }
}
