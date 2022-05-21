using System;
using System.Collections.Generic;
using System.Drawing;
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
            StringFormat drawFormat = new StringFormat();
            SolidBrush drawBrush = new SolidBrush(Color.Black);
            g.DrawString("Патроны: " + Player.Ammo.ToString(), font, drawBrush, 100,820);
            var timeReady = !IsTimeStop && !IsTimeBackAfterStop ? "ГОТОВО": "НЕ ГОТОВО";
            g.DrawString("Замедление времени: " + timeReady, font, drawBrush, 950, 820);
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
                            new Bandit(new Point(550, 100), new Point(4,4), "fist", 10),
                            new Bandit(new Point(550, 700), new Point(4,4), "fist", 10),

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
                else if (LevelNumber == 3)
                {
                    Level[16, 4] = 3;
                    if (Level[x, y] == 3)
                    {
                        NextLevel();
                        Bandits = new Bandit[]
                        {
                            new Bandit(new Point(100, 100), new Point(7,7), "fist", 10),
                            new Bandit(new Point(300, 100), new Point(7,7), "fist", 10),
                            new Bandit(new Point(500, 100), new Point(7,7), "fist", 10),
                            new Bandit(new Point(700, 100), new Point(7,7), "fist", 10),
                            new Bandit(new Point(900, 100), new Point(7,7), "fist", 10),
                            new Bandit(new Point(1100, 100), new Point(7,7), "fist", 10),
                            new Bandit(new Point(1300, 100), new Point(7,7), "fist", 10),
                            new Bandit(new Point(1300, 100), new Point(7,7), "fist", 10),
                            new Bandit(new Point(1300, 100), new Point(7,7), "fist", 10),
                            new Bandit(new Point(1500, 100), new Point(7,7), "fist", 10),
                            new Bandit(new Point(100, 300), new Point(7,7), "fist", 10),
                            new Bandit(new Point(100, 500), new Point(7,7), "fist", 10),
                            new Bandit(new Point(100, 700), new Point(7,7), "fist", 10),
                            new Bandit(new Point(1500, 100), new Point(7,7), "fist", 10),
                            new Bandit(new Point(1300, 300), new Point(7,7), "fist", 10),
                            new Bandit(new Point(1300, 500), new Point(7,7), "fist", 10),
                            new Bandit(new Point(1300, 100), new Point(7,7), "fist", 10),
                            new Bandit(new Point(300, 500), new Point(7,7), "fist", 10),
                            new Bandit(new Point(500, 500), new Point(7,7), "fist", 10),
                            new Bandit(new Point(900, 500), new Point(7,7), "fist", 10),
                            new Bandit(new Point(1100, 500), new Point(7,7), "fist", 10),
                        };
                        Level = new int[,]
                        {
                            {2, 2, 2, 2, 2, 2, 2, 2, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 4},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 2, 2, 2, 2, 2, 2, 2, 2},
                        };
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
                        Bandits = new Bandit[]
                        {
                            new Bandit(new Point(700,100), new Point(8,7), "fist", 10),
                            new Bandit(new Point(800,100), new Point(8,7), "fist", 10),
                            new Bandit(new Point(1000,100), new Point(9,2), "fist", 10),
                            new Bandit(new Point(1500, 100), new Point(8,7), "rifle", 3),
                            new Bandit(new Point(500,100), new Point(6,2), "fist", 10),
                            new Bandit(new Point(500,700), new Point(7,5), "fist", 10),
                            new Bandit(new Point(100, 700), new Point(8,7), "rifle",3),
                            new Bandit(new Point(1000,700), new Point(9,5), "fist", 10),
                        };
                        Level = new int[,]
                        {
                            {2, 2, 2, 2, 2, 2, 2, 2, 2},
                            {2, 1, 1, 1, 5, 1, 1, 1, 2},
                            {2, 1, 1, 1, 5, 1, 1, 1, 2},
                            {2, 1, 1, 1, 5, 1, 1, 1, 2},
                            {2, 1, 1, 1, 5, 1, 1, 1, 2},
                            {2, 1, 1, 1, 5, 1, 1, 1, 2},
                            {2, 2, 1, 2, 2, 1, 2, 2, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 4},
                            {2, 1, 1, 1, 1, 1, 1, 1, 4},
                            {2, 2, 1, 2, 2, 1, 2, 2, 2},
                            {2, 1, 1, 1, 5, 1, 1, 1, 2},
                            {2, 1, 1, 1, 5, 1, 1, 1, 2},
                            {2, 1, 1, 1, 5, 1, 1, 1, 2},
                            {2, 1, 1, 1, 5, 1, 1, 1, 2},
                            {2, 1, 1, 1, 5, 1, 1, 1, 2},
                            {2, 1, 1, 1, 5, 1, 1, 1, 2},
                            {2, 2, 2, 2, 2, 2, 2, 2, 2},
                        };
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
                        Bandits = new Bandit[]
                        {
                            new Bandit(new Point(100,100), new Point(1, 4), "fist", 10),
                            new Bandit(new Point(100,700), new Point(1, 4), "fist", 10),
                            new Bandit(new Point(300, 100), new Point(3,4), "rifle", 3),
                            new Bandit(new Point(300, 600), new Point(3,4), "rifle", 3),
                            new Bandit(new Point(700, 500), new Point(3,4), "fist", 10),
                            new Bandit(new Point(900, 100), new Point(9,6), "rifle", 3),
                            new Bandit(new Point(1100, 700), new Point(10,1), "rifle", 3),
                            new Bandit(new Point(1500, 100), new Point(10,1), "rifle", 3),
                            new Bandit(new Point(1100, 300), new Point(12,4), "fist", 10),
                            new Bandit(new Point(1100, 500), new Point(12,4), "fist", 10),
                            new Bandit(new Point(1500, 700), new Point(14,6), "fist", 10),
                        };
                        Level = new int[,]
                        {
                            {2, 2, 2, 2, 4, 2, 2, 2, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 2, 2, 2, 1, 2, 2, 2, 2},
                            {2, 1, 1, 5, 1, 5, 1, 1, 2},
                            {2, 1, 1, 5, 1, 5, 5, 1, 2},
                            {2, 1, 1, 5, 1, 1, 1, 1, 2},
                            {2, 1, 1, 5, 1, 5, 5, 1, 2},
                            {2, 1, 1, 1, 1, 1, 5, 1, 2},
                            {2, 5, 5, 5, 5, 5, 5, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 5, 5, 5, 5, 2, 2, 2},
                            {2, 1, 5, 1, 1, 1, 1, 1, 2},
                            {2, 1, 5, 5, 1, 5, 1, 5, 2},
                            {2, 1, 1, 1, 1, 5, 1, 5, 2},
                            {2, 5, 5, 5, 5, 5, 1, 5, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 2, 2, 2, 2, 2, 2, 2, 2},
                        };
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
                        Bandits = new Bandit[]
                        {
                            new Bandit(new Point(500, 600), new Point(-1, -1), "fist", 0),
                        };
                        Level = new int[,]
                        {
                            {2, 2, 2, 2, 2, 2, 2, 2, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 2, 2, 2, 2},
                            {2, 1, 1, 1, 1, 2, 1, 2, 2},
                            {2, 1, 1, 1, 1, 2, 2, 2, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 4},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 2, 2, 2, 2, 2, 2, 2, 2},
                        };
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
            //if (e.KeyCode == Keys.G)
            //{
            //    Kills = Bandits.Length;
            //    foreach (var bandit in Bandits)
            //    {
            //        bandit.IsDead = true;
            //    }
            //    Player.Speed = 35;
            //    Player.Ammo = 100;
            //}
            //END OF DEBUG BUTTON


            if (e.KeyCode == Keys.R)
            {
                ClearLevel();
                waveOut.Play();
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
                }

                if (LevelNumber == 2)
                {
                    Player.Point.X = 100;
                    Player.Point.Y = 100;
                    Bandits = new Bandit[]
                    {
                        new Bandit(new Point(1500, 100), new Point(4, 4), "rifle", 0),
                        new Bandit(new Point(1500, 700), new Point(4, 4), "rifle", 0),
                        new Bandit(new Point(550, 100), new Point(4,4), "fist", 10),
                        new Bandit(new Point(550, 700), new Point(4,4), "fist", 10),
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

                if (LevelNumber == 4)
                {
                    Bandits = new Bandit[]
                        {
                            new Bandit(new Point(100, 100), new Point(7,7), "fist", 10),
                            new Bandit(new Point(300, 100), new Point(7,7), "fist", 10),
                            new Bandit(new Point(500, 100), new Point(7,7), "fist", 10),
                            new Bandit(new Point(700, 100), new Point(7,7), "fist", 10),
                            new Bandit(new Point(900, 100), new Point(7,7), "fist", 10),
                            new Bandit(new Point(1100, 100), new Point(7,7), "fist", 10),
                            new Bandit(new Point(1300, 100), new Point(7,7), "fist", 10),
                            new Bandit(new Point(1300, 100), new Point(7,7), "fist", 10),
                            new Bandit(new Point(1300, 100), new Point(7,7), "fist", 10),
                            new Bandit(new Point(1500, 100), new Point(7,7), "fist", 10),
                            new Bandit(new Point(100, 300), new Point(7,7), "fist", 10),
                            new Bandit(new Point(100, 500), new Point(7,7), "fist", 10),
                            new Bandit(new Point(100, 700), new Point(7,7), "fist", 10),
                            new Bandit(new Point(1500, 100), new Point(7,7), "fist", 10),
                            new Bandit(new Point(1300, 300), new Point(7,7), "fist", 10),
                            new Bandit(new Point(1300, 500), new Point(7,7), "fist", 10),
                            new Bandit(new Point(1300, 100), new Point(7,7), "fist", 10),
                            new Bandit(new Point(300, 500), new Point(7,7), "fist", 10),
                            new Bandit(new Point(500, 500), new Point(7,7), "fist", 10),
                            new Bandit(new Point(900, 500), new Point(7,7), "fist", 10),
                            new Bandit(new Point(1100, 500), new Point(7,7), "fist", 10),
                        };
                    Level = new int[,]
                    {
                            {2, 2, 2, 2, 2, 2, 2, 2, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 4},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 1, 1, 1, 1, 1, 1, 1, 2},
                            {2, 2, 2, 2, 2, 2, 2, 2, 2},
                    };
                    Player.Point.X = 700;
                    Player.Point.Y = 700;
                }

                if (LevelNumber == 5)
                {
                    Bandits = new Bandit[]
                    {
                        new Bandit(new Point(700,100), new Point(8,7), "fist", 10),
                        new Bandit(new Point(800,100), new Point(8,7), "fist", 10),
                        new Bandit(new Point(1000,100), new Point(9,2), "fist", 10),
                        new Bandit(new Point(1500, 100), new Point(8,7), "rifle", 3),
                        new Bandit(new Point(500,100), new Point(6,2), "fist", 10),
                        new Bandit(new Point(500,700), new Point(7,5), "fist", 10),
                        new Bandit(new Point(100, 700), new Point(8,7), "rifle",3),
                        new Bandit(new Point(1000,700), new Point(9,5), "fist", 10),
                    };
                    Level = new int[,]
                    {
                        {2, 2, 2, 2, 2, 2, 2, 2, 2},
                        {2, 1, 1, 1, 5, 1, 1, 1, 2},
                        {2, 1, 1, 1, 5, 1, 1, 1, 2},
                        {2, 1, 1, 1, 5, 1, 1, 1, 2},
                        {2, 1, 1, 1, 5, 1, 1, 1, 2},
                        {2, 1, 1, 1, 5, 1, 1, 1, 2},
                        {2, 2, 1, 2, 2, 1, 2, 2, 2},
                        {2, 1, 1, 1, 1, 1, 1, 1, 4},
                        {2, 1, 1, 1, 1, 1, 1, 1, 4},
                        {2, 2, 1, 2, 2, 1, 2, 2, 2},
                        {2, 1, 1, 1, 5, 1, 1, 1, 2},
                        {2, 1, 1, 1, 5, 1, 1, 1, 2},
                        {2, 1, 1, 1, 5, 1, 1, 1, 2},
                        {2, 1, 1, 1, 5, 1, 1, 1, 2},
                        {2, 1, 1, 1, 5, 1, 1, 1, 2},
                        {2, 1, 1, 1, 5, 1, 1, 1, 2},
                        {2, 2, 2, 2, 2, 2, 2, 2, 2},
                    };
                    Player.Point.X = 800;
                    Player.Point.Y = 700;
                }

                if (LevelNumber == 6)
                {
                    Bandits = new Bandit[]
                    {
                        new Bandit(new Point(100,100), new Point(1, 4), "fist", 10),
                        new Bandit(new Point(100,700), new Point(1, 4), "fist", 10),
                        new Bandit(new Point(300, 100), new Point(3,4), "rifle", 3),
                        new Bandit(new Point(300, 600), new Point(3,4), "rifle", 3),
                        new Bandit(new Point(700, 500), new Point(3,4), "fist", 10),
                        new Bandit(new Point(900, 100), new Point(9,6), "rifle", 3),
                        new Bandit(new Point(1100, 700), new Point(10,1), "rifle", 3),
                        new Bandit(new Point(1500, 100), new Point(10,1), "rifle", 3),
                        new Bandit(new Point(1100, 300), new Point(12,4), "fist", 10),
                        new Bandit(new Point(1100, 500), new Point(12,4), "fist", 10),
                        new Bandit(new Point(1500, 700), new Point(14,6), "fist", 10),
                    };
                    Level = new int[,]
                    {
                        {2, 2, 2, 2, 4, 2, 2, 2, 2},
                        {2, 1, 1, 1, 1, 1, 1, 1, 2},
                        {2, 2, 2, 2, 1, 2, 2, 2, 2},
                        {2, 1, 1, 5, 1, 5, 1, 1, 2},
                        {2, 1, 1, 5, 1, 5, 5, 1, 2},
                        {2, 1, 1, 5, 1, 1, 1, 1, 2},
                        {2, 1, 1, 5, 1, 5, 5, 1, 2},
                        {2, 1, 1, 1, 1, 1, 5, 1, 2},
                        {2, 5, 5, 5, 5, 5, 5, 1, 2},
                        {2, 1, 1, 1, 1, 1, 1, 1, 2},
                        {2, 1, 5, 5, 5, 5, 2, 2, 2},
                        {2, 1, 5, 1, 1, 1, 1, 1, 2},
                        {2, 1, 5, 5, 1, 5, 1, 5, 2},
                        {2, 1, 1, 1, 1, 5, 1, 5, 2},
                        {2, 5, 5, 5, 5, 5, 1, 5, 2},
                        {2, 1, 1, 1, 1, 1, 1, 1, 2},
                        {2, 2, 2, 2, 2, 2, 2, 2, 2},
                    };
                    Player.Point.X = 100;
                    Player.Point.Y = 400;
                }

                if (LevelNumber == 7)
                {
                    Bandits = new Bandit[]
                    {
                        new Bandit(new Point(500, 600), new Point(-1, -1), "fist", 0),
                    };
                    Level = new int[,]
                    {
                        {2, 2, 2, 2, 2, 2, 2, 2, 2},
                        {2, 1, 1, 1, 1, 1, 1, 1, 2},
                        {2, 1, 1, 1, 1, 1, 1, 1, 2},
                        {2, 1, 1, 1, 1, 1, 1, 1, 2},
                        {2, 1, 1, 1, 1, 2, 2, 2, 2},
                        {2, 1, 1, 1, 1, 2, 1, 2, 2},
                        {2, 1, 1, 1, 1, 2, 2, 2, 2},
                        {2, 1, 1, 1, 1, 1, 1, 1, 2},
                        {2, 1, 1, 1, 1, 1, 1, 1, 4},
                        {2, 1, 1, 1, 1, 1, 1, 1, 2},
                        {2, 1, 1, 1, 1, 1, 1, 1, 2},
                        {2, 1, 1, 1, 1, 1, 1, 1, 2},
                        {2, 1, 1, 1, 1, 1, 1, 1, 2},
                        {2, 1, 1, 1, 1, 1, 1, 1, 2},
                        {2, 1, 1, 1, 1, 1, 1, 1, 2},
                        {2, 1, 1, 1, 1, 1, 1, 1, 2},
                        {2, 2, 2, 2, 2, 2, 2, 2, 2},
                    };
                    Player.Point.X = 800;
                    Player.Point.Y = 700;
                }
            }

        }

    }

}
