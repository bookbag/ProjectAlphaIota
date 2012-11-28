using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;


namespace ProjectAlphaIota
{
    public class Camera
    {
        protected float _zoom; // Camera Zoom
        public Matrix _transform; // Matrix Transform
        protected float _rotation; // Camera Rotation

        public Camera()
        {
            _zoom = 1.0f;
            _rotation = 0.0f;
            Pos = Vector2.Zero;
        }
        public float Zoom
        {
            get { return _zoom; }
            set { _zoom = value; if (_zoom < 0.1f) _zoom = 0.1f; } // Negative zoom will flip image
        }

        public float Rotation { get; set; }

        // Auxiliary function to move the camera
        public void Move(Vector2 amount)
        {
            Pos += amount;
        }

        // Get set position
        public Vector2 Pos { get; set; }
        public Matrix get_transformation(GraphicsDevice graphicsDevice)
        {
            _transform =       // Thanks to o KB o for this solution
              Matrix.CreateTranslation(new Vector3(-Pos.X, -Pos.Y, 0)) *
                                         Matrix.CreateRotationZ(Rotation) *
                                         Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                                         Matrix.CreateTranslation(new Vector3(0,0, 0));
            return _transform;
        }

    }
}
