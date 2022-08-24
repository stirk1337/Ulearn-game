using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NAudio.Wave;
using System.IO;
using System.Linq;

namespace Ulearn_game
{
    public partial class Game : Form
    {
        public static Player Player;
        public static List<Bandit> Bandits = new List<Bandit>();
        public static List<Bullet> Bullets;
        readonly Timer mainTimer = new Timer();
        public static int[,] Level;
        public static int Kills;
        public static int LevelNumber;
        public static bool IsMusic;
        private static WaveOutEvent outputSound;
        private WaveOut waveOut;
        public static bool IsTimeStop;
        public static bool IsTimeBackAfterStop;
        public static int TimeSlowTick;
        public static bool IsGameEnd;

        public Game()
        {
            InitializeComponent();

            Player = new Player();
            Bullets = new List<Bullet>();
            Kills = 0;
            IsMusic = false;
            IsTimeStop = false;
            IsGameEnd = false;
            IsTimeBackAfterStop = false;
            LevelNumber = 1;
            TimeSlowTick = 0;
            Bandits = UpdateBandits(Levels.Bandits1);
            Level = UpdateLevel(Levels.Level1);
            DoubleBuffered = true;
            mainTimer.Interval = 20;
            KeyDown += GameKeyDown;
            KeyUp += GameKeyUp;
            MouseMove += Player.UpdateAngle;
            MouseClick += Player.Attack;
            KeyUp += GameKeyControl;
            mainTimer.Tick += MainLoop;
            mainTimer.Tick += CheckIsTimeSpeeding;
            mainTimer.Start();
        }
        List<Bandit> UpdateBandits(List<Bandit> original)
        {
            var Bandits = original.Select(p => new Bandit(p.Point, p.AgroPoint, p.Weapon, p.Speed)).ToList();
            return Bandits;
        }

        int[,] UpdateLevel(int[,] original)
        {
            var Level = (int[,])original.Clone();
            return Level;
        }

        public void MainLoop(object sender, EventArgs e)
        {
            Invalidate();
        }

        public void ChangeLevelMusic(int level)
        {
            if (IsMusic)
                return;
            var dir = Directory.GetParent(Directory.GetCurrentDirectory());
            var path = Directory.GetParent(dir.ToString()).ToString();
            IsMusic = true;
            WaveFileReader reader = new WaveFileReader(path + "/src/music/level" + LevelNumber.ToString() + ".wav");
            LoopStream loop = new LoopStream(reader);
            waveOut = new WaveOut();
            waveOut.Init(loop);
            waveOut.Volume = 0.3f;
            waveOut.Play();
        }

        public void NextLevel()
        {
            waveOut.Stop();
            waveOut.Dispose();
            waveOut = null;
            IsMusic = false;
            PlaySound("LevelComplete");
            LevelNumber++;
            ChangeLevelMusic(LevelNumber);
            Kills = 0;
            IsTimeStop = false;
            IsTimeBackAfterStop = false;
            TimeSlowTick = 0;
            Player.LastAmmo = Player.Ammo;
        }

