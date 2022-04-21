using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
        public bool IsMoving { get; set; }
        public Point MovePoint { get; set; }

        public Bandit(Point point)
        {
            Sprite = Properties.Resources.bandit;
            Width = 80;
            Height = 80;
            Point = new Point(point.X, point.Y);
            Weapon = "fist";
            Speed = 5;
            IsDead = false;
            DeadAngle = -1;
        }

        public void PlayAnimation()
        {
            throw new NotImplementedException();
        }

        public void Alive(Graphics g)
        {
            Move();
            OnPaint(g);
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
            OnPaint(g);
        }

        public float GetAngleToTarget(Point playerPoint, int playerWidth, int playerHeight, string target)
        {
            var y = target == "player" ? playerPoint.Y + playerHeight / 2f - Point.Y - Height / 2f : Point.Y + Height / 2f - playerPoint.Y - playerHeight / 2f;
            var x = target == "player" ? playerPoint.X + playerWidth / 2f - Point.X - Width / 2f : Point.X + Width / 2f - playerPoint.X - playerWidth / 2f;
            return (float)Math.Atan2(y, x) * 180 / (float)Math.PI;
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

        public void OnPaint(Graphics g)
        {
            var angle = IsDead
                ? DeadAngle
                : GetAngleToTarget(Game.Player.Point, Game.Player.Width, Game.Player.Height, "player");
            Rotate(g, angle);
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
            
        public bool IsWall()
        {
            var x = (Point.X + Width / 2) / 100;
            var y = (Point.Y + Height / 2) / 100;
            if (Game.Level[x, y] == 2)
                return true;
            return false;
        }

        public Point GetFastestPath()
        {
            var dx = new int[] {0, 1, 0, -1, 1, 1, -1, -1};
            var dy = new int[] {1, 0, -1, 0, 1, -1, -1, 1};
            var used = new bool[100, 100];
            var start = new Point((Point.X + Width / 2) / 100, (Point.Y + Height / 2) / 100);
            var end = new Point((Game.Player.Point.X + Game.Player.Width / 2) / 100,
                (Game.Player.Point.Y + Game.Player.Height / 2) / 100);
            var q = new Queue<Point>();
            var path = new List<Point>[100, 100];
            for (int i = 0; i < path.GetLength(0); i++)
            for (int j = 0; j < path.GetLength(1); j++)
                path[i, j] = new List<Point>();
            q.Enqueue(start);
            used[start.X, start.Y] = true;
            while (q.Count != 0)
            {
                var ind = q.Dequeue();
                var indx = ind.X;
                var indy = ind.Y;
                for (int d = 0; d < 8; ++d)
                {
                    var nx = indx + dx[d];
                    var ny = indy + dy[d];
                    if (nx >= 0 && nx < Game.Level.GetLength(0) && ny >= 0 && ny < Game.Level.GetLength(1) &&
                        Game.Level[nx, ny] == 1 && !used[nx, ny])
                    {
                        q.Enqueue(new Point(nx, ny));
                        used[nx, ny] = true;
                        path[nx, ny] = path[indx, indy].ToList();
                        path[nx, ny].Add(new Point(indx, indy));
                    }
                }
            }

            //Debug.WriteLine(path[end.X, end.Y][1]);
            try
            {
                Debug.WriteLine(path[end.X, end.Y][1]);
                path[end.X, end.Y][1] = new Point(path[end.X, end.Y][1].X * 100 + 50, path[end.X, end.Y][1].Y * 100 + 50);
                return path[end.X, end.Y][1];
                
            }
            catch
            {
                Debug.WriteLine(new Point((Game.Player.Point.X + Game.Player.Width / 2),
                    (Game.Player.Point.Y + Game.Player.Height / 2)));
                return new Point((Game.Player.Point.X + Game.Player.Width / 2),
                    (Game.Player.Point.Y + Game.Player.Height / 2));
            }

        }

        public void Move()
        {

            var moveTo = GetFastestPath();
            Debug.WriteLine(moveTo);
            if (moveTo.X < Point.X)
                Point = new Point(Point.X - Speed, Point.Y);
            else if(moveTo.X > Point.X)
                Point = new Point(Point.X + Speed, Point.Y);
            if (moveTo.Y < Point.Y)
                Point = new Point(Point.X, Point.Y - Speed);
            else if(moveTo.Y > Point.Y)
                Point = new Point(Point.X, Point.Y + Speed);


        }

    }
}
