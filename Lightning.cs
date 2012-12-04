using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectAlphaIota
{
    struct LineSegment
    {
        public Vector3 EndPoint;
        public Vector3 StartPoint;

        public LineSegment(Vector3 StartPoint, Vector3 EndPoint)
        {
            this.StartPoint = StartPoint;
            this.EndPoint = EndPoint;
        }
    }

    class Lightning
    {
        private static Random rand = new Random();
        public VertexPositionColorTexture[] Bolt1 = new VertexPositionColorTexture[2048];
        GraphicsDevice _device;
        ContentManager _contentManager;
        private Effect shader;
        private Matrix projectionMatrix;
        private float TotalTime;

        public Lightning(Vector3 startPoint, Vector3 endPoint)
        {
            GenerateLightning(startPoint, endPoint, 3f, ref Bolt1);
            TotalTime = 0;
        }
        public void Update(GameTime gameTime)
        {
            TotalTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        public void LoadContent(ContentManager contentManager, GraphicsDevice device)
        {
            this._device = device;
            this._contentManager = contentManager;
            shader = contentManager.Load<Effect>(@"Effects/Effect1");
            projectionMatrix = Matrix.CreateOrthographicOffCenter(0, device.Viewport.Width, device.Viewport.Height, 0, 0, 1);
            shader.Parameters["World"].SetValue(Matrix.Identity);
            shader.Parameters["View"].SetValue(Matrix.Identity);
            shader.Parameters["Projection"].SetValue(projectionMatrix);
            shader.Parameters["modelTexture"].SetValue(_contentManager.Load<Texture2D>(@"Textures/solid"));
        }
        public void Draw()
        {
            _device.BlendState = BlendState.AlphaBlend;
            _device.RasterizerState = RasterizerState.CullNone;
            shader.Parameters["t"].SetValue(TotalTime);
            foreach (EffectPass pass in shader.CurrentTechnique.Passes)
            {
                shader.Parameters["alpha"].SetValue(1.0f);
                pass.Apply();
                _device.DrawUserPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleStrip, Bolt1, 0,
                                                                       Bolt1.Length - 2);
            }

        }
        public void GenerateLightning(Vector3 StartPoint, Vector3 EndPoint, float width, ref VertexPositionColorTexture[] vertexBuffer)
        {

            float offsetAmount = 72;
            
            int NUM_GENERATION = 4;
            List<Queue<LineSegment>> lineSegments = new List<Queue<LineSegment>>();
            Queue<LineSegment> tempQueue= new Queue<LineSegment>();
            tempQueue.Enqueue(new LineSegment(StartPoint, EndPoint));
            lineSegments.Add(tempQueue);

            for (int i = 0; i < NUM_GENERATION; i++)
            {
                int numOfQueue = lineSegments.Count;
                for (int k = 0; k < numOfQueue; k++)
                {
                    int numOfSegments = lineSegments[k].Count;
                    for (int j = 0; j < numOfSegments; j++)
                    {
                        LineSegment temp = lineSegments[k].Dequeue();
                        Vector3 midpoint = (temp.EndPoint + temp.StartPoint) * .5f;
                        midpoint += Vector3.Cross(Vector3.Backward, (Vector3.Normalize(temp.EndPoint - temp.StartPoint)) * ((float)rand.NextDouble() * 2 * offsetAmount - offsetAmount));
                        lineSegments[k].Enqueue(new LineSegment(temp.StartPoint, midpoint));
                        lineSegments[k].Enqueue(new LineSegment(midpoint, temp.EndPoint));
                        Vector3 direction = midpoint - temp.StartPoint;
                       
                        if (rand.Next(10) < 7)
                        {
                            float angle = (float)rand.NextDouble() * MathHelper.PiOver4 - MathHelper.PiOver4 / 2;
                            Vector3 splitEnd = Vector3.Transform(direction, Matrix.CreateRotationX(angle)) * .7f + midpoint;
                            tempQueue = new Queue<LineSegment>();
                            tempQueue.Enqueue(new LineSegment(midpoint, splitEnd));
                            lineSegments.Add(tempQueue);
                        }
                    }                    
                }
                offsetAmount /= 2;
            }
            
            int counter = 0;
            vertexBuffer = new VertexPositionColorTexture[2048];
            for (int k = 0; k < lineSegments.Count; k++)
            {
                LineSegment temp = lineSegments[k].Dequeue();
                if(k != 0)
                    vertexBuffer[counter++] = new VertexPositionColorTexture(new Vector3(temp.StartPoint.X, temp.StartPoint.Y, 0), Color.Red, new Vector2(1 / 32.0f * counter, 0));
                vertexBuffer[counter++] = new VertexPositionColorTexture(new Vector3(temp.StartPoint.X, temp.StartPoint.Y, 0), Color.Red, new Vector2(1 / 32.0f * counter, 0));
                vertexBuffer[counter++] = new VertexPositionColorTexture(new Vector3(temp.StartPoint.X, temp.StartPoint.Y + width, 0), Color.Red, new Vector2(1 / 32.0f * (counter - 1), 1));
                vertexBuffer[counter++] = new VertexPositionColorTexture(new Vector3(temp.EndPoint.X, temp.EndPoint.Y, 0), Color.Red, new Vector2(1 / 32.0f * (counter - 1), 0));
                vertexBuffer[counter++] = new VertexPositionColorTexture(new Vector3(temp.EndPoint.X, temp.EndPoint.Y + width, 0), Color.Red, new Vector2(1 / 32.0f * (counter - 2), 1));
                if (lineSegments[k].Count == 0)
                    vertexBuffer[counter++] = new VertexPositionColorTexture(new Vector3(temp.EndPoint.X, temp.EndPoint.Y + width, 0), Color.Red, new Vector2(1 / 32.0f * (counter - 2), 1));
                while (lineSegments[k].Count != 0)
                {
                    temp = lineSegments[k].Dequeue();
                    vertexBuffer[counter++] = new VertexPositionColorTexture(new Vector3(temp.EndPoint.X, temp.EndPoint.Y, 0), Color.Red, new Vector2(1 / 32.0f * (counter - 1), 0));
                    vertexBuffer[counter++] = new VertexPositionColorTexture(new Vector3(temp.EndPoint.X, temp.EndPoint.Y + width, 0), Color.Red, new Vector2(1 / 32.0f * (counter - 2), 1));
                    if(lineSegments[k].Count == 0)
                        vertexBuffer[counter++] = new VertexPositionColorTexture(new Vector3(temp.EndPoint.X, temp.EndPoint.Y + width, 0), Color.Red, new Vector2(1 / 32.0f * (counter - 2), 1));
                }                
            }
        }
        
    }
}
