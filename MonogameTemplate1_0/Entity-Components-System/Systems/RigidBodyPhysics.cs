using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;

namespace MonogameTemplate1_0
{
    internal class RigidBodyPhysics : GameSystem
    {
        public RigidBodyPhysics()
        {
            DebugDrawer.Debug.SetLeft(GetDebugCollison,1);
        }

        protected override bool HasControlOver(Entity entity)
        {
            return TryGetComponent<RectangleHitbox>(entity, out _);
        }

        bool collisionExists = false;

        protected override void ForEachEntity(Entity entity, List<Entity> entities, float deltaTime)
        {
            TryGetComponent(entity, out RectangleHitbox selfHitbox);

            foreach(Entity otherEntity in entities)
            {
                if(otherEntity == entity)
                {
                    continue;
                }
                TryGetComponent(otherEntity, out RectangleHitbox otherHitbox);

                if (selfHitbox.Hitbox.Intersects(otherHitbox.Hitbox))
                {
                    collisionExists = true;
                    TryGetComponent<LocVelAccel>(entity, out var component);
                    component.Velocity *= 0.4f;
                    component.RevertLocation();
                }
            }
        }

        protected override void OnTickStart(float deltaTime)
        {
            collisionExists = false;
        }

        private string GetDebugCollison()
        {
            return "collisionExists: " + collisionExists;
        }
    }
}
