using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectAlphaIota
{
    class UIItem
    {
        public UIScreen parent;
        public UIItem()
        {

        }
        public virtual void Draw()
        {
        }
        public virtual void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
