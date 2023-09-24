using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using FontStashSharp;
using MonoGame.Extended.BitmapFonts;
using System.Security.Cryptography;

namespace MonogameTemplate1_0
{
    internal class DebugDrawer
    {
        private static DebugDrawer _debug;
        public static DebugDrawer Debug
        {
            get
            {
                _debug ??= new DebugDrawer();
                return _debug;
            }
        }

        const int Rows = 16;

        public delegate string GetDebugString();

        private GetDebugString[] leftSide = new GetDebugString[Rows];
        private GetDebugString[] rightSide = new GetDebugString[Rows];

        private float scale = 1;

        private bool enabled = false;

        public bool IsDebugging
        {
            get
            {
                return enabled;
            }
        }
        public static readonly Color drawColor = Color.Black;

        private DebugDrawer() 
        {

        }

        public void Draw()
        {
            if(InputHelper.RisingEdge(Keys.F3))
            {
                enabled = !enabled;
            }

            if(!enabled)
            {
                return;
            }

            float drawHeight = Globals.Inst.Font.MeasureString("O").Y * 1.2f;

            for(int i = 0; i < Rows; i++)
            {
                if(leftSide[i] == null)
                {
                    Globals.Inst.spriteBatch.DrawString(Globals.Inst.Font, "null", new Vector2(4, drawHeight * i), drawColor);
                }
                else
                {
                    Globals.Inst.spriteBatch.DrawString(Globals.Inst.Font, leftSide[i](), new Vector2(4, drawHeight * i), drawColor);
                }
            }
        }
        public void SetLeft(GetDebugString left, int column)
        {
            leftSide[column] = left;
        }
        public void SetRight(GetDebugString right, int column)
        {
            rightSide[column] = right;
        }
    }
}