        public void CreateInterface(Graphics g)
        {
            var font = new Font("Comic Sans MS", 32);
            SolidBrush drawBrush = new SolidBrush(Color.Black);
            g.DrawString("Патроны: " + Player.Ammo.ToString(), font, drawBrush, 100,820);
            var timeReady = !IsTimeStop && !IsTimeBackAfterStop ? "ГОТОВО": "НЕ ГОТОВО";
            g.DrawString("Сменить оружие(Q)     Замедление времени(E): " + timeReady, font, drawBrush, 450, 820);
            if (IsGameEnd)
            {
                drawBrush = new SolidBrush(Color.White);
                g.DrawString("Конец игры\nАвтор: stirk\nСпасибо, что играли", font, drawBrush, 350, 300);
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            CreateMap(g);
            ChangeLevelMusic(LevelNumber);
            for (int i = Bullets.Count - 1; i >= 0; i--)
            {
               Bullets[i].OnPaint(g, i);
            }
            foreach (var bandit in Bandits)
            {
                if (!bandit.IsDead)
                {
                    bandit.Alive(g);
                }
                else
                {
                    bandit.Dead(g);
                }
            }
            if (!Player.IsDead)
            {
                Player.Alive(g);
            }
            else
            {
                Player.Dead(g);
                PlaySound("kill");
                var font = new Font("Comic Sans MS", 70);
                SolidBrush drawBrush = new SolidBrush(Color.White);
                g.DrawString("Нажми R, чтобы начать сначала", font, drawBrush, 100,400);
                mainTimer.Stop();
            }
            CreateInterface(g);
            if (Kills == Bandits.Count)
            {
                var x = (Player.Point.X + Player.Width / 2) / 100;
                var y = (Player.Point.Y + Player.Height / 2) / 100;
                if (LevelNumber == 1)
                {
                    Level[16, 6] = 3;
                    Level[16, 7] = 3;
                    if (Level[x, y] == 3)
                    {
                        NextLevel();
                        Bandits = UpdateBandits(Levels.Bandits2);
                        Level = UpdateLevel(Levels.Level2);
                        Player.Point.X = 100;
                        Player.Point.Y = 100;
                    }
                }
                else if (LevelNumber == 2)
                {
                    Level[16, 4] = 3;
                    if (Level[x, y] == 3)
                    {
                        NextLevel();
                        Bandits = UpdateBandits(Levels.Bandits3);
                        Level = UpdateLevel(Levels.Level3);
                        Player.Point.X = 100;
                        Player.Point.Y = 400;
                    }
                }
                else if (LevelNumber == 3)
                {
                    Level[16, 4] = 3;
                    if (Level[x, y] == 3)
                    {
                        NextLevel();
                        Bandits = UpdateBandits(Levels.Bandits4);
                        Level = UpdateLevel(Levels.Level4);
                        Player.Point.X = 700;
                        Player.Point.Y = 700;
                    }
                }
                else if (LevelNumber == 4)
                {
                    Level[8, 0] = 3;
                    if (Level[x, y] == 3)
                    {
                        NextLevel();
                        Bandits = UpdateBandits(Levels.Bandits5);
                        Level = UpdateLevel(Levels.Level5);
                        Player.Point.X = 800;
                        Player.Point.Y = 700;
                    }
                }
                else if (LevelNumber == 5)
                {
                    Level[8, 0] = 3;
                    Level[7, 0] = 3;
                    if (Level[x, y] == 3)
                    {
                        NextLevel();
                        Bandits = UpdateBandits(Levels.Bandits6);
                        Level = UpdateLevel(Levels.Level6);
                        Player.Point.X = 100;
                        Player.Point.Y = 400;
                    }
                }
                else if (LevelNumber == 6)
                {
                    Level[16, 4] = 3;
                    if (Level[x, y] == 3)
                    {
                        IsGameEnd = true;
                        NextLevel();
                        Bandits = UpdateBandits(Levels.Bandits7);
                        Level = UpdateLevel(Levels.Level7);
                        Player.Point.X = 800;
                        Player.Point.Y = 700;
                    }
                }
            }
        }

        public static void PlaySound(string sound)
        {
            outputSound = new WaveOutEvent();
            var dir = Directory.GetParent(Directory.GetCurrentDirectory());
            var path = Directory.GetParent(dir.ToString()).ToString();
            AudioFileReader audioFile = new AudioFileReader(path + "/src/sound/" + sound + ".wav");
            outputSound.Init(audioFile);
            outputSound.Play();
        }

        public void CreateMap(Graphics g)
        {
            for (int i = 0; i < Level.GetLength(0); i++)
            {
                for (int j = 0; j < Level.GetLength(1); j++)
                {
                    if (Level[i, j] == 1)
                        g.DrawImage(Properties.Resources.brick2, i * 100, j * 100);
                    else if (Level[i, j] == 2)
                        g.DrawImage(Properties.Resources.brick1, i * 100, j * 100);
                    else if(Level[i,j] == 3)
                        g.DrawImage(Properties.Resources.brick_go, i * 100, j * 100);
                    else if(Level[i,j] == 4)
                        g.DrawImage(Properties.Resources.brick_old, i * 100, j * 100);
                    else if (Level[i, j] == 5)
                    {
                        g.DrawImage(Properties.Resources.brick3, i * 100, j * 100);
                    }
                }
            }
        }
        public void GameKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D) { Player.Right = true; }
            if (e.KeyCode == Keys.A) { Player.Left = true; }
            if (e.KeyCode == Keys.W) { Player.Up = true; }
            if (e.KeyCode == Keys.S) { Player.Down = true; }
        }

