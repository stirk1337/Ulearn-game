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
    public class Player: IEntity
    {
        public bool Up, Down, Left, Right;
        public int Width { get; set; }
        public int Height { get; set; }
        public Bitmap Sprite { get; set; }
        public float Angle { get; set; }
        public Point Mouse { get; set; }
        public Point Point { get; set; }
        public bool IsAttacking { get; set; } 
        public int CurrentFrame { get; set; }
        public Bitmap[] Punch1;
        public Bitmap[] Punch2;
        public Bitmap[] DeadSprites;
        public int Speed { get; set; }
        public string Weapon { get; set; }
        public bool IsDead { get; set; }
        public float DeadAngle { get; set; }
        public Point KillerPoint { get; set; }
        public int KillerHeight { get; set; }
        public int KillerWidth { get; set; }

        public Player()
        {
            Width = 160;
            Height = 160;
            Sprite = new Bitmap(Properties.Resources.jacket_idle, Width, Height);
            Up = false;
            Down = false;
            Left = false;
            Right = false;
            Point = new Point(200, 500);
            Angle = 0;
            Speed = 10;
            Weapon = "punch1";
            IsAttacking = false;
            CurrentFrame = 0;
            IsDead = false;
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

        public bool IsWall()
        {
            var x = (Point.X + Width / 2) / 100;
            var y = (Point.Y + Height / 2) / 100;
            if (Game.Level[x, y] == 2)
                return true;
            return false;
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

        public void RotatePlayer(Graphics g, float angle)
        {
            var rotated = new Bitmap(Width, Height);
            using (Graphics fromImage = Graphics.FromImage(rotated))
            {
                fromImage.TranslateTransform(Width / 2f, Height / 2f);
                fromImage.RotateTransform(angle);
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
            UpdateAngle(null, null);
            var angle = IsDead ? GetAngleToTarget(KillerPoint, KillerWidth, KillerHeight, "")+180 : Angle;
            RotatePlayer(g, angle);
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
            IsAttacking = true;
        }

        public float GetAngleToTarget(Point targetPoint, int targetWidth, int targetHeight, string target)
        {
            var y = targetPoint.Y + targetHeight / 2f - Point.Y - Height / 2f;
            var x = targetPoint.X + targetHeight / 2f - Point.X - Width / 2f;
            return (float)Math.Atan2(y, x) * 180 / (float)Math.PI;
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
                    KillerPoint = new Point(bandit.Point.X, bandit.Point.Y);
                    KillerHeight = bandit.Height;
                    KillerWidth = bandit.Width;
                }
            }
        }

        public void Alive(Graphics g)
        {
            OnPaint(g);
            Move();
            IsGettingMeleeDamage();
            PlayAnimation();
        }

        public void Dead(Graphics g)
        {
            var rnd = new Random();
            var random = rnd.Next(0, Game.Bandits.Length);
            if (Speed != 0)
            {
                Sprite = DeadSprites[random];
                Speed = 0;
            }

            OnPaint(g);
        }

        public Point GetFastestPath()
        {
            throw new NotImplementedException();
        }
    }
}
