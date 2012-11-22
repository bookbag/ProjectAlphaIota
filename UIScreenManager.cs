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
    enum SCREEN_STATE
    {
        MAIN_MENU, IN_GAME_SCREEN, SPLASH_SCREEN, NUM_OF_SCREENS
    }
    class UIScreenManager
    {
        public Dictionary<SCREEN_STATE, UIScreen> screens = new Dictionary<SCREEN_STATE,UIScreen>();
        public GraphicsDevice graphicsDevice;
        public ContentManager contentManager;

        public UIScreenManager() 
        {
        }
        public void Initialize()
        {
        }
        public void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            // Load content belonging to the screen manager.
            this.contentManager = contentManager;
            this.graphicsDevice = graphicsDevice;
            spriteBatch = new SpriteBatch(graphicsDevice);
            font = contentManager.Load<SpriteFont>(@"SpriteFont1");
            //blankTexture = content.Load<Texture2D>("blank");

        }

        public SpriteBatch spriteBatch;
        public SpriteFont font;
        public bool isActive;
        public KeyboardState previousKeyboardState;
        public KeyboardState currentKeyboardState;

        public void Update(GameTime gameTime)
        {
            previousKeyboardState = Keyboard.GetState();
            
            foreach (KeyValuePair<SCREEN_STATE, UIScreen> screen in screens)
            {
                if (!screen.Value.isVisible)
                    continue;

                screen.Value.Update(gameTime);
            }
            currentKeyboardState = Keyboard.GetState();
        }
        public void Draw()
        {
            foreach (KeyValuePair<SCREEN_STATE, UIScreen> screen in screens)
            {
                if (!screen.Value.isVisible)
                    continue;

                screen.Value.Draw();
            }
        }
        public void Add(SCREEN_STATE screen_state, UIScreen screen, bool visible = false)
        {
            screen.isVisible = visible;
            screen.manager = this;
            screen.Initialize();
            screen.LoadContent();
            screens[screen_state] = screen;
        }
        public void Remove(SCREEN_STATE identifier)
        {
            screens.Remove(identifier);
        }
    }

}
