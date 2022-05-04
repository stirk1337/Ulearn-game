using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Speed = 50;
            Calculated = false;
            Point = new PointF(playerPoint.X + deltaWidth, playerPoint.Y + deltaHeight);
        }

        public void OnPaint(Graphics g, int index)
        {
            if(!Calculated)
                CalculateDestination();
            Rotate(g, Angle);
            Move(index);
            //g.DrawImage(Sprite, Point.X, Point.Y);
        }

        public void Move(int index)
        {
            Point.X += SpeedX;
            Point.Y -= SpeedY;
            if (IsWall())
            {
                Game.Bullets.RemoveAt(index);
            }
        }
        public void Rotate(Graphics g, float angle)
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
            var x = Destination.X - Point.X;
            var y = Destination.Y - Point.Y;
            SpeedX = x > 0 ? Speed : -Speed;
            SpeedY = y > 0 ? Math.Abs((y / x)) * -Speed : Math.Abs((y / x)) * Speed;
            Debug.WriteLine(SpeedY);
        }

    }



}
