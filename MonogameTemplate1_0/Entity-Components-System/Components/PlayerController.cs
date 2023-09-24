using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonogameTemplate1_0
{
    internal class PlayerController : GameComponent
    {
        private LocVelAccel _component;

        public PlayerController(Entity parent) : base(parent)
        {
            _component = GetParentComponent<LocVelAccel>();
        }

        protected override void DoTick(float deltaTime)
        {
            Vector2 Acceleration = Vector2.Zero;
            if (InputHelper.Down(Keys.W))
            {
                Acceleration -= Vector2.UnitY;
            }
            if (InputHelper.Down(Keys.S))
            {
                Acceleration += Vector2.UnitY;
            }

            if (InputHelper.Down(Keys.A))
            {
                Acceleration -= Vector2.UnitX;

            }
            if (InputHelper.Down(Keys.D))
            {
                Acceleration += Vector2.UnitX;
            }
            if (Acceleration != Vector2.Zero)
            {
                Acceleration = MathFunc.SetDistance(1, Acceleration);
            }
            _component.Acceleration = Acceleration;
        }
    }
}
