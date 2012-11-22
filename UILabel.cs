using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectAlphaIota
{
    class UILabel : UIItem
    {
        public UILabel(string text, Vector2 position, Color color, Alignment alignment)
            : base()
        {
            this.text = text;
            this.position = position;
            this.color = color;
            this.alignment = alignment;
        }
        public override void Draw()
        {
            
            UIScreenManager manager = parent.manager;
            SpriteBatch spriteBatch = manager.spriteBatch;
            spriteBatch.Begin();           
            Vector2 origin = new Vector2(parent.manager.font.MeasureString(text).X / 2, parent.manager.font.MeasureString(text).Y / 2);
            
            if (alignment == Alignment.CENTER)
                parent.manager.spriteBatch.DrawString(parent.manager.font, text, position, color, 0, origin, scale, SpriteEffects.None, 0);
            else if (alignment == Alignment.RIGHT)
                parent.manager.spriteBatch.DrawString(parent.manager.font, text, position - new Vector2(parent.manager.font.MeasureString(text).X / 2, 0), color, 0, origin, scale, SpriteEffects.None, 0);
            else //alignment == Alignment.LEFT
                parent.manager.spriteBatch.DrawString(parent.manager.font, text, position + new Vector2(parent.manager.font.MeasureString(text).X / 2, 0), color, 0, origin, scale, SpriteEffects.None, 0);
                        
            spriteBatch.End();
        }
        public override void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        public float scale = 1.0f;
        public string text;
        public Vector2 position;
        public Color color;
        public Alignment alignment;
    }
}
