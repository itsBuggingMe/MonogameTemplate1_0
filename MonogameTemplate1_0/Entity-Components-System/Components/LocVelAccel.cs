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
    internal class LocVelAccel : GameComponent
    {
        public Vector2 PrevLocation { get; private set; }

        public Vector2 Location { get; private set; }

        public Vector2 Velocity { get; set; }

        public Vector2 Acceleration { get; set; }

        public float Rotation { get; set; }

        public const float FrictionCoeffecient = 0.97f;

        public LocVelAccel(Entity parent) : base(parent)
        {
            DebugDrawer.Debug.SetLeft(DebugString, 0);
        }

        protected override void DoTick(float deltaTime)
        {
            Velocity += Acceleration;

            PrevLocation = Location;

            Location += Velocity;

            Velocity *= 0.9f;
            Acceleration = Vector2.Zero;
        }

        public void RevertLocation()
        {
            Location = PrevLocation;
        }

        public string DebugString()
        {
            return "Location: " + Location.ToString();
        }

        public void SetLocation(Vector2 vector2)
        {
            PrevLocation = Location;
            Location = vector2;
        }
    }
}
