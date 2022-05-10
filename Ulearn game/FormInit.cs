using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;
using System.IO;

namespace Ulearn_game
{
    public partial class Game : Form
    {
        public static Player Player;
        public static Bandit[] Bandits;
        public static List<Bullet> Bullets;
        public static List<Bullet> AngryBullets;
        Timer mainTimer = new Timer();
        public static int[,] Level;
        public static int Kills;
        public static int LevelNumber;
        public static bool IsMusic;
        private static WaveOutEvent outputSound;
        private WaveOut waveOut;
        public static bool IsTimeStop;
        public static bool IsTimeBackAfterStop;
        public static int TimeSlowTick;
        public Game()
        {
            InitializeComponent();

            Player = new Player();
            Bullets = new List<Bullet>();
            Kills = 0;
            IsMusic = false;
            IsTimeStop = false;
            IsTimeBackAfterStop = false;
            LevelNumber = 1;
            TimeSlowTick = 0;
            Bandits = new Bandit[]
            {
                new Bandit(new Point(550,550), new Point(3,1), "fist", 10),
                new Bandit(new Point(1550, 650), new Point(7,6), "fist", 10), 
                new Bandit(new Point(850, 150), new Point(11,5), "fist", 10),
                new Bandit(new Point(1450, 150), new Point(11,6), "fist", 10),
                new Bandit(new Point(850, 350), new Point(11, 5), "fist", 10),
                new Bandit(new Point(1450, 350), new Point(11,5), "fist", 10),
                
            };
            Level = new int[,]
            {
                {2, 2, 2, 2, 2, 2, 2, 2, 2},
                {2, 1, 1, 1, 1, 1, 1, 1, 2},
                {2, 1, 1, 1, 1, 1, 1, 1, 2},
                {2, 1, 2, 2, 2, 2, 2, 2, 2},
                {2, 1, 1, 1, 1, 1, 1, 1, 2},
                {2, 1, 1, 1, 1, 1, 1, 1, 2},
                {2, 1, 1, 1, 1, 1, 1, 1, 2},
                {2, 2, 2, 2, 2, 2, 1, 2, 2},
                {2, 1, 1, 1, 1, 2, 1, 1, 2},
                {2, 1, 1, 1, 1, 2, 1, 1, 2},
                {2, 1, 1, 1, 1, 2, 1, 1, 2},
                {2, 1, 1, 1, 1, 1, 1, 1, 2},
                {2, 1, 1, 1, 1, 2, 1, 1, 2},
                {2, 1, 1, 1, 1, 2, 1, 1, 2},
                {2, 1, 1, 1, 1, 2, 1, 1, 2},
                {2, 1, 1, 1, 1, 2, 1, 1, 2},
                {2, 2, 2, 2, 2, 2, 2, 2, 2},
            };
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
        
        public void MainLoop(object sender, EventArgs e)
        {
            Invalidate();
        }

        public class LoopStream : WaveStream
        {
            WaveStream sourceStream;
            public LoopStream(WaveStream sourceStream)
            {
                this.sourceStream = sourceStream;
                this.EnableLooping = true;
            }
            public bool EnableLooping { get; set; }
            public override WaveFormat WaveFormat
            {
                get { return sourceStream.WaveFormat; }
            }
            public override long Length
            {
                get { return sourceStream.Length; }
            }
            public override long Position
            {
                get { return sourceStream.Position; }
                set { sourceStream.Position = value; }
            }
            public override int Read(byte[] buffer, int offset, int count)
            {
                int totalBytesRead = 0;

                while (totalBytesRead < count)
                {
                    int bytesRead = sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);
                    if (bytesRead == 0)
                    {
                        if (sourceStream.Position == 0 || !EnableLooping)
                        {
                            break;
                        }
                        sourceStream.Position = 0;
                    }
                    totalBytesRead += bytesRead;
                }
                return totalBytesRead;
            }
        }

