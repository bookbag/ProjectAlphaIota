using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectAlphaIota
{
    class ActionItem
    {
        Texture2D background, foreground;
        public Rectangle Destination;

        public bool IsVisible = false;
        public int Value;
        public string Text;
        private readonly UIGameScreen gameScreen;
        public Action CallFunc;
        public ActionItem(UIGameScreen gameScreen, string text, Texture2D background, Rectangle destination, Texture2D foreground, Action method)
        {
            CallFunc = method;
            this.Text = text;
            this.background = background;
            this.foreground = foreground;
            this.Destination = destination;
            this.gameScreen = gameScreen;
        }
        public void Draw(SpriteBatch spriteBatch, bool isSelected)
        {
            if (!IsVisible)
                return;
            spriteBatch.Begin();
            Rectangle source;
            if (isSelected)
                source = new Rectangle(background.Width / 2, 0, background.Width / 2, background.Height);
            else
                source = new Rectangle(0, 0, background.Width / 2, background.Height);
            spriteBatch.Draw(background, Destination, source, Color.White);
            if (foreground != null) spriteBatch.Draw(foreground, Destination, Color.White);
            Vector2 origin = new Vector2(gameScreen.manager.Font.MeasureString(Text).X / 2, gameScreen.manager.Font.MeasureString(Text).Y / 2);
            spriteBatch.DrawString(gameScreen.manager.Font, Text, new Vector2(Destination.X + Destination.Width/2, Destination.Y + Destination.Height/2), Color.White , 0.0f, origin, 1.0f, SpriteEffects.None, 0);
            spriteBatch.End();
        }
    }

}
