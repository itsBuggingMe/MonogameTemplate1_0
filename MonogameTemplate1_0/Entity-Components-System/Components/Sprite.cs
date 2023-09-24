using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonogameTemplate1_0
{
    internal class GlobalSprite : GameComponent
    {
        public LocVelAccel LocVelAccel { get; private set; }
        private Texture2D _texture;

        private float scale = 1;

        private readonly Vector2 origin;

        private readonly Vector2 size;

        private Camera camera = Globals.Inst.Camera;

        private Point spriteSheetSize = new Point(1,1);

        public uint Frame
        {
            get
            {
                return _frame;
            }
            set
            {
                _frame = (uint)(value % (spriteSheetSize.X * spriteSheetSize.Y));
            }
        }

        private uint _frame = 0;

        private SpriteEffects _spriteEffect;
        public SpriteEffects SpriteEffect { get { return _spriteEffect; } set
            {
                if(value == _spriteEffect)
                {
                    return;
                }
                
                if ((_spriteEffect == SpriteEffects.None && value == SpriteEffects.FlipHorizontally) ||
                    (_spriteEffect == SpriteEffects.FlipHorizontally && value == SpriteEffects.None))
                {
                    offset *= new Vector2(-1, 1);
                }
                else if ((_spriteEffect == SpriteEffects.None && value == SpriteEffects.FlipVertically) ||
                         (_spriteEffect == SpriteEffects.FlipVertically && value == SpriteEffects.None))
                {
                    offset *= new Vector2(1, -1);
                }

                _spriteEffect = value;
            }
        }

        public Vector2 offset;

        public GlobalSprite(Entity parent, string texture, DrawOrigin drawOrigin = DrawOrigin.TopLeft, float Scale = 1, Point? spriteSheetSize = null, Vector2 offset = default) : base(parent)
        {
            _texture = Globals.Inst.textures.get(texture);
            LocVelAccel = GetParentComponent<LocVelAccel>();
            this.scale = Scale;

            SpriteEffect = SpriteEffects.None;

            if (spriteSheetSize.HasValue)
            {
                if (_texture.Width % spriteSheetSize.Value.X != 0 || _texture.Height % spriteSheetSize.Value.Y != 0)
                {
                    throw new Exception($"Sprite sheet size not matching texture size. TextureSize: {_texture.Width}, {_texture.Height}\t Sheet: {spriteSheetSize.Value.X},{spriteSheetSize.Value.Y}");
                }
                this.spriteSheetSize = spriteSheetSize.Value;
                size = new Vector2(_texture.Width / spriteSheetSize.Value.X, _texture.Height / spriteSheetSize.Value.Y);
            }
            else
            {
                size = new Vector2(_texture.Width, _texture.Height);
            }

            switch (drawOrigin)
            {
                case DrawOrigin.TopLeft:
                    origin = Vector2.Zero;
                    break;
                case DrawOrigin.TopRight:
                    origin = new Vector2(size.X, 0);
                    break;
                case DrawOrigin.BottomLeft:
                    origin = new Vector2(0, size.Y);
                    break;
                case DrawOrigin.BottomRight:
                    origin = size;
                    break;
                case DrawOrigin.Center:
                    origin = size / 2f;
                    break;
                default:
                    throw new NotImplementedException();
            }

            this.offset = offset;
        }

        public GlobalSprite(Entity parent, string texture, Vector2 drawOrigin, float Scale = 1, Point? spriteSheetSize = null, Vector2 offset = default) : base(parent)
        {
            _texture = Globals.Inst.textures.get(texture);
            LocVelAccel = GetParentComponent<LocVelAccel>();
            this.scale = Scale;

            SpriteEffect = SpriteEffects.None;

            if (spriteSheetSize.HasValue)
            {
                if (_texture.Width % spriteSheetSize.Value.X != 0 || _texture.Height % spriteSheetSize.Value.Y != 0)
                {
                    throw new Exception($"Sprite sheet size not matching texture size. TextureSize: {_texture.Width}, {_texture.Height}\t Sheet: {spriteSheetSize.Value.X},{spriteSheetSize.Value.Y}");
                }
                this.spriteSheetSize = spriteSheetSize.Value;
                size = new Vector2(_texture.Width / spriteSheetSize.Value.X, _texture.Height / spriteSheetSize.Value.Y);
            }
            else
            {
                size = new Vector2(_texture.Width, _texture.Height);
            }

            this.origin = drawOrigin * size;
            this.offset = offset;
        }

        public float Rotation { get; set; }

        protected override void DoTick(float deltaTime)
        {

        }

        protected override void DoDraw()
        {
            Globals.Inst.spriteBatch.Draw(_texture, camera.WorldToScreen(LocVelAccel.Location + offset), GetSourceRectangle(), Color.White, MathHelper.ToRadians(Rotation), origin, scale * camera.Zoom, SpriteEffect, 1);
        }

        private Rectangle GetSourceRectangle()
        {
            int x = (int)(Frame % spriteSheetSize.X) * (int)size.X;
            int y = ((int)(Frame / spriteSheetSize.X) * (int)size.Y);
            return new Rectangle(new Point(x, y), size.ToPoint());
        }


        private float ticksSinceLastStop = 0;
        public void OnSpriteTick(float deltaTime)
        {
            if (LocVelAccel.Velocity.LengthSquared() > 5)
            {
                const int FrameTimeFrame = 30;
                ticksSinceLastStop += deltaTime;

                ticksSinceLastStop %= FrameTimeFrame;

                Frame = (uint)(ticksSinceLastStop > FrameTimeFrame / 2 ? 1 : 2);
                Frame = (uint)(ticksSinceLastStop > FrameTimeFrame / 2 ? 1 : 2);

                SpriteEffect = LocVelAccel.Velocity.X > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            }
            else
            {
                ticksSinceLastStop = 0;
                Frame = 0;
            }
        }

        
        public void OnGunSpriteTick(float deltaTime)
        {
            Rotation = MathFunc.VectorPointAngle(LocVelAccel.Location, Globals.Inst.Camera.ScreenToWorld(InputHelper.MouseState.Position.ToVector2()));

            if(Rotation < 90 || Rotation > 270)
            {
                Frame = 0;
            }
            else
            {
                Frame = 1;
            }
            if (LocVelAccel.Velocity.LengthSquared() > 5 && float.IsNegative(LocVelAccel.Velocity.X) != float.IsNegative(offset.X))
            {
                offset.X *= -1;
            }
            DebugDrawer.Debug.SetLeft(spD, 2);
        }

        public string spD()
        {
            return SpriteEffect.ToString();
        }
    }

    internal enum DrawOrigin
    {
        TopLeft = 0,
        BottomLeft = 1,
        TopRight = 2,
        BottomRight = 3,
        Center = 4,
    }
}
