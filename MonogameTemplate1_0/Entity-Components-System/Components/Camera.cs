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
    internal class CameraComponent : GameComponent
    {
        private LocVelAccel locVelAccel;

        public CameraComponent(Entity parent) : base(parent)
        {
            locVelAccel = GetParentComponent<LocVelAccel>();
        }
        protected override void DoTick(float deltaTime)
        {
            Camera camera = Globals.Inst.Camera;
            Vector2 pos = camera.Position;
            Vector2 want = locVelAccel.Location;
            Vector2 travel = want - pos;
            travel *= 0.2f;
            camera.SetLocation(travel + camera.Position);
        }

        protected override void DoDraw()
        {

        }
    }
}
