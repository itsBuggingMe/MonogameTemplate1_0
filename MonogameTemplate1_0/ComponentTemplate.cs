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
    internal class ComponentTemplate : GameComponent
    {
        //private GameComponent otherComponentNeededForThis;

        public ComponentTemplate(Entity parent) : base(parent)
        {
            //locVelAccel = GetParentComponent<LocVelAccel>();
        }
        protected override void DoTick(float deltaTime)
        {

        }

        protected override void DoDraw()
        {

        }
    }
}
