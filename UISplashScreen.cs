using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProjectAlphaIota
{
    class UISplashScreen : UIScreen
    {
        Texture2D splashScreen;
        public UISplashScreen()
        {
        }

        public override void LoadContent()
        {
            base.LoadContent();
            splashScreen = manager.ContentManager.Load<Texture2D>("Content/Textures/splashScreen");
        }

        public override void Initialize()
        {


        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(splashScreen, new Rectangle(0, 0, manager.GraphicsDevice.Viewport.Width, manager.GraphicsDevice.Viewport.Height), Color.White);
            spriteBatch.End();            
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);            
        }
    }
}
