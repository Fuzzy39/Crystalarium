﻿using CrystalCore.Model.CoreContract;
using CrystalCore.Model.ObjectContract;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalCore.Model.DefaultObjects
{
    internal class DefaultChunk : Chunk
    {
        private bool _destroyed;
        private Point _chunkCoords;
        private Grid _grid;

       
        private List<MapObject> _objectsIntersecting;

        // Properties

        public Point ChunkCoords
        {
            get => _chunkCoords;
        }

        public List<MapObject> ObjectsIntersecting   
        {
            get { return _objectsIntersecting; }
        }

        public Grid Grid
        {
            get { return _grid; }
        }


        public bool Destroyed
        {
            get => _destroyed;
        }


        public event ComponentEvent OnDestroy;
     

        public DefaultChunk( Grid grid, Point chunkCoords)
        {
            _grid = grid;
            _chunkCoords = chunkCoords;
  
            _objectsIntersecting = new List<MapObject>();
        }



        public void Destroy()
        {

            _destroyed = true;
            OnDestroy.Invoke(this, new());

            // just to make sure stuff crashes if they use us.
            _chunkCoords = new();
            _grid = null;

            // how was this not a thing until M6?
            while (_objectsIntersecting.Count > 0)
            {
                // TODO: Make sure this actually works...
                _objectsIntersecting[0].Destroy();
            }



          
        }


        internal void OnWithinDestroyed(MapComponent comp, EventArgs e)
        {
          

            MapObject obj = (MapObject)comp;   
            _objectsIntersecting.Remove(obj);
        


        }

        public void RegisterObject(MapObject obj)
        {
            // that's uglier than I feel like it should be, but I guess it discourages default interface implementations.
            Rectangle Bounds = (((Chunk)this).Bounds);

            if (!obj.Bounds.Intersects(Bounds))
            {
                throw new ArgumentException("MapObject '" + obj.ToString() + "' is not within Chunk '" + this.ToString() + "'.");
            }

            _objectsIntersecting.Add(obj);
            obj.OnDestroy += OnWithinDestroyed;

        
        }

        public override string ToString()
        {
            if (_destroyed)
            {
                return "DefaultChunk [Destroyed]";
            }
            return "DefaultChunk [ At Chunk Coords: "+_chunkCoords+" Members: "+_objectsIntersecting.Count+" Child of: "+_grid+" ]";
        }
    }
}
