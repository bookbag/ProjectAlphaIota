using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectAlphaIota
{
    public enum Alignment
    {
        LEFT, CENTER, RIGHT
    }
    public enum PULSATE_DIRECTION
    {
        UP, DOWN
    }
    class MenuItem
    {
        public MenuItem(string text, Vector2 position, Color color, Alignment alignment)
            : base()
        {
            this.text = text;
            this.position = position;
            this.color = color;
            this.alignment = alignment;
        }
        public virtual void Draw(UIMainMenu mainmenu, bool isSelected)
        {
            
            UIScreenManager manager = mainmenu.manager;
            SpriteBatch spriteBatch = manager.spriteBatch;
            spriteBatch.Begin();
            Color tempColor = isSelected ? selectedColor : color;
            Vector2 origin = new Vector2(mainmenu.manager.font.MeasureString(text).X / 2, mainmenu.manager.font.MeasureString(text).Y / 2);
            
            if (alignment == Alignment.CENTER)
                mainmenu.manager.spriteBatch.DrawString(mainmenu.manager.font, text, position, tempColor, 0, origin, scale, SpriteEffects.None, 0);
            else if (alignment == Alignment.RIGHT)
                mainmenu.manager.spriteBatch.DrawString(mainmenu.manager.font, text, position - new Vector2(mainmenu.manager.font.MeasureString(text).X / 2, 0), tempColor, 0, origin, scale, SpriteEffects.None, 0);
            else //alignment == Alignment.LEFT
                mainmenu.manager.spriteBatch.DrawString(mainmenu.manager.font, text, position + new Vector2(mainmenu.manager.font.MeasureString(text).X / 2, 0), tempColor, 0, origin, scale, SpriteEffects.None, 0);
                        
            spriteBatch.End();
        }
        public virtual void Update(UIMainMenu mainmenu, bool isSelected, GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (isSelected)
            {
                Pulsate(delta);
            }
        }
        public void Pulsate(float delta)
        {            

            if (pulsateState == PULSATE_DIRECTION.UP)
                scale += .2f * delta;
            else
                scale -= .2f * delta;
            if (scale >= 1.5)
                pulsateState = PULSATE_DIRECTION.DOWN;
            else if(scale <= 1)
                pulsateState = PULSATE_DIRECTION.UP;

        }
        public float scale = 1.0f;
        public string text;
        public Vector2 position;
        public Color color;
        public Color selectedColor = Color.Yellow;
        public Alignment alignment;
        private PULSATE_DIRECTION pulsateState = PULSATE_DIRECTION.UP;
    }
}
