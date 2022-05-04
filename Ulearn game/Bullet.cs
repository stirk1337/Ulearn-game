using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Ulearn_game
{
    public class Bullet
    {
        private float Angle;
        private int Width;
        private int Height;
        private Bitmap Sprite;
        private int DeltaWidth;
        private int DeltaHeight;
        private PointF Point;
        private Point Destination;
        private float Speed;
        private float SpeedX;
        private float SpeedY;
        private bool Calculated;

        public Bullet(float angle, int deltaWidth, int deltaHeight, int destX, int destY, Point playerPoint)
        {
            Angle = angle;
            DeltaWidth = deltaWidth;
            DeltaHeight = deltaHeight;
            Sprite = new Bitmap(Properties.Resources.bullet);
            Width = Sprite.Height;
            Height = Sprite.Width;
            Destination = new Point(destX, destY);
            Speed = 10;
            Calculated = false;
            Point = new PointF(playerPoint.X, playerPoint.Y); // need some delta
        }

        public void OnPaint(Graphics g)
        {
            if(!Calculated)
                CalculateDestination();
            Move();
            g.DrawImage(Sprite, Point.X, Point.Y);
        }

        public void Move()
        {
            Point.X += SpeedX;
            Point.Y -= SpeedY;
            if (IsWall())
            {
                //throw new NotImplementedException(); // if bullet touch wall throw this
            }
        }

        public bool IsWall()
        {
            var x = (int)(Point.X + Width / 2) / 100;
            var y = (int)(Point.Y + Height / 2) / 100;
            try
            {
                if (Game.Level[x, y] == 2 || Game.Level[x, y] == 4)
                    return true;
            }
            catch
            {
                return false;
            }
            return false;
        }

        public void CalculateDestination()
        {
            Calculated = true;
            var x = Math.Abs(Destination.X - Point.X);
            var y = Math.Abs(Destination.Y - Point.Y);
            if (x > y)
            {
                
            }
            else
            {
                
            }
        }

    }



}
