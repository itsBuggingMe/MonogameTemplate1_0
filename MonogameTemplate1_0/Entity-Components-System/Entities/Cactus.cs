using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonogameTemplate1_0
{
    internal class Cactus : Entity
    {
        public Cactus() 
        {
            AddComponent(new LocVelAccel(this));
            AddComponent(new RectangleHitbox(this, "Cactus", null, scale: 2));
            AddComponent(new GlobalSprite(this, "Cactus", DrawOrigin.Center, Scale: 2));
        }
    }
}
