using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProjectAlphaIota
{
    class UIGameScreen : UIScreen
    {

        public List<ActionItem> actionItems = new List<ActionItem>();
        private Texture2D blankTexture;
        private int margin = 5;
        private int padding = 2;
        private int width = 300;
        private int height = 180;
        private int col = 5;
        private int row = 3;
        private int item_width, item_height;
        private Vector2 Position;
        private int selectedItem = -1;

        public UIGameScreen()
        {
            item_width = (int) ((width - 2.0f*margin - (col + 1)*padding)/col);
            item_height = (int) ((height - 2.0f*margin - (col + 1)*padding)/row);
        }

        public override void Initialize()
        {
            Position = new Vector2(manager.GraphicsDevice.Viewport.Width - width, manager.GraphicsDevice.Viewport.Height - height);
            blankTexture = new Texture2D(manager.GraphicsDevice, 1, 1);
            blankTexture.SetData<Color>(new Color[] { Color.White });
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            MouseState currentMouseState = Mouse.GetState();
            Rectangle rect = new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 1, 1);
            selectedItem = -1;
            for (int i = 0; i < actionItems.Count; i++)
            {
                actionItems[i].IsVisible = true;
                if (rect.Intersects(actionItems[i].Destination))
                {
                    selectedItem = i;                    
                }

            }
            if (currentMouseState.LeftButton == ButtonState.Pressed)
            {
                if (selectedItem != -1)
                {
                    actionItems[selectedItem].CallFunc();
                }
            }
        }

        public void AddItem(Rectangle position, string text, string foreground, Action method)
        {
            actionItems.Add(new ActionItem(this, text, manager.ContentManager.Load<Texture2D>("Textures/button"),
                                           position,
                                           (foreground != null) ? manager.ContentManager.Load<Texture2D>(foreground) : null, method));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Draw each menu entry in turn.
            for (int i = 0; i < actionItems.Count; i++)
            {
                bool isSelected = (i == selectedItem);
                actionItems[i].Draw(spriteBatch, isSelected);
            }
        }
    }
}
