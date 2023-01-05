using System;
using System.Collections.Generic;
using CrystalCore.Model;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;



namespace CrystalCore.View.Rendering
{
    /// <summary>
    /// A physics camera creates a more linear zoom on top of a camera's scaling abilities and adds velocity to panning and zooming.
    /// </summary>
    public class PhysicsCamera:Camera
    {
        
        
       
        // 'Camera' controls
       
        private Vector3 _velocity; // the velocity of the camera in x, y, and z dimensions. (in pixels/frame)
        private Vector3 friction = new Vector3(.05f, .05f, .000f); // the rate which camera velocity is reduced, as a ratio of velocity lost per frame.
        private const float MIN_FRICTION = .3f; // the minimum amount of friction that can be applied, if the camera is in motion, in pixels/frame.
        private const float MAX_SPEED = 15f; // the maximum velocity per dimension of the camera in pixels/frame.

        

        private Point _zoomOrigin; // the point, in pixels relative to the top left corner of our gridview,
                                   // that serves as the origin for dilation translations/zooming.

       
        public override Vector2 Position
        {
            get => base.Position;
            set
            {
                base.Position = value;
                VelX = 0;
                VelY = 0;
            }
             
        }


        public override Vector2 OriginPosition
        {
            get => base.OriginPosition;
            set
            {
                base.OriginPosition = value;
                VelX = 0;
                VelY = 0;
            }

        }


        // Properties

        /// <summary>
        /// scale, but linear, and scaled from 0 to 100, where 100 is maxScale, and 0 is minScale.
        /// </summary>
        public float Zoom
        {
            get 
            {
                int MaxArea = MaxScale * MaxScale;
                int MinArea = MinScale * MinScale;

                // lerp between area and zoom
                // scale where 0 is min and 1 is max. .5 is average of min and max.
                //float linearScale = (float)( ( (float)_scale*_scale ) - MinArea ) / ((float)MaxArea-MinArea);
                float cons = 100f/ ((float)MaxScale - MinScale);
                float zoom =  ((float)_scale - MinScale)*cons ;
                if (MaxScale > 50) 
                    Console.WriteLine("get: " + zoom);

                return zoom;
                //return linearScale* 100f;
            }
            set
            {
                if (value < 0) { value = 0; VelZ = 0; }
                if (value > 100) { value = 100; VelZ = 0; }
              

                int MaxArea = MaxScale * MaxScale;
                int MinArea = MinScale * MinScale;

                // lerp between area and zoom
                //_scale = MathF.Sqrt( (value/ 100f)*(MaxArea - MinArea)+MinArea);

                _scale = MathHelper.Lerp(MinScale, MaxScale, value / 100f);

                if (MaxScale > 50)
                    Console.WriteLine("Set: " + value +" Scale: "+_scale);


            }
        }   


        public Vector3 Velocity
        {
            get => _velocity;

            set
            {
                VelX = value.X;
                VelY = value.Y;
                VelZ = value.Z;
            }
        

        }

        public float VelX
        {
            get => _velocity.X;
            set 
            {
                _velocity.X = value;
                // limit velocity.
                if (_velocity.X > MAX_SPEED) _velocity.X = MAX_SPEED;
                if (_velocity.X < -MAX_SPEED) _velocity.X = -MAX_SPEED;
            }
        }

        public float VelY
        {
            get => _velocity.Y;
            set
            {
                _velocity.Y = value;

                // limit velocity.
                if (_velocity.Y > MAX_SPEED) _velocity.Y = MAX_SPEED;
                if (_velocity.Y < -MAX_SPEED) _velocity.Y = -MAX_SPEED;
            }
        }

        public float VelZ
        {
            get => _velocity.Z;
           
            set
            {
                _velocity.Z = value;
                // limit velocity
                if (_velocity.Z > MAX_SPEED) _velocity.Z = MAX_SPEED;
                if (_velocity.Z < -MAX_SPEED) _velocity.Z = -MAX_SPEED;
            }

        }

