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
    public class Bandit : IEntity
    {
        public Bitmap Sprite { get; set; }
        public string Weapon { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Point Point { get; set; }
        public int Speed { get; set; }
        public bool IsDead { get; set; }
        public float DeadAngle { get; set; }

        public Bandit(Point point)
        {
            Sprite = Properties.Resources.bandit;
            Width = 80;
            Height = 80;
            Point = new Point(point.X, point.Y);
            Weapon = "fist";
            Speed = 3;
            IsDead = false;
            DeadAngle = -1;
        }

        public void PlayAnimation()
        {
            throw new NotImplementedException();
        }

        public void Alive(Graphics g)
        {
            MoveEntity();
            OnPaint(g, GetAngleToTarget(Game.Player.Point, Game.Player.Width, Game.Player.Height, "player"));
            IsGettingMeleeDamage();
        }

        public void Dead(Graphics g)
        {
            Sprite = Properties.Resources.bandit_dead;
            Height = 150;
            Width = 150;
            Speed = 0;
            if(DeadAngle == -1)
                    DeadAngle = GetAngleToTarget(Game.Player.Point, Game.Player.Width, Game.Player.Height, "bandit");
            OnPaint(g, DeadAngle);
        }

        public float GetAngleToTarget(Point playerPoint, int playerWidth, int playerHeight, string target)
        {
            var y = target == "player" ? playerPoint.Y + playerHeight / 2f - Point.Y - Height / 2f : Point.Y + Height / 2f - playerPoint.Y - playerHeight / 2f;
            var x = target == "player" ? playerPoint.X + playerWidth / 2f - Point.X - Width / 2f : Point.X + Width / 2f - playerPoint.X - playerWidth / 2f;
            return (float)Math.Atan2(y, x) * 180 / (float)Math.PI;
        }
        public void OnPaint(Graphics g, float angle)
        {
            var rotated = new Bitmap(Width, Height);
            using (Graphics fromImage = Graphics.FromImage(rotated))
            {
                fromImage.TranslateTransform(Width / 2f, Height / 2f);
                fromImage.RotateTransform(angle);
                fromImage.TranslateTransform(-(Width / 2f), -(Height / 2f));
                fromImage.DrawImage(Sprite, 0, 0, Width, Height);
            }
            g.DrawImage(rotated, Point.X, Point.Y, Width, Height);
        }
        public void IsGettingMeleeDamage()
        {
            Point player = new Point(Game.Player.Point.X + Game.Player.Width / 2, Game.Player.Point.Y + Game.Player.Height / 2);
            Point bandit = new Point(Point.X + Width / 2, Point.Y + Height / 2);
            var distance = Math.Sqrt(Math.Pow(bandit.X - player.X, 2) + Math.Pow(bandit.Y - player.Y, 2));
            float angleFromPlayer =
                GetAngleToTarget(Game.Player.Point, Game.Player.Width, Game.Player.Height, "bandit");
            var range = Game.Player.Angle < angleFromPlayer ? angleFromPlayer - Game.Player.Angle : Game.Player.Angle - angleFromPlayer;
            if (Game.Player.IsAttacking && distance < 90 && Math.Abs(range) < 50)
                IsDead = true;
        }

        public void MoveEntity()
        {
            if (Game.Player.Point.X < Point.X && Game.Player.Point.Y < Point.Y)
            {
                Point = new Point(Point.X - Speed, Point.Y);
                Point = new Point(Point.X, Point.Y - Speed);
            }
            else if (Game.Player.Point.X < Point.X && Game.Player.Point.Y > Point.Y)
            {
                Point = new Point(Point.X - Speed, Point.Y);
                Point = new Point(Point.X, Point.Y + Speed);
            }
            else if (Game.Player.Point.X > Point.X && Game.Player.Point.Y < Point.Y)
            {
                Point = new Point(Point.X + Speed, Point.Y);
                Point = new Point(Point.X, Point.Y - Speed);
            }
            else if (Game.Player.Point.X > Point.X && Game.Player.Point.Y > Point.Y)
            {
                Point = new Point(Point.X + Speed, Point.Y);
                Point = new Point(Point.X, Point.Y + Speed);
            }
            else if (Game.Player.Point.X < Point.X && Game.Player.Point.Y == Point.Y)
            {
                Point = new Point(Point.X - Speed, Point.Y);
            }
            else if (Game.Player.Point.X > Point.X && Game.Player.Point.Y == Point.Y)
            {
                Point = new Point(Point.X + Speed, Point.Y);
            }
            else if (Game.Player.Point.Y > Point.Y && Game.Player.Point.X == Point.X)
            {
                Point = new Point(Point.X, Point.Y + Speed);
            }
            else if (Game.Player.Point.Y < Point.Y && Game.Player.Point.X == Point.X)
            {
                Point = new Point(Point.X, Point.Y - Speed);
            }
        }
    }
}
