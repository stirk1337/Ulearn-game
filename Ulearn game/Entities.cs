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
        public Image Sprite;
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