        /// <summary>
        /// The Center from which zooming will happen, in pixels relative to this camera's bounds
        /// </summary>
        public Point ZoomOrigin
        {
            get => _zoomOrigin;
            set => _zoomOrigin = value; 
        
        }


        // Constructors

        internal PhysicsCamera(Rectangle pixelBoundry) : base(pixelBoundry)
        {


            _zoomOrigin = new Point(0);
           
        }



        // Drawing code:


     
      

        // bounds represents the boundries of this 
        internal override void Update(Rectangle bounds)
        {

            base.Update(bounds);


            UpdatePosition();
          
            UpdateZoom(Zoom + Velocity.Z);

            // Apply friction.
            _velocity.X = ApplyFriction(Velocity.X, friction.X);
            _velocity.Y = ApplyFriction(Velocity.Y, friction.Y);
            _velocity.Z = ApplyFriction(Velocity.Z, friction.Z);


        }


        private void UpdatePosition()
        {
            Vector2 nextPos = OriginPosition + new Vector2(Velocity.X / (float)Scale, Velocity.Y / (float)Scale);

            //Console.WriteLine(Scale);
            // prevent camera movement outside the bounds.
            if (!SetPosition(new Vector2(nextPos.X, OriginPosition.Y)))
            {
                // add a bit of bounce to the camera.
                VelX = (float)(-VelX / 2);


            }

            if (!SetPosition(new Vector2(OriginPosition.X, nextPos.Y)))
            {
                // add a bit of bounce to the camera.
                VelY = (float)(-VelY / 2);


            }

        }


        private float ApplyFriction(float before, float frict) 
        { 
            if(MathF.Abs(frict*before) < MIN_FRICTION)
            {
                
                return MiscUtil.Reduce(before, MIN_FRICTION);
            }

            return MiscUtil.Reduce(before, before * frict);
        }


        private void UpdateZoom(float newScale)
        {
            // functionally a dilation.

            // we need to change the scale to newscale while ensuring that the center remains the same.
            // first, get the tilespace location of the center currently.

            // this must remain in the same location as the origin before and after the transformation
            Vector2 originLocation = PixelToTileCoords(_zoomOrigin);


            // if all went well, actually change the zoom.
            Zoom = newScale;

            // ensure we don't bust anything.
            if (_scale > MaxScale) { _scale = MaxScale; VelZ = 0; }
            if (_scale < MinScale) { _scale = MinScale; VelZ = 0; }

            // okay, we need to correct the camera position now.


            // get the changes we need to make
            Vector2 originError = originLocation- PixelToTileCoords(_zoomOrigin);
            Vector2 pos = OriginPosition + originError;

            // pos could be invalid. we must forcibly validate it.
            if (_isBound)
            {
                // first, get the center.
                float centerX = (float)(((TileBounds().Size.X) / 2f)) + pos.X;
                float centerY = (float)(((TileBounds().Size.Y) / 2f)) + pos.Y;
                Vector2 center = new Vector2(centerX, centerY);

                // rectify positions.
                // the .1fs are for some ridiclous floating point nonsense
                if (center.X > _bounds.Right) { pos.X -= center.X - _bounds.Right; }
                if (center.X < _bounds.Left) { pos.X -=  center.X - _bounds.Left - .1f; }
                if (center.Y > _bounds.Bottom) { pos.Y -= center.Y -_bounds.Bottom; }
                if (center.Y < _bounds.Top) { pos.Y -= center.Y - _bounds.Top -.1f; }

            }
            
            
            SetPosition(pos);
            
        }

        public void AddVelocity(float vel, Direction d)
        {
            // get the velocity we need to add.
            Vector2 toAdd = d.ToPoint().ToVector2()*vel;
            VelX += toAdd.X;
            VelY += toAdd.Y;

        }


    }   
}
