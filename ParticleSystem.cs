using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ProjectAlphaIota
{
    struct ParticleSystemInfo
    {
        public string name;

        public float minLife;
        public float maxLife;

        public int minParticles;
        public int maxParticles;

        public Vector2 minRotation;
        public Vector2 maxRotation;

        public Vector2 minVelocity;
        public Vector2 maxVelocity;

        public Vector2 minAcceleration;
        public Vector2 maxAcceleration;

        public Color MinColor;
        public Color MaxColor;

        public float lifetime;

        public Vector2 Gravity;

        public string TextureName;

        public float MinStartScale;
        public float MaxStartScale;

        public float MinEndScale;
        public float MaxEndScale;

        public float alphaDecay;
        public float alphaDecayLength;

        public BlendState BlendState;
    }
    enum ParticleStatus
    {
        Alive, Dying, Respawn, Dead
    }
    class ParticleSystem
    {
        public ParticleSystemInfo particleInfo;
        List<Particle> particles;
        Queue<Particle> freeParticles;
        public Vector2 origin;
        Effect shader;
        ContentManager contentManager;
        GraphicsDevice device;
        public ParticleStatus status;
        static Random rand = new Random();
        public const int MAX_PARTICLES = 1000;

        public ParticleSystem()
        {
            freeParticles = new Queue<Particle>(MAX_PARTICLES);
            particles = new List<Particle>(MAX_PARTICLES);            
            
            for (var i = 0; i < MAX_PARTICLES; i++)
            {
                freeParticles.Enqueue(new Particle());
            }
            status = ParticleStatus.Dead;
        }
        public void Initialize (ParticleSystemInfo particleInfo, Vector2 origin)
        {
            for (var i = particles.Count - 1; i > 0; i--)
            {
                particles[i].isAlive = false;
            }
            status = ParticleStatus.Alive;
            this.particleInfo = particleInfo;
            this.origin = origin;
            makeParticles();
        }
        public void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            device = graphicsDevice;
            this.contentManager = contentManager;
            //shader = contentManager.Load<Effect>("Content/ParticleEffect");
        }
        public void Draw(SpriteBatch spriteBatch, Camera cam)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, cam.get_transformation(device));
            for (var i = 0; i < particles.Count; i++)
            {
                spriteBatch.Draw(contentManager.Load<Texture2D>("Textures/" + particleInfo.TextureName), new Vector2((int)particles[i].Position.X, (int)particles[i].Position.Y), null, Color.White, particles[i].Rotation, new Vector2(5f, 5f), particles[i].Scale, SpriteEffects.None, 0 );
                
            }
            spriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            for (var i = 0; i < particles.Count; i++)
            {
                if (particles[i].isAlive)
                {
                    particles[i].Update(delta);
                }
            }
            for (var i = 0; i < particles.Count; i++)
            {
                if (!particles[i].isAlive)
                {
                    freeParticles.Enqueue(particles[i]);
                    particles.RemoveAt(i);
                    i--;
                }
            }
            if (status != ParticleStatus.Dying)           
                makeParticles();
            if (status == ParticleStatus.Dying && particles.Count == 0)
                status = ParticleStatus.Dead;
        }

        public void makeParticles()
        {
            var randValue = rand.Next(particleInfo.minParticles, particleInfo.maxParticles);
            if (particles.Count < randValue)
            {
                for (var i = particles.Count; i < randValue; i++)
                {
                    Particle particle = freeParticles.Dequeue();
                    particle.isAlive = true;
                    particle.alpha = 1.0f;
                    particle.TimeSinceStart = 0.0f;
                    particle.Gravity = particleInfo.Gravity;
                    particle.Color = Color.Lerp(particleInfo.MinColor, particleInfo.MaxColor, (float)rand.NextDouble());
                    particle.lifetime = (float)rand.NextDouble() * (particleInfo.maxLife - particleInfo.minLife) + particleInfo.minLife;
                    particle.Velocity = new Vector2(rand.Next((int)particleInfo.minVelocity.X, (int)particleInfo.maxVelocity.X), rand.Next((int)particleInfo.minVelocity.Y, (int)particleInfo.maxVelocity.Y));
                    particle.Acceleration = new Vector2(rand.Next((int)particleInfo.minAcceleration.X, (int)particleInfo.maxAcceleration.X), rand.Next((int)particleInfo.minAcceleration.Y, (int)particleInfo.maxAcceleration.Y));
                    particle.Position = origin;
                    particle.StartScale = (float)rand.NextDouble() * (particleInfo.MaxStartScale - particleInfo.MinStartScale) + particleInfo.MinStartScale;
                    particle.Scale = particle.StartScale;
                    particle.MaxScale = (float)rand.NextDouble() * (particleInfo.MaxEndScale - particleInfo.MinEndScale) + particleInfo.MinEndScale;
                    particle.Rotation = (float)(rand.NextDouble() * Math.PI * 2);
                    particle.alphaDecay = particleInfo.alphaDecay;
                    particle.alphaDecayStart = particle.lifetime - particleInfo.alphaDecayLength;
                    particle.DelayedStart = (float)rand.NextDouble() * (particleInfo.minLife) ;
                    particles.Add(particle);
                    
                    
                }
            }
        }
    }
   
}
