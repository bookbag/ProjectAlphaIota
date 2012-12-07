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

        public LineSegment(Vector3 startPoint, Vector3 endPoint)
        {
            this.StartPoint = startPoint;
            this.EndPoint = endPoint;
        }
    }

    class Lightning
    {
        private static Random rand = new Random();
        GraphicsDevice _device;
        ContentManager _contentManager;
        private Effect _shader;
        private Matrix _projectionMatrix;
        private float _totalTime;
        public Vector3 StartPoint;
        public Vector3 EndPoint;
        public Color Color;
        public float Width;
        private List<Queue<LineSegment>> lineSegments;
        private List<Queue<LineSegment>> lineSegments2;
        private Texture2D blank;

        public Lightning(Vector3 startPoint, Vector3 endPoint)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
            Color = Color.Yellow;
            Width = 2f;
            GenerateLightning(startPoint, endPoint, Width, Color);
            lineSegments2 = lineSegments;
            _totalTime = 0;
            
        }
        public void Update(GameTime gameTime)
        {
            _totalTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if(_totalTime > .2f)
            {
                lineSegments2 = lineSegments;
                GenerateLightning();
                _totalTime = 0;
            }
        }
        public void LoadContent(ContentManager contentManager, GraphicsDevice device)
        {
            this._device = device;
            this._contentManager = contentManager;
            _shader = contentManager.Load<Effect>(@"Effects/Effect1");           
            _projectionMatrix = Matrix.CreateOrthographicOffCenter(0, device.Viewport.Width, device.Viewport.Height, 0, 0, 1);
            _shader.Parameters["World"].SetValue(Matrix.Identity);
            _shader.Parameters["View"].SetValue(Matrix.Identity);
            _shader.Parameters["Projection"].SetValue(_projectionMatrix);
            _shader.Parameters["color"].SetValue(new Vector4(1f,1f,0,1));
            _shader.Parameters["modelTexture"].SetValue(_contentManager.Load<Texture2D>(@"Textures/solid"));
        }
        void DrawLine(SpriteBatch batch, Texture2D blank,
              float width, Color color, Vector2 point1, Vector2 point2)
        {
         
            
            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, _shader);
            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            float length = Vector2.Distance(point1, point2);

            //batch.Draw(_contentManager.Load<Texture2D>(@"Textures/solid"), new Rectangle((int)Math.Ceiling(point1.X), (int)Math.Ceiling(point1.Y), (int)Math.Ceiling(length), (int)Math.Ceiling(width)), null, color,
            //           angle, Vector2.Zero, 
            //           SpriteEffects.None, 0);
            batch.Draw(_contentManager.Load<Texture2D>(@"Textures/solid"), new Vector2(point1.X,point1.Y), null, color,
                       angle, Vector2.Zero, new Vector2(.05f * length,.05f * width),
                       SpriteEffects.None, 0);
            batch.End();
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            _shader.Parameters["t"].SetValue(_totalTime);
            _shader.Parameters["alpha"].SetValue(1.0f);
            foreach (var lineSegmentQueue in lineSegments)
            {
                foreach (var lineSegment in lineSegmentQueue)
                    DrawLine(spriteBatch, blank, Width, Color, new Vector2(lineSegment.StartPoint.X, lineSegment.StartPoint.Y), new Vector2(lineSegment.EndPoint.X, lineSegment.EndPoint.Y));
            }
            _shader.Parameters["alpha"].SetValue(.5f);
            foreach (var lineSegmentQueue in lineSegments2)
            {
                foreach (var lineSegment in lineSegmentQueue)
                    DrawLine(spriteBatch, blank, Width, Color, new Vector2(lineSegment.StartPoint.X, lineSegment.StartPoint.Y), new Vector2(lineSegment.EndPoint.X, lineSegment.EndPoint.Y));
            }
            /*
            _device.BlendState = BlendState.AlphaBlend;
            _device.RasterizerState = RasterizerState.CullNone;
            _shader.Parameters["t"].SetValue(_totalTime);

            foreach (EffectPass pass in _shader.CurrentTechnique.Passes)
            {
                _shader.Parameters["alpha"].SetValue(1.0f);
                pass.Apply();
                _device.DrawUserPrimitives<VertexPositionColorTexture>(PrimitiveType.LineList, Bolt1, 0,
                                                                       Bolt1.Length / 2);

                _shader.Parameters["alpha"].SetValue(.5f);
                pass.Apply();
                _device.DrawUserPrimitives<VertexPositionColorTexture>(PrimitiveType.LineList, Bolt2, 0,
                                                                       Bolt2.Length / 2);
            }       
             */
        }
        public void GenerateLightning()
        {
            GenerateLightning(StartPoint, EndPoint, Width, Color);
        }
        public void GenerateLightning(Vector3 startPoint, Vector3 endPoint, float width, Color color)
        {
            float offsetAmount = 72;
            
            const int numGeneration = 4;
            lineSegments = new List<Queue<LineSegment>>();
            var tempQueue= new Queue<LineSegment>();
            tempQueue.Enqueue(new LineSegment(startPoint, endPoint));
            lineSegments.Add(tempQueue);

            for (int i = 0; i < numGeneration; i++)
            {
                int numOfQueue = lineSegments.Count;
                for (int k = 0; k < numOfQueue; k++)
                {
                    int numOfSegments = lineSegments[k].Count;
                    for (var j = 0; j < numOfSegments; j++)
                    {
                        var temp = lineSegments[k].Dequeue();
                        var midpoint = (temp.EndPoint + temp.StartPoint) * .5f;
                        midpoint += Vector3.Cross(Vector3.Backward, (Vector3.Normalize(temp.EndPoint - temp.StartPoint)) * ((float)rand.NextDouble() * 2 * offsetAmount - offsetAmount));
                        lineSegments[k].Enqueue(new LineSegment(temp.StartPoint, midpoint));
                        lineSegments[k].Enqueue(new LineSegment(midpoint, temp.EndPoint));
                        var direction = midpoint - temp.StartPoint;
                       
                        if (rand.Next(10) < 7)
                        {
                            var angle = (float)rand.NextDouble() * MathHelper.PiOver4 - MathHelper.PiOver4 / 2;
                            var splitEnd = Vector3.Transform(direction, Matrix.CreateRotationX(angle)) * .7f + midpoint;
                            tempQueue = new Queue<LineSegment>();
                            tempQueue.Enqueue(new LineSegment(midpoint, splitEnd));
                            lineSegments.Add(tempQueue);
                        }
                    }                    
                }
                offsetAmount /= 2;
            }
            
            //int counter = 0;
            //vertexBuffer = new VertexPositionColorTexture[2048];
            //for (int k = 0; k < lineSegments.Count; k++)
            //{
            //    while (lineSegments[k].Count != 0)
            //    {
            //        LineSegment temp = lineSegments[k].Dequeue();
            //        vertexBuffer[counter++] = new VertexPositionColorTexture(new Vector3(temp.StartPoint.X, temp.StartPoint.Y, 0), Color.Red, new Vector2(0, 0));
            //        vertexBuffer[counter++] = new VertexPositionColorTexture(new Vector3(temp.EndPoint.X, temp.EndPoint.Y, 0), Color.Red, new Vector2(1, 1));
            //    }  
            //}
        }
        
    }
}
