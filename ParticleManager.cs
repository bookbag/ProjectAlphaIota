using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ProjectAlphaIota
{
    class ParticleManager
    {
        public float TotalTime;
        public List<ParticleSystem> particleSystems;
        public Queue<ParticleSystem> freeParticleSystems;
        public const int MAX_PARTICLE_SYSTEMS = 100;
        GraphicsDevice device;
        ContentManager contentManager;
        public Dictionary<string, ParticleSystemInfo> particleSystemInfoDict;

        public ParticleManager()
        {
            particleSystems = new List<ParticleSystem>(MAX_PARTICLE_SYSTEMS);
            freeParticleSystems = new Queue<ParticleSystem>(MAX_PARTICLE_SYSTEMS);
            for (var i = 0; i < MAX_PARTICLE_SYSTEMS; i++ )
            {
                freeParticleSystems.Enqueue(new ParticleSystem());
            }
            particleSystemInfoDict = new Dictionary<string, ParticleSystemInfo>();
            AddSmokeInfo();
            AddFireSmallInfo();
            AddFireworkInfo();
            AddSparkInfo();
        }
        #region SparkInfo
        public void AddSparkInfo()
        {
            ParticleSystemInfo sparkParticle = new ParticleSystemInfo();
            sparkParticle.TextureName = "fire";
            sparkParticle.name = "spark";
            sparkParticle.maxParticles = 100;
            sparkParticle.minParticles = 50;

            sparkParticle.minLife = 0f;
            sparkParticle.maxLife = .1f;

            sparkParticle.minVelocity = new Vector2(-30, -30);
            sparkParticle.maxVelocity = new Vector2(30, 30);

            sparkParticle.minAcceleration = new Vector2(10, 10);
            sparkParticle.maxAcceleration = new Vector2(10, 10);

            // Set gravity upside down, so the flames will 'fall' upward.
            sparkParticle.Gravity = new Vector2(0, 10);

            sparkParticle.MinColor = new Color(255, 0, 0, 200);
            sparkParticle.MaxColor = new Color(255, 0, 0, 200);

            sparkParticle.MinStartScale = 1f;
            sparkParticle.MaxStartScale = 1;

            sparkParticle.MinEndScale = 2;
            sparkParticle.MaxEndScale = 2;

            sparkParticle.BlendState = BlendState.Additive;
            sparkParticle.alphaDecay = .05f;
            sparkParticle.alphaDecayLength = .1f;

            particleSystemInfoDict["spark"] = sparkParticle;
        }
        #endregion
        #region FireSmallInfo
        public void AddFireSmallInfo()
        {
            ParticleSystemInfo fireSmallParticle = new ParticleSystemInfo();
            fireSmallParticle.TextureName = "fire";
            fireSmallParticle.name = "fireSmall";
            fireSmallParticle.maxParticles = 40;
            fireSmallParticle.minParticles = 20;

            fireSmallParticle.minLife = 0f;
            fireSmallParticle.maxLife = .1f;

            fireSmallParticle.minVelocity = new Vector2(-10, -10);
            fireSmallParticle.maxVelocity = new Vector2(10, 10);

            fireSmallParticle.minAcceleration = new Vector2(0, 0);
            fireSmallParticle.maxAcceleration = new Vector2(0, 0);

            fireSmallParticle.minRotation = 0;
            fireSmallParticle.maxRotation = 2* (float) Math.PI;

            // Set gravity upside down, so the flames will 'fall' upward.
            fireSmallParticle.Gravity = new Vector2(0, 0);

            fireSmallParticle.MinColor = new Color(0, 0, 0, 255);
            fireSmallParticle.MaxColor = new Color(255, 255, 255, 255);

            fireSmallParticle.MinStartScale = .1f;
            fireSmallParticle.MaxStartScale = .1f;

            fireSmallParticle.MinEndScale = .2f;
            fireSmallParticle.MaxEndScale = .2f;

            fireSmallParticle.BlendState = BlendState.Additive;
            fireSmallParticle.alphaDecay = .05f;
            fireSmallParticle.alphaDecayLength = .1f;

            particleSystemInfoDict["fireSmall"] = fireSmallParticle;
        }
        #endregion
        #region FountainInfo
        public void AddFireworkInfo()
        {
            ParticleSystemInfo fireworkParticle = new ParticleSystemInfo();
            fireworkParticle.TextureName = "fire";
            fireworkParticle.name = "firework";
            fireworkParticle.maxParticles = 2000;
            fireworkParticle.minParticles = 1000;

            fireworkParticle.minLife = 1f;
            fireworkParticle.maxLife = 1.5f;

            fireworkParticle.minVelocity = new Vector2(-50, -50);
            fireworkParticle.maxVelocity = new Vector2(50, 50);

            fireworkParticle.minAcceleration = new Vector2(0, 0);
            fireworkParticle.maxAcceleration = new Vector2(0, 0);

            fireworkParticle.Gravity = new Vector2(0, 0);

            fireworkParticle.MinColor = new Color(0, 0, 0, 0);
            fireworkParticle.MaxColor = new Color(255, 255, 255, 255);

            fireworkParticle.MinStartScale = .02f;
            fireworkParticle.MaxStartScale = .02f;

            fireworkParticle.MinEndScale = .03f;
            fireworkParticle.MaxEndScale = .03f;

            fireworkParticle.BlendState = BlendState.AlphaBlend;
            fireworkParticle.alphaDecay = 0;
            fireworkParticle.alphaDecayLength = 0;
            fireworkParticle.minRotation = (float)Math.PI * .1f;
            fireworkParticle.maxRotation = (float)Math.PI * .1f;

            particleSystemInfoDict["firework"] = fireworkParticle;
        }
        #endregion
      
        #region SmokeInfo
        public void AddSmokeInfo()
        {
            ParticleSystemInfo smokeParticle = new ParticleSystemInfo();
            smokeParticle.TextureName = "smoke";
            smokeParticle.name = "smoke";
            smokeParticle.maxParticles = 600;
            smokeParticle.minParticles = 400;

            smokeParticle.minLife = 1;
            smokeParticle.maxLife = 3;

            smokeParticle.minVelocity = new Vector2(-10, 10);
            smokeParticle.maxVelocity = new Vector2(10, 50);

            smokeParticle.minAcceleration = new Vector2(-10, 10);
            smokeParticle.maxAcceleration = new Vector2(10, 50);

            // Set gravity upside down, so the flames will 'fall' upward.
            smokeParticle.Gravity = new Vector2(0, 15);

            smokeParticle.MinColor = new Color(100, 100, 100, 100);
            smokeParticle.MaxColor = new Color(100, 100, 100, 100);

            smokeParticle.MinStartScale = 3;
            smokeParticle.MaxStartScale = 5;

            smokeParticle.MinEndScale = 6;
            smokeParticle.MaxEndScale = 9;

            smokeParticle.BlendState = BlendState.AlphaBlend;
            smokeParticle.alphaDecay = .01f;
            smokeParticle.alphaDecayLength = 1f;

            particleSystemInfoDict["smoke"] = smokeParticle;
        }
        #endregion

        public void LoadContent(ContentManager contentManager, GraphicsDevice GraphicsDevice)
        {
            this.contentManager = contentManager;
            this.device = GraphicsDevice;
        }
        public void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TotalTime += delta;
            foreach (ParticleSystem particleSystem in particleSystems)
            {
                if(particleSystem.status != ParticleStatus.Dead)
                    particleSystem.Update(gameTime);
            }
            for (var i = 0; i < particleSystems.Count; i++)
            {
                if (particleSystems[i].status == ParticleStatus.Dead)
                {
                    freeParticleSystems.Enqueue(particleSystems[i]);
                    particleSystems.RemoveAt(i);
                    i--;
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch, Camera cam)
        {
            foreach (ParticleSystem particleSystem in particleSystems)
                particleSystem.Draw(spriteBatch, cam);
        }
        //0 lifetime = infinity
        public ParticleSystem Spawn(ParticleSystemInfo psInfo, Vector2 origin, float lifetime = 0)
        {
            ParticleSystem temp = freeParticleSystems.Dequeue();
            temp.Initialize(psInfo, origin, lifetime);
            temp.LoadContent(contentManager, device);
            particleSystems.Add(temp);
            return temp;
        }
    }
}
