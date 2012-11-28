using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectAlphaIota
{
    class Particle
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 Acceleration;
        public float lifetime;
        public float TimeSinceStart;
        public float MaxScale;
        public float StartScale;
        public float Scale;
        public float Rotation;
        public float RotationSpeed;
        public Vector2 Gravity;
        public Color Color;
        public float alpha = 1.0f;
        public float alphaDecay;
        public float alphaDecayStart;
        public float DelayedStart;
        public bool isAlive = false;
        public void Update(float dt)
        {
            TimeSinceStart += dt;
            if (isAlive && TimeSinceStart > DelayedStart)
            {
                Acceleration += Gravity * dt;
                Velocity += Acceleration * dt;
                Position += Velocity * dt;
                Scale += (MaxScale - StartScale) / lifetime * dt;
                
                if (alphaDecayStart > TimeSinceStart)
                {
                    //alpha -= alphaDecay;
                    //alpha -= 1/(lifetime - alphaDecayStart) *  dt;
                }
                if (TimeSinceStart >= lifetime)
                {
                    isAlive = false;
                }
            }
        }
        public Particle()
        {
         
        }
        public Particle(Vector2 position, float rotation, Vector2 velocity, Vector2 acceleration, float lifetime, float scale, float rotationSpeed)
        {
            Random rand = new Random();
            this.Position = position;
            this.Velocity = velocity;
            this.Rotation = rotation;
            this.Acceleration = acceleration;
            this.lifetime = lifetime;
            this.Scale = scale;
            this.RotationSpeed = rotationSpeed;
            this.TimeSinceStart = 0.0f;
            this.Rotation = rotation;
            this.Gravity = Vector2.Zero;
        }
    }
}
