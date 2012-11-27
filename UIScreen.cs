using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace ProjectAlphaIota
{
    public enum ScreenState
    {
        Hidden, Active, TransitionOn, TransitionOff
    }
    abstract class UIScreen
    {
        public UIScreen()
        {

        }
        public abstract void Initialize();

        public virtual void LoadContent(){}

        public virtual void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TotalTime += delta;
            if (isExiting)
            {
                ScreenState = ScreenState.TransitionOff;
            }
            
            bLPressed = Mouse.GetState().LeftButton == ButtonState.Pressed && isMouseOver;
            bRPressed = Mouse.GetState().RightButton == ButtonState.Pressed && isMouseOver;
            mouseWheel = Mouse.GetState().ScrollWheelValue;

            if (ScreenState == ScreenState.TransitionOn)
            {
                TransitionPosition -= delta;
                if (TransitionPosition <= 0)
                {
                    ScreenState = ScreenState.Active;
                }
            }

            MousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            onMouseLeftClick();
            onMouseRightClick();
            onMouseScroll();
        }
        public virtual void onMouseLeftClick()
        {
            if (!bLPressed)       
               return;
        }

        public virtual void onMouseRightClick()
        {
            if (!(bRPressed))
                return;
        }
        public virtual void onMouseScroll()
        {
            
        }


        public abstract void Draw(SpriteBatch spriteBatch);

        public virtual void Enter() { }
        public virtual void Leave() { }
        public virtual void Reset() { }
        public virtual bool IsDone() { return false; }

        public float TotalTime;
        public bool isPopup = false;
        public float transitionOnTime = 0;
        public float TransitionOffTime = 0;
        public float TransitionPosition = 1;
        public bool bLPressed = false;
        public bool bRPressed = false;
        public int mouseWheel = 0;
        public bool isVisible = true;
        public bool isMouseOver = false;
        public bool isActive = false;
        public Vector2 MousePosition;
        public float TransitionAlpha
        {
            get { return 1f - TransitionPosition; }
        }
        public ScreenState ScreenState = ScreenState.TransitionOn;

        bool isExiting;
        public UIScreenManager manager;
    }    
}
