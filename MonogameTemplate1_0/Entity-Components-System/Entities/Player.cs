using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Apos.Input;
using System.ComponentModel;

namespace MonogameTemplate1_0
{
    internal class Player : Entity
    {
        const int playerHitboxWidth = 256, playerHitboxHeight = 256;

        public Player() 
        {
            //For the movement
            AddComponent(new LocVelAccel(this));

            //For the camera, updates its position
            AddComponent(new CameraComponent(this));

            //For collisions, attachted to movement
            AddComponent(new RectangleHitbox(this, new Point(playerHitboxWidth, playerHitboxHeight),null));
            //For what to move, attached to movement
            AddComponent(new PlayerController(this));
            //Display sprite
            AddComponent(new GlobalSprite(this, "Dino", drawOrigin: DrawOrigin.Center, spriteSheetSize: new Point(3, 1)), out var comp);
            comp.SetOnTick(comp.OnSpriteTick);
            //Display gun
            AddComponent(new GlobalSprite(this, "Gun", new Vector2(0.1f, 0.2f), offset: new Vector2(84,-8), spriteSheetSize: new Point(2,1)),out var comp1);
            comp1.SetOnTick(comp1.OnGunSpriteTick);

            //AddComponent(new Gun(this));
        }
    }
}
