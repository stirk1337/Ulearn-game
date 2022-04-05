using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
        public float Angle;

        public void UpdateAngle()
        {
            var y = Game.Player.Point.Y + Game.Player.Sprite.Height / 2f - Point.Y + Sprite.Height / 2f;
            var x = Game.Player.Point.X + Game.Player.Sprite.Width / 2f - Point.X + Sprite.Width / 2f;
            Angle = (float)Math.Atan2(y, x) * 180 / (float)Math.PI;
        }

        public void RotateEntity(Graphics g)
        {
            var rotated = new Bitmap(Sprite.Width, Sprite.Height);
            using (Graphics fromImage = Graphics.FromImage(rotated))
            {
                fromImage.TranslateTransform(Sprite.Width / 2f, Sprite.Height / 2f);
                fromImage.RotateTransform(Angle);
                fromImage.TranslateTransform(-(Sprite.Width / 2f), -(Sprite.Height / 2f));
                fromImage.DrawImage(Sprite, 0, 0);
            }
            g.DrawImage(rotated, Point.X, Point.Y);
        }

    }
    public class Bandit : Entity
    {
        public Bandit(Point point)
        {
            Sprite = Properties.Resources.bandit;
            Width = 60;
            Height = 60;
            Point.X = point.X;
            Point.Y = point.Y;
            Angle = 0;
            Weapon = "fist";
        }

        public void OnPaint(Graphics g)
        {
            ToAttack();
            RotateEntity(g);
        }

        public void ToAttack()
        {
            if (Game.Player.Point.X < Point.X && Game.Player.Point.Y < Point.Y)
            {
                Point.X -= 10;
                Point.Y -= 10;
            }
            else if (Game.Player.Point.X < Point.X && Game.Player.Point.Y > Point.Y)
            {
                Point.X -= 10;
                Point.Y += 10;
            }
            else if (Game.Player.Point.X > Point.X && Game.Player.Point.Y < Point.Y)
            {
                Point.X += 10;
                Point.Y -= 10;
            }
            else if (Game.Player.Point.X > Point.X && Game.Player.Point.Y > Point.Y)
            {
                Point.X += 10;
                Point.Y += 10;
            }
            else if (Game.Player.Point.X < Point.X && Game.Player.Point.Y == Point.Y)
            {
                Point.X -= 10;
            }
            else if (Game.Player.Point.X > Point.X && Game.Player.Point.Y == Point.Y)
            {
                Point.X += 10;
            }
            else if (Game.Player.Point.Y > Point.Y && Game.Player.Point.X == Point.X)
            {
                Point.Y += 10;
            }
            else if (Game.Player.Point.Y < Point.Y && Game.Player.Point.X == Point.X)
            {
                Point.Y -= 10;
            }
        }
    }

}
