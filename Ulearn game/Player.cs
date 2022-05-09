using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ulearn_game.Properties;


namespace Ulearn_game
{
    public class Player: Entity
    {
        public bool Up, Down, Left, Right;
        public float Angle { get; set; }
        public Point Mouse { get; set; }
        public bool IsAttacking { get; set; } 
        public int CurrentFrame { get; set; }
        public Bitmap[] Punch1;
        public Bitmap[] Punch2;
        public Bitmap[] DeadSprites;
        public Point KillerPoint { get; set; }
        public int KillerHeight { get; set; }
        public int KillerWidth { get; set; }
        public int Ammo { get; set; }
        public int LastAmmo { get; set; }

        public Player()
        {
            Width = 160;
            Height = 160;
            Sprite = new Bitmap(Properties.Resources.jacket_idle, Width, Height);
            Up = false;
            Down = false;
            Left = false;
            Right = false;
            Point = new Point(100, 500);
            Angle = 0;
            Speed = 10;
            Weapon = "punch1";
            IsAttacking = false;
            CurrentFrame = 0;
            IsDead = false;
            Ammo = 0;
            LastAmmo = 0;
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
            DeadSprites = new Bitmap[]
            {
                new Bitmap(Properties.Resources.jacket_dead_1, Width, Height),
                new Bitmap(Properties.Resources.jacket_dead_2, Width, Height),
                new Bitmap(Properties.Resources.jacket_dead_3, Width, Height),
                new Bitmap(Properties.Resources.jacket_dead_3, Width, Height),
            };
            
        }

        public void Move()
        {
            if (Right)
            {
                Point = new Point(Point.X + Speed, Point.Y);
                if (IsWall())
                    Point = new Point(Point.X - Speed, Point.Y);
            }

            if (Left)
            {
                Point = new Point(Point.X - Speed, Point.Y);
                if (IsWall())
                    Point = new Point(Point.X + Speed, Point.Y);
            }

            if (Up)
            {
                Point = new Point(Point.X, Point.Y - Speed);
                if (IsWall())
                    Point = new Point(Point.X, Point.Y + Speed);
            }

            if (Down)
            {
                Point = new Point(Point.X, Point.Y + Speed);
                if (IsWall())
                    Point = new Point(Point.X, Point.Y - Speed);
            }
        }

        public void PlayAnimation()
        {
            if(CurrentFrame == 1 && (Weapon == "punch1" || Weapon == "punch2"))
                Game.PlaySound("punch");
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
            else if (Weapon == "pistol")
            {
                Sprite = new Bitmap(Properties.Resources.jacket_pistol, Width, Height);
                IsAttacking = false;
                CurrentFrame = 0;
            }
            else
            {
                CurrentFrame = 0;
                IsAttacking = false;
            }
        }

        public void OnPaint(Graphics g)
        {
            UpdateAngle(null, null);
            var angle = IsDead ? GetAngleToTarget(KillerPoint, KillerWidth, KillerHeight, "")+180 : Angle;
            Rotate(g, angle);
        }

        public void UpdateAngle(object sender, MouseEventArgs e)
        {
            if (e != null)
            {
                Angle = (float)Math.Atan2(e.Y - Point.Y - Height/2f , e.X - Point.X - Width/2f) * 180 / (float)Math.PI;
                Mouse = new Point(e.X, e.Y);
            }
            else
            {
                Angle = (float)Math.Atan2(Mouse.Y - Point.Y - Height/2f, Mouse.X - Point.X - Width/2f) * 180 / (float)Math.PI;
            }
        }

        public void Attack(object sender, MouseEventArgs e)
        {
            if(Weapon == "punch1" || Weapon == "punch2")
                IsAttacking = true;
            else if(Ammo > 0)
            {
                Game.PlaySound("shoot");
                Ammo -= 1;
                Game.Bullets.Add(new Bullet(Angle, Width /2, Height/2, Mouse.X, Mouse.Y, Point, true));
            }
        }

        public void IsGettingMeleeDamage()
        {
            foreach (var bandit in Game.Bandits)
            {
                var banditPoint = new Point(bandit.Point.X + bandit.Width / 2, bandit.Point.Y + bandit.Height / 2);
                var playerPoint = new Point(Point.X + Width / 2, Point.Y + Height / 2);
                var distance = Math.Sqrt(Math.Pow(banditPoint.X - playerPoint.X, 2) + Math.Pow(banditPoint.Y - playerPoint.Y, 2));
                if (distance < 70 && !bandit.IsDead)
                {
                    IsDead = true;
                }
            }
        }

        public void IsGettingRangeDamage()
        {
            foreach (var bullet in Game.Bullets)
            {
                var bulletPoint = new PointF(bullet.Point.X + bullet.Width / 2, bullet.Point.Y + bullet.Height / 2);
                var playerPoint = new PointF(Point.X + Width / 2, Point.Y + Height / 2);
                var distance = Math.Sqrt(Math.Pow(bulletPoint.X - playerPoint.X, 2) + Math.Pow(bulletPoint.Y - playerPoint.Y, 2));
                if (distance < 50 && !IsDead && !bullet.IsAlly)
                {
                    Game.PlaySound("kill_bullet");
                    IsDead = true;
                }
            }
        }

        public void Alive(Graphics g)
        {
            OnPaint(g);
            Move();
            IsGettingMeleeDamage();
            IsGettingRangeDamage();
            PlayAnimation();
        }

        public void Dead(Graphics g)
        {
            var rnd = new Random();
            var random = rnd.Next(0, DeadSprites.Length);
            if (Speed != 0)
            {
                Sprite = DeadSprites[random];
                Speed = 0;
            }

            OnPaint(g);
        }
    }
}
