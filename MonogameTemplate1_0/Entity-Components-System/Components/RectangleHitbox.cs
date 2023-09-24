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
    internal class RectangleHitbox : GameComponent
    {
        private LocVelAccel LocVelAccel;

        private Rectangle hitbox;

        public Rectangle Hitbox
        {
            get { return hitbox; }
        }

        private Point size;
        private Point offset;

        public RectangleHitbox(Entity parent, Point size, Point? offset) : base(parent)
        {
            LocVelAccel = GetParentComponent<LocVelAccel>();

            this.size = size;

            if(offset.HasValue)
            {
                this.offset = offset.Value;
            }
            else
            {
                this.offset = new Point(-size.X / 2, -size.Y / 2);
            }

            this.hitbox = new Rectangle(LocVelAccel.Location.ToPoint() + this.offset, size);
        }

        public RectangleHitbox(Entity parent, string sizeTexture, Point? offset, float scale = 1) : base(parent)
        {
            LocVelAccel = GetParentComponent<LocVelAccel>();
            Texture2D texture2D = Globals.Inst.textures.get(sizeTexture);
            this.size = new Vector2(texture2D.Width * scale, texture2D.Height * scale).ToPoint();

            if (offset.HasValue)
            {
                this.offset = offset.Value;
            }
            else
            {
                this.offset = new Point(-size.X / 2, -size.Y / 2);
            }

            this.hitbox = new Rectangle(LocVelAccel.Location.ToPoint() + this.offset, size);
        }

        protected override void DoTick(float deltaTime)
        {
            hitbox = new Rectangle(LocVelAccel.Location.ToPoint() + offset, size);
        }

        protected override void DoDraw()
        {
            if(DebugDrawer.Debug.IsDebugging)
            {
                Camera camera = Globals.Inst.Camera;

                Globals.Inst.postShapeBatch.BorderRectangle(
                    camera.WorldToScreen(hitbox.Location.ToVector2()),
                    hitbox.Size.ToVector2() * camera.Zoom, 
                    DebugDrawer.drawColor
                    );
            }
        }
    }
}
