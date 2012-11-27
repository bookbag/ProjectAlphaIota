using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace ProjectAlphaIota
{
    class UIScreenManager
    {
        public Dictionary<SCREEN_STATE, UIScreen> Screens = new Dictionary<SCREEN_STATE,UIScreen>();
        public GraphicsDevice GraphicsDevice;
        public ContentManager ContentManager;

        public void Initialize()
        {
        }
        public void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            // Load content belonging to the screen manager.
            ContentManager = contentManager;
            GraphicsDevice = graphicsDevice;
            Font = contentManager.Load<SpriteFont>(@"SpriteFont1");
            //blankTexture = content.Load<Texture2D>("blank");

        }

        public SpriteFont Font;
        public bool IsActive;
        public KeyboardState PreviousKeyboardState;
        public KeyboardState CurrentKeyboardState;

        public void Update(GameTime gameTime)
        {
            PreviousKeyboardState = Keyboard.GetState();
            
            foreach (KeyValuePair<SCREEN_STATE, UIScreen> screen in Screens)
            {
                if (!screen.Value.isVisible)
                    continue;

                screen.Value.Update(gameTime);
            }
            CurrentKeyboardState = Keyboard.GetState();
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (KeyValuePair<SCREEN_STATE, UIScreen> screen in Screens)
            {
                if (!screen.Value.isVisible)
                    continue;

                screen.Value.Draw(spriteBatch);
            }
        }
        public void Add(SCREEN_STATE screen_state, UIScreen screen, bool visible = false)
        {
            screen.isVisible = visible;
            screen.manager = this;
            screen.Initialize();
            screen.LoadContent();
            Screens[screen_state] = screen;
        }
        public void Remove(SCREEN_STATE identifier)
        {
            Screens.Remove(identifier);
        }
    }

}
