using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ulearn_game
{ 
    class Entity
    {
        public int Health;
        public int Damage;
        public Bitmap Sprite;
        public string Weapon;
    }
    class Bandit : Entity
    {
        public Bandit()
        {
            Health = 20;
            Damage = 10;
        }
    }

}