        public void PlayMusic(int level)
        {
            var dir = Directory.GetParent(Directory.GetCurrentDirectory());
            var path = Directory.GetParent(dir.ToString()).ToString();
            if (level == 1 && !IsMusic)
            {
                IsMusic = true;
                WaveFileReader reader = new WaveFileReader(path + "/src/music/level1.wav");
                LoopStream loop = new LoopStream(reader);
                waveOut = new WaveOut();
                waveOut.Init(loop);
                waveOut.Play();
            }

            if (level == 2 && !IsMusic)
            {
                IsMusic = true;
                WaveFileReader reader = new WaveFileReader(path + "/src/music/level2.wav");
                LoopStream loop = new LoopStream(reader);
                waveOut = new WaveOut();
                waveOut.Init(loop);
                waveOut.Play();
            }

            if (level == 3 && !IsMusic)
            {
                IsMusic = true;
                WaveFileReader reader = new WaveFileReader(path + "/src/music/level3.wav");
                LoopStream loop = new LoopStream(reader);
                waveOut = new WaveOut();
                waveOut.Init(loop);
                waveOut.Play();
            }
        }

        public void NextLevel()
        {
            waveOut.Stop();
            waveOut.Dispose();
            waveOut = null;
            IsMusic = false;
            PlaySound("LevelComplete");
            LevelNumber++;
            PlayMusic(LevelNumber);
            Kills = 0;
            IsTimeStop = false;
            IsTimeBackAfterStop = false;
            TimeSlowTick = 0;
            Player.LastAmmo = Player.Ammo;
        }

        public void CreateInterface(Graphics g)
        {
            var font = new Font("Comic Sans MS", 32);
            StringFormat drawFormat = new StringFormat();
            SolidBrush drawBrush = new SolidBrush(Color.Black);
            g.DrawString("Патроны: " + Player.Ammo.ToString(), font, drawBrush, 100,820);
            var timeReady = !IsTimeStop && !IsTimeBackAfterStop ? "ГОТОВО": "НЕ ГОТОВО";
            g.DrawString("Замедление времени: " + timeReady, font, drawBrush, 950, 820);
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            CreateMap(g);
            PlayMusic(LevelNumber);
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
                StringFormat drawFormat = new StringFormat();
                SolidBrush drawBrush = new SolidBrush(Color.White);
                g.DrawString("Нажми R, чтобы начать сначала", font, drawBrush, 100,400);
                mainTimer.Stop();
            }
            CreateInterface(g);

