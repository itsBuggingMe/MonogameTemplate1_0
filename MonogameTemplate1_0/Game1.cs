using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonogameTemplate1_0;

namespace MonogameTemplate1_0
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private Globals _globals;

        //TODO(for Jason)
        //Add debug class
        //learn apos.input

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            _graphics.GraphicsProfile = GraphicsProfile.HiDef;

            _graphics.IsFullScreen = true;
            _graphics.HardwareModeSwitch = false;

            _graphics.SynchronizeWithVerticalRetrace = true;
            IsFixedTimeStep = false;

            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            Point windowSize = new Point(_graphics.GraphicsDevice.DisplayMode.Width, _graphics.GraphicsDevice.DisplayMode.Height);

            _globals = new Globals(this, windowSize);
            Globals.Inst = _globals;

            base.Initialize();

            StateMachine.Initalise(new World());
        }

        protected override void LoadContent()
        {
            // TODO: Use Globals.Inst.Add...(); to add content
            // Eg. Globals.Inst.AddTexture("Massimo's Waifu");
            //AddSound(), AddEffect(), AddMusic(),
            //Do not use content loader for fonts

            Globals.Inst.SetFont("nulshock");
            Globals.Inst.AddTexture("Dino");
            Globals.Inst.AddTexture("Cactus");
            Globals.Inst.AddTexture("Gun");
        }

        // Use Apos.Input for input when possible https://apostolique.github.io/Apos.Input/getting-started/
        protected override void Update(GameTime gameTime)
        {
            InputHelper.TickUpdate();

            StateMachine.Instance.Tick(
                (float)(gameTime.ElapsedGameTime.TotalMilliseconds / (100 / 6d))
                );

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightGray);

            StateMachine.Instance.Draw();

            base.Draw(gameTime);
        }
    }
}