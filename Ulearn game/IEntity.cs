using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ulearn_game
{
    interface IEntity
    {
        Bitmap Sprite { get; set; }
        string Weapon { get; set; }
        int Width { get; set; }
        int Height { get; set; }
        Point Point { get; set; }   
        int Speed { get; set; }
        bool IsDead { get; set; }
        float DeadAngle { get; set; }
        float GetAngleToTarget(Point targetPoint, int targetWidth, int targetHeight, string target);
        void OnPaint(Graphics g);
        void IsGettingMeleeDamage();
        void Alive(Graphics g);
        void Dead(Graphics g);
        void Move();
        void PlayAnimation();

        bool IsWall();

    }
}
