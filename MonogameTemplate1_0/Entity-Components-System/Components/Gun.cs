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
    internal class Gun : GameComponent
    {
        private LocVelAccel locVelAccel;
        private Texture2D gunTexture;

        public Gun(Entity parent) : base(parent)
        {
            gunTexture = Globals.Inst.textures.get("Gun");
            locVelAccel = GetParentComponent<LocVelAccel>();
        }

        private float rotation;

        protected override void DoTick(float deltaTime)
        {
        }

        protected override void DoDraw()
        {
            Camera camera = Globals.Inst.Camera;
            Globals.Inst.spriteBatch.Draw(gunTexture, camera.WorldToScreen(locVelAccel.Location), null, Color.White, MathHelper.ToRadians(rotation), Vector2.One * 16, 1, SpriteEffects.None, 1);
        }
    }
}
