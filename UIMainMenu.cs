using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProjectAlphaIota
{
    class UIMainMenu : UIScreen
    {
        public List<MenuItem> menuitems = new List<MenuItem>();
        int selectedItem = 0;
        Texture2D blankTexture;
        public UIMainMenu()
        {
            
        }
        public override void Initialize()
        {
            blankTexture = new Texture2D(manager.graphicsDevice, 1, 1);
            blankTexture.SetData<Color>(new Color[] { Color.Black });
        }
        public void AddMenuItem(MenuItem item)
        {
            menuitems.Add(item);
        }
        public override void Draw()
        {
            SpriteBatch spriteBatch = manager.spriteBatch;
            spriteBatch.Begin();
            manager.spriteBatch.Draw(blankTexture, new Rectangle(50, 50, 900, 600), Color.Red);
            spriteBatch.End();
            // Draw each menu entry in turn.
            for (int i = 0; i < menuitems.Count; i++)
            {
                bool isSelected = (i == selectedItem);

                menuitems[i].Draw(this, isSelected);
            }
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);


            if ((this.manager.currentKeyboardState.IsKeyDown(Keys.Up) && !this.manager.previousKeyboardState.IsKeyDown(Keys.Up))
                || (this.manager.currentKeyboardState.IsKeyDown(Keys.Left) && this.manager.previousKeyboardState.IsKeyUp(Keys.Left)))
            {
                menuitems[selectedItem].scale = 1.0f;
                selectedItem--;
                if (selectedItem < 0)
                    selectedItem = menuitems.Count - 1;
            }
            if (this.manager.currentKeyboardState.IsKeyDown(Keys.Down) && this.manager.previousKeyboardState.IsKeyUp(Keys.Down) 
                || this.manager.currentKeyboardState.IsKeyDown(Keys.Right) && this.manager.previousKeyboardState.IsKeyUp(Keys.Right))
            {
                menuitems[selectedItem].scale = 1.0f;
                selectedItem++;
                if (selectedItem >=  menuitems.Count)
                    selectedItem = 0;
            }

            for (int i = 0; i < menuitems.Count; i++)
            {
                bool isSelected = (i == selectedItem);

                menuitems[i].Update(this, isSelected, gameTime);
            }
        }
    }
}
