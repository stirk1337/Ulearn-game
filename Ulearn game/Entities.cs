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

    public class Entity
    {
        public Bitmap Sprite;
        public string Weapon;
        public int Width;
        public int Height;
        public Point Point;
        public int Speed;
        public bool IsDead;
        public float DeadAngle;
        public bool IsMoving;
        public int Spawned;
        public Point AgroPoint;
        public int AttackTick; 

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
            var x = (Point.X + Width / 2) / 100;
            var y = (Point.Y + Height / 2) / 100;
            try
            {
                if (Game.Level[x, y] == 2 || Game.Level[x,y] == 4)
                    return true;
            }
            catch
            {
                return false;
            }
            return false;
        }

        public Point GetFastestPath()
        {
            var dx = new int[] { 0, 1, 0, -1, 1, 1, -1, -1 };
            var dy = new int[] { 1, 0, -1, 0, 1, -1, -1, 1 };
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

            try
            {
                path[end.X, end.Y][1] = new Point(path[end.X, end.Y][1].X * 100 + 50, path[end.X, end.Y][1].Y * 100 + 50);
                return path[end.X, end.Y][1];

            }
            catch
            {

                return new Point((Game.Player.Point.X + Game.Player.Width / 2),
                    (Game.Player.Point.Y + Game.Player.Height / 2));
            }

        }

        public float GetAngleToTarget(Point playerPoint, int playerWidth, int playerHeight, string target)
        {
            if (Spawned == 0)
            {
                var spawnAngles = new int[]
                {
                    0, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 260, 270, 280, 290, 300, 310, 320, 330, 340, 350, 360,
                };
                Spawned = spawnAngles[(Point.X + Point.Y) % spawnAngles.Length];
                return Spawned;
            }

            if (!IsMoving)
                return Spawned;

            var y = 0.0;
            var x = 0.0;
            if (target == "player")
            {
                y = playerPoint.Y + playerHeight / 2f - Point.Y - Height / 2f;
                x = playerPoint.X + playerWidth / 2f - Point.X - Width / 2f;

            }
            else if (target == "bandit")
            {
                y = Point.Y + Height / 2f - playerPoint.Y - playerHeight / 2f;
                x = Point.X + Width / 2f - playerPoint.X - playerWidth / 2f;
            }
            else if (target == "block")
            {
                var block = GetFastestPath();
                y = block.Y + 50 - Point.Y - Height / 2f;
                x = block.X + 50 - Point.X - Width / 2f;
            }

            return (float)Math.Atan2(y, x) * 180 / (float)Math.PI;
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
            {
                Game.PlaySound("kill");
                IsDead = true;
                Game.Kills++;
            }
        }

        public void IsGettingRangeDamage()
        {
            foreach (var bullet in Game.Bullets)
            {
                var bulletPoint = new PointF(bullet.Point.X + bullet.Width / 2, bullet.Point.Y + bullet.Height / 2);
                var banditPoint = new PointF(Point.X + Width / 2, Point.Y + Height / 2);
                var distance = Math.Sqrt(Math.Pow(bulletPoint.X - banditPoint.X, 2) + Math.Pow(bulletPoint.Y - banditPoint.Y, 2));
                if (distance < 50 && !IsDead && bullet.IsAlly)
                {
                    Game.PlaySound("kill_bullet");
                    IsDead = true;
                    Game.Kills++;
                }
            }
        }

        public void IsPlayerNear()
        {
            if((Game.Player.Point.X + Game.Player.Width / 2) / 100 == AgroPoint.X && (Game.Player.Point.Y + Game.Player.Height / 2) / 100 == AgroPoint.Y)
                IsMoving = true;
        }


        public void Move()
        {
            if (!IsMoving)
                return;
            AttackTick = (AttackTick + 1) % 100;
            if (Weapon == "rifle" && AttackTick % 10 == 0)
            {
                Game.PlaySound("shoot");
                Game.Bullets.Add(new Bullet(
                    GetAngleToTarget(Game.Player.Point, Game.Player.Width, Game.Player.Height, "player"), Width / 2,
                    Height / 2, Game.Player.Point.X, Game.Player.Point.Y, Point, false));
            }

            var moveTo = GetFastestPath();
            Debug.WriteLine(moveTo);
            if (moveTo.X < Point.X)
            {
                Point = new Point(Point.X - Speed, Point.Y);
                if (IsWall())
                    Point = new Point(Point.X + Speed, Point.Y);
            }
            else if (moveTo.X > Point.X)
            {
                Point = new Point(Point.X + Speed, Point.Y);
                if (IsWall())
                    Point = new Point(Point.X - Speed, Point.Y);
            }

            if (moveTo.Y < Point.Y)
            {
                Point = new Point(Point.X, Point.Y - Speed);
                if (IsWall())
                    Point = new Point(Point.X, Point.Y + Speed);
            }
            else if (moveTo.Y > Point.Y)
            {
                Point = new Point(Point.X, Point.Y + Speed);
                if (IsWall())
                    Point = new Point(Point.X, Point.Y - Speed);
            }


        }

    }

    public class Bandit : Entity
    {
        public Bandit(Point point, Point agroPoint, string weapon, int speed)
        {
            Point = new Point(point.X, point.Y);
            Weapon = weapon;
            if (Weapon == "fist")
            {
                Sprite = Properties.Resources.bandit;
                Width = 80;
                Height = 80;
            }

            if (Weapon == "rifle")
            {
                Sprite = Properties.Resources.bandit_rifle;
                Width = 80;
                Height = 80;
            }

            Speed = speed;
            IsDead = false;
            DeadAngle = -1;
            IsMoving = false;
            AgroPoint = new Point(agroPoint.X, agroPoint.Y);
            Spawned = 0;
            AttackTick = 0;
        }

        public void Alive(Graphics g)
        {
            Move();
            OnPaint(g);
            IsGettingMeleeDamage();
            IsGettingRangeDamage();
            IsPlayerNear();
        }

        public void Dead(Graphics g)
        {
            Sprite = Properties.Resources.bandit_dead;
            Height = 150;
            Width = 150;
            Speed = 0;
            if (DeadAngle == -1)
            {
                DeadAngle = GetAngleToTarget(Game.Player.Point, Game.Player.Width, Game.Player.Height, "bandit");
            }

            OnPaint(g);
        }

        public void OnPaint(Graphics g)
        {
            var angle = IsDead
                ? DeadAngle
                : GetAngleToTarget(Game.Player.Point, Game.Player.Width, Game.Player.Height, "player");
            Rotate(g, angle);   
        }

    }
}
