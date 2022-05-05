using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;

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
        private static WaveOutEvent outputDevice;
        private static AudioFileReader audioFile;
        public Game()
        {
            InitializeComponent();

            Player = new Player();
            Bullets = new List<Bullet>();
            Kills = 0;
            LevelNumber = 1;
            Bandits = new Bandit[]
            {
                new Bandit(new Point(550,550), new Point(3,1)),
                new Bandit(new Point(1550, 650), new Point(7,6)), 
                new Bandit(new Point(850, 150), new Point(11,5)),
                new Bandit(new Point(1450, 150), new Point(11,6)),
                new Bandit(new Point(850, 350), new Point(11, 5)),
                new Bandit(new Point(1450, 350), new Point(11,5)),
                
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
            KeyUp += GameKeyChangeWeapon;
            mainTimer.Tick += MainLoop;
            mainTimer.Start();
        }
        
        public void MainLoop(object sender, EventArgs e)
        {
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            CreateMap(g);
            for(int i = Bullets.Count - 1; i >= 0; i--)
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
                mainTimer.Stop();
            }

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
                        PlaySound("LevelComplete");
                        LevelNumber++;
                        Kills = 0;
                        Bandits = new Bandit[]
                        {
                            new Bandit(new Point(0, 0), new Point(0, 0)),
                        };
                        Level = new int[,]
                        {
                            {2, 4, 4, 2, 2, 2, 2, 2, 2},
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
            }
        }

        public static void PlaySound(string sound)
        {
            outputDevice = new WaveOutEvent();
            if (sound == "kill")
            {
                audioFile = new AudioFileReader(@"C:\Users\stirk\source\repos\Ulearn game\Ulearn game\src\sound\kill.wav");
                outputDevice.Init(audioFile);
            }

            if (sound == "punch")
            {
                audioFile = new AudioFileReader(@"C:\Users\stirk\source\repos\Ulearn game\Ulearn game\src\sound\punch.wav");
                outputDevice.Init(audioFile);
            }

            if (sound == "shoot")
            {
                audioFile = new AudioFileReader(@"C:\Users\stirk\source\repos\Ulearn game\Ulearn game\src\sound\pistol.wav");
                outputDevice.Init(audioFile);
            }

            if (sound == "HitWall")
            {
                audioFile = new AudioFileReader(@"C:\Users\stirk\source\repos\Ulearn game\Ulearn game\src\sound\HitWall.wav");
                outputDevice.Init(audioFile);
            }

            if (sound == "kill_bullet")
            {
                audioFile = new AudioFileReader(@"C:\Users\stirk\source\repos\Ulearn game\Ulearn game\src\sound\kill_bullet.wav");
                outputDevice.Init(audioFile);
            }

            if (sound == "LevelComplete")
            {
                audioFile = new AudioFileReader(@"C:\Users\stirk\source\repos\Ulearn game\Ulearn game\src\sound\LevelComplete.wav");
                outputDevice.Init(audioFile);
            }

            if (sound == "change")
            {
                audioFile = new AudioFileReader(@"C:\Users\stirk\source\repos\Ulearn game\Ulearn game\src\sound\change.wav");
                outputDevice.Init(audioFile);
            }
            outputDevice.Play();
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

        public void GameKeyChangeWeapon(object sender, KeyEventArgs e)
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
        }
    }

}