        public void GameKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D) { Player.Right = false; }
            if (e.KeyCode == Keys.A) { Player.Left = false; }
            if (e.KeyCode == Keys.W) { Player.Up = false; }
            if (e.KeyCode == Keys.S) { Player.Down = false; }
        }

        public void CheckIsTimeSpeeding(object sender, EventArgs e)
        {
            if (!IsTimeBackAfterStop && IsTimeStop)
            {
                TimeSlowTick += 1;
                if (TimeSlowTick == 100)
                {
                    waveOut.Play();
                    IsTimeBackAfterStop = true;
                    IsTimeStop = false;
                    foreach (var bandit in Bandits)
                    {
                        bandit.Speed *= 3;
                        bandit.AttackTickDivider /= 3;
                    }

                    foreach (var bullet in Bullets)
                    {
                        bullet.SpeedX *= 3;
                        bullet.SpeedY *= 3;
                    }
                }
            }
        }

        public void ClearLevel()
        {
            Kills = 0;
            mainTimer.Start();
            Player.IsDead = false;
            Player.Right = false;
            Player.Left = false;
            Player.Up = false;
            Player.Down = false;
            Player.Speed = 10;
            TimeSlowTick = 0;
            IsTimeStop = false;
            IsTimeBackAfterStop = false;
            Bullets.Clear();
            Bandits.Clear();
            Player.Ammo = Player.LastAmmo;
        }

        public void GameKeyControl(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Q)
            {
                PlaySound("change");
                if (Player.Weapon == "punch1" || Player.Weapon == "punch2")
                    Player.Weapon = "pistol";
                else if (Player.Weapon == "pistol")
                {
                    Player.Weapon = "punch1";
                }
            }

            if (e.KeyCode == Keys.E)
            {
                if (!IsTimeStop && !IsTimeBackAfterStop)
                {
                    IsTimeStop = true;
                    waveOut.Stop();
                    PlaySound("slow");
                    foreach (var bandit in Bandits)
                    {
                        bandit.Speed /= 3;
                        bandit.AttackTickDivider *= 3;
                    }

                    foreach (var bullet in Bullets)
                    { 
                        bullet.SpeedX /= 3;
                        bullet.SpeedY /= 3;
                    }
                }
            }

            if (e.KeyCode == Keys.R)
            {
                ClearLevel();
                waveOut.Play();
                if (LevelNumber == 1)
                {
                    Player.Point.X = 200;
                    Player.Point.Y = 500;
                    Bandits = UpdateBandits(Levels.Bandits1);
                    Level = UpdateLevel(Levels.Level1);
                }
                if (LevelNumber == 2)
                {
                    Player.Point.X = 100;
                    Player.Point.Y = 100;
                    Bandits = UpdateBandits(Levels.Bandits2);
                    Level = UpdateLevel(Levels.Level2);
                }
                if (LevelNumber == 3)
                {
                    Bandits = UpdateBandits(Levels.Bandits3);
                    Level = UpdateLevel(Levels.Level3);
                    Player.Point.X = 100;
                    Player.Point.Y = 400;
                }
                if (LevelNumber == 4)
                {
                    Bandits = UpdateBandits(Levels.Bandits4);
                    Level = UpdateLevel(Levels.Level4);
                    Player.Point.X = 700;
                    Player.Point.Y = 700;
                }
                if (LevelNumber == 5)
                {
                    Bandits = UpdateBandits(Levels.Bandits5);
                    Level = UpdateLevel(Levels.Level5);
                    Player.Point.X = 800;
                    Player.Point.Y = 700;
                }
                if (LevelNumber == 6)
                {
                    Bandits = UpdateBandits(Levels.Bandits6);
                    Level = UpdateLevel(Levels.Level6);
                    Player.Point.X = 100;
                    Player.Point.Y = 400;
                }
                if (LevelNumber == 7)
                {
                    Bandits = UpdateBandits(Levels.Bandits7);
                    Level = UpdateLevel(Levels.Level7);
                    Player.Point.X = 800;
                    Player.Point.Y = 700;
                }
            }
        }
    }
}