            if (Kills == Bandits.Length)
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
                        Bandits = new Bandit[]
                        {
                            new Bandit(new Point(1500, 100), new Point(4, 4), "rifle", 0),
                            new Bandit(new Point(1500, 700), new Point(4, 4), "rifle", 0),
                            new Bandit(new Point(550, 100), new Point(5,4), "fist", 10),
                            new Bandit(new Point(550, 700), new Point(5,4), "fist", 10),
                        };
                        Level = new int[,]
                        {
                            {2, 4, 4, 2, 2, 2, 2, 2, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 2, 2, 2, 1, 2, 2, 2, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2}, 
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 2, 2, 2, 2, 2, 2, 2, 2},
                        };
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
                        Bandits = new Bandit[]
                        {
                            new Bandit(new Point(1500, 700), new Point(1, 4), "rifle", 0),
                            new Bandit(new Point(1500, 100), new Point(1,4), "rifle", 0),
                            new Bandit(new Point(100,100), new Point(1,4), "fist", 10),
                            new Bandit(new Point(100, 700), new Point(1,4), "fist", 10),
                            new Bandit(new Point(1100, 700), new Point(9,4), "fist", 10),
                            new Bandit(new Point(1100, 100), new Point(9,4), "fist", 10),
                        };
                        Level = new int[,]
                        {
                            {2, 2, 2, 2, 4, 2, 2, 2, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 2, 1, 2, 1, 2, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 2, 1, 2, 1, 2, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 2, 1, 2, 1, 2, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 5, 5, 2, 1, 2, 5, 5, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 5, 5, 1, 1, 1, 5, 5, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 2, 2, 2, 2, 2, 2, 2, 2},
                        };
                        Player.Point.X = 100;
                        Player.Point.Y = 400;
                    }
                }
            }
        }

        public static void PlaySound(string sound)
        {
            outputSound = new WaveOutEvent();
            var dir = Directory.GetParent(Directory.GetCurrentDirectory());
            var path = Directory.GetParent(dir.ToString()).ToString();
            if (sound == "kill")
            {
                var audioFile = new AudioFileReader(path + "/src/sound/kill.wav");
                outputSound.Init(audioFile);
            }

            if (sound == "punch")
            {
                var audioFile = new AudioFileReader(path + "/src/sound/punch.wav");
                outputSound.Init(audioFile);
            }

            if (sound == "shoot")
            {
                var audioFile = new AudioFileReader(path + "/src/sound/pistol.wav");
                audioFile.Volume = 0.6f;
                outputSound.Init(audioFile);
            }

            if (sound == "HitWall")
            {
                var audioFile = new AudioFileReader(path + "/src/sound/HitWall.wav");
                outputSound.Init(audioFile);
            }

            if (sound == "kill_bullet")
            {
                var audioFile = new AudioFileReader(path + "/src/sound/kill_bullet.wav");
                outputSound.Init(audioFile);
            }

            if (sound == "LevelComplete")
            {
                var audioFile = new AudioFileReader(path + "/src/sound/LevelComplete.wav");
                outputSound.Init(audioFile);
            }

            if (sound == "change")
            {
                var audioFile = new AudioFileReader(path + "/src/sound/change.wav");
                outputSound.Init(audioFile);
            }

            if (sound == "slow")
            {
                var audioFile = new AudioFileReader(path + "/src/sound/slow.wav");
                outputSound.Init(audioFile);
            }

            if (sound == "noAmmo")
            {
                var audioFile = new AudioFileReader(path + "/src/sound/no_ammo.wav");
                outputSound.Init(audioFile);
            }
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
                        g.DrawImage(Properties.Resources.brick1, i * 100, j * 100);
                        g.DrawImage(Properties.Resources.glass, i * 100, j * 100);
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


            //DEBUG BUTTON
            if (e.KeyCode == Keys.G)
            {
                Kills = Bandits.Length;
                foreach (var bandit in Bandits)
                {
                    bandit.IsDead = true;
                }
                Player.Speed = 35;
                Player.Ammo = 100;
            }

            //END OF DEBUG BUTTON


            if (e.KeyCode == Keys.R)
            {
                ClearLevel();
                if (LevelNumber == 1)
                {
                    Player.Point.X = 200;
                    Player.Point.Y = 500;
                    Bandits = new Bandit[]
                    {
                        new Bandit(new Point(550,550), new Point(3,1), "fist", 10),
                        new Bandit(new Point(1550, 650), new Point(7,6), "fist", 10),
                        new Bandit(new Point(850, 150), new Point(11,5), "fist", 10),
                        new Bandit(new Point(1450, 150), new Point(11,6), "fist", 10),
                        new Bandit(new Point(850, 350), new Point(11, 5), "fist", 10),
                        new Bandit(new Point(1450, 350), new Point(11,5), "fist", 10),

                    };
                }

                if (LevelNumber == 2)
                {
                    Player.Point.X = 100;
                    Player.Point.Y = 100;
                    Bandits = new Bandit[]
                    {
                        new Bandit(new Point(1500, 100), new Point(4, 4), "rifle", 0),
                        new Bandit(new Point(1500, 700), new Point(4, 4), "rifle", 0),
                        new Bandit(new Point(550, 100), new Point(5,4), "fist", 10),
                        new Bandit(new Point(550, 700), new Point(5,4), "fist", 10),
                    };
                }

                if (LevelNumber == 3)
                {
                    Bandits = new Bandit[]
                    {
                        new Bandit(new Point(1500, 700), new Point(1, 4), "rifle", 0),
                        new Bandit(new Point(1500, 100), new Point(1,4), "rifle", 0),
                        new Bandit(new Point(100,100), new Point(1,4), "fist", 10),
                        new Bandit(new Point(100, 700), new Point(1,4), "fist", 10),
                        new Bandit(new Point(1100, 700), new Point(9,4), "fist", 10),
                        new Bandit(new Point(1100, 100), new Point(9,4), "fist", 10),
                    };
                    Player.Point.X = 100;
                    Player.Point.Y = 400;
                }

            }

        }

    }

}
