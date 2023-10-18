﻿using CrystalCore.Model.ObjectContract;
using CrystalCore.Util;
using Microsoft.Xna.Framework;

namespace CrystalCore.Model.CoreContract
{
    /// <summary>
    /// The physical component of a map.
    /// </summary>
    public interface Grid
    {


        List<List<Chunk>> Chunks { get; }

        List<Chunk> ChunkList { get; }


        Point ChunkOrigin { get; }


        Point ChunkSize { get; }
    
        Rectangle Bounds { get; }

       

        event EventHandler? OnResize;

        void Destroy();
        void Expand(Direction d);

        void ExpandToFit(Rectangle rect);

        Chunk ChunkAtCoords(Point tileCoord);

        List<Chunk> ChunksIntersecting(Rectangle bounds);
    }
}