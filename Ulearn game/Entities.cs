using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ulearn_game
{ 
    public class Entity
    {
        public Bitmap Sprite;
        public string Weapon;
        public int Width;
        public int Height;
        public Point Point;
        public float AngleToPlayer;
        public float AngleFromPlayer;
        public int Speed;
        public bool IsDead;

        public void UpdateAngleToPlayer()
        {
            var y = Game.Player.Point.Y + Game.Player.Sprite.Height / 2f - Point.Y - Sprite.Height / 2f;
            var x = Game.Player.Point.X + Game.Player.Sprite.Width / 2f - Point.X - Sprite.Width / 2f;
            AngleToPlayer = (float)Math.Atan2(y, x) * 180 / (float)Math.PI;
        }

        public void UpdateAngleFromPlayer()
        {
            var y = Point.Y + Sprite.Height / 2f - Game.Player.Point.Y - Game.Player.Sprite.Height / 2f;
            var x = Point.X + Sprite.Width / 2f - Game.Player.Point.X - Game.Player.Sprite.Width / 2f;
            AngleFromPlayer = (float)Math.Atan2(y, x) * 180 / (float)Math.PI;
        }

        public void RotateEntity(Graphics g)
        {
            var rotated = new Bitmap(Width, Height);
            using (Graphics fromImage = Graphics.FromImage(rotated))
            {
                fromImage.TranslateTransform(Width / 2f, Height / 2f);
                fromImage.RotateTransform(AngleToPlayer);
                fromImage.TranslateTransform(-(Width / 2f), -(Height / 2f));
                fromImage.DrawImage(Sprite, 0, 0, Width, Height);
            }
            g.DrawImage(rotated, Point.X, Point.Y, Width, Height);
        }

        public void IsGettingMeleeDamage()
        {
            Point player = new Point(Game.Player.Point.X + Game.Player.Width/2, Game.Player.Point.Y + Game.Player.Height/2);
            Point bandit = new Point(Point.X + Width/2, Point.Y + Height/2);
            var EPS = 50;
            var distance = Math.Sqrt(Math.Pow(bandit.X - player.X, 2) + Math.Pow(bandit.Y - player.Y, 2));
            float range;
            if (Game.Player.Angle < AngleFromPlayer)
            {
                range = AngleFromPlayer - Game.Player.Angle;
            }
            else
            {
                range = Game.Player.Angle - AngleFromPlayer;
            }
            if (Game.Player.IsAttacking && distance < 90 && Math.Abs(range) < EPS)
            {
                IsDead = true;
            }
        }
    }
    public class Bandit : Entity
    {
        public Bandit(Point point)
        {
            Sprite = Properties.Resources.bandit;
            Width = 80;
            Height = 80;
            Point.X = point.X;
            Point.Y = point.Y;
            AngleToPlayer = 0;
            Weapon = "fist";
            Speed = 3;
            IsDead = false;
        }

        public void OnPaint(Graphics g)
        {
            ToAttack();
            RotateEntity(g);
        }

        public void Alive(Graphics g)
        {
            OnPaint(g);
            UpdateAngleToPlayer();
            IsGettingMeleeDamage();
            UpdateAngleFromPlayer();
        }

        public void Dead(Graphics g)
        {
            Sprite = Properties.Resources.bandit_dead;
            Height = 150;
            Width = 150;
            Speed = 0;
            AngleToPlayer = AngleFromPlayer;
            OnPaint(g);
        }
        public void ToAttack()
        {
            if (Game.Player.Point.X < Point.X && Game.Player.Point.Y < Point.Y)
            {
                Point.X -= Speed;
                Point.Y -= Speed;
            }
            else if (Game.Player.Point.X < Point.X && Game.Player.Point.Y > Point.Y)
            {
                Point.X -= Speed;
                Point.Y += Speed;
            }
            else if (Game.Player.Point.X > Point.X && Game.Player.Point.Y < Point.Y)
            {
                Point.X += Speed;
                Point.Y -= Speed;
            }
            else if (Game.Player.Point.X > Point.X && Game.Player.Point.Y > Point.Y)
            {
                Point.X += Speed;
                Point.Y += Speed;
            }
            else if (Game.Player.Point.X < Point.X && Game.Player.Point.Y == Point.Y)
            {
                Point.X -= Speed;
            }
            else if (Game.Player.Point.X > Point.X && Game.Player.Point.Y == Point.Y)
            {
                Point.X += Speed;
            }
            else if (Game.Player.Point.Y > Point.Y && Game.Player.Point.X == Point.X)
            {
                Point.Y += Speed;
            }
            else if (Game.Player.Point.Y < Point.Y && Game.Player.Point.X == Point.X)
            {
                Point.Y -= Speed;
            }
        }
    }
}
