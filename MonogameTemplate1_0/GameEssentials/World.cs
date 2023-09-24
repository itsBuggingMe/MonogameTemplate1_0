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
    /// <summary>
    /// Main world class
    /// </summary>
    internal class World : IState
    {
        public List<GameSystem> Systems
        {
            get
            {
                return gameSystems;
            }
        }
        private List<GameSystem> gameSystems = new List<GameSystem>();

        public List<Entity> Entities
        {
            get
            {
                return entities;
            }
        }
        private List<Entity> entities = new List<Entity>();
        
        public World()
        {
            Systems.Add(new RigidBodyPhysics());
            Entities.Add(new Player());
            for(int i = 0; i < 16; i++)
            {
                Cactus cactus = new Cactus();
                cactus.GetComponent<LocVelAccel>().SetLocation(new Vector2(Random.Shared.Next(4096), Random.Shared.Next(4096)));
                Entities.Add(cactus);
            }
        }

        public void Tick(float deltaTime)
        {
            foreach(Entity entity in entities)
            {
                entity.Tick(deltaTime);
            }
            foreach (GameSystem system in Systems)
            {
                system.Tick(Entities, deltaTime);
            }
        }

        public void Draw()
        {
            foreach (Entity entity in entities)
            {
                entity.Draw();
            }
        }

        public List<string> OnStateExit()
        {
            throw new NotImplementedException("We probrably wont need this, as example game doesnt really have any states");
        }
        public void OnStateEnter(List<string> args)
        {
            throw new NotImplementedException("We probrably wont need this, as example game doesnt really have any states");
        }
    }
}
