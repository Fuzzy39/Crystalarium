using CrystalCore.Sim;
using CrystalCore.View.AgentRender;
using CrystalCore.View.ChunkRender;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.View
{
    internal class SubviewManager
    {
        /*
         *  An integral part of a gridview, the Subview manager is responsible for updating the list of subviews and rendering them when commanded.
         *  
         * 
         * 
         */

        // fields

        private GridView _parent; // the gridview that we help manage.

        private List<Subview> _chunkRenderers; // list of chunk renderers currently in existence


        private List<Subview> _agentRenderers; // the list of agent renderers currently in existance.

        private List<AgentGhost> _ghosts; // the ghosts currently in existance. usually only one.


        // properties

        internal List<Subview> ChunkViews
        {
            get => _chunkRenderers;
        }

        internal List<Subview> AgentViews
        {
            get => _agentRenderers;
        }


        internal GridView Parent
        {
            get => _parent;
        }

        // constructors

        internal SubviewManager( GridView parent)
        {

            _parent = parent;

            // initialize our lists
            _chunkRenderers = new List<Subview>();
            _agentRenderers = new List<Subview>();

            _ghosts = new List<AgentGhost>();
        }

        // methods
        internal void AddGhost(AgentGhost gh)
        {
            if(!_parent.AllowMultipleGhosts)
            {
                _ghosts.Clear();
            }

            _ghosts.Add(gh);
            
        }


        internal void Draw( SpriteBatch sb)
        {
            // first update the chunk list and draw chunks.
            AddChunks();
            DrawChunks(sb);

            // do the same with agents.
            if (Parent.DoAgentRendering)
            {
                AddAgents();
                DrawAgents(sb);

               
            }

            DrawGhosts(sb);
        }

        // adds chunks to be rendered, if needbe.
        private void AddChunks()
        {
            foreach (List<Chunk> list in Parent.Grid.Chunks)
            {
                foreach (Chunk ch in list)
                {

                    if (!Parent.Camera.TileBounds().Intersects(ch.Bounds))
                    {
                        continue;
                    }

                    // ensure this chunk does not already exist
                    bool existing = false;
                    foreach (ChunkView r in _chunkRenderers)
                    {
                        if (r.RenderData == ch)
                        {
                            existing = true;
                        }

                    }

                    // this is really poetic.
                    // use this if statement to guide you in life.
                    if (existing)
                    {
                        continue;
                    }

                    Parent.RenderConfig.CreateRenderer(_parent, ch, _chunkRenderers);



                }
            }


        }


        // adds all visible agents.
        private void AddAgents()
        {
            // find agents that need rendered
            foreach (ChunkView chr in _chunkRenderers)
            {

                AddAgents((Chunk)chr.RenderData);
            }

        }

        // adds all visible agents in chunk ch
        private void AddAgents(Chunk ch)
        {
            

            foreach (Agent a in ch.Children)
            {
                bool hasRenderer = false;
                foreach (AgentView ar in _agentRenderers)
                {
                    if (ar.RenderData == a)
                    {
                        hasRenderer = true;
                        break;
                    }
                }

                if (hasRenderer)
                    continue;

                // does this agent need rendered?
                if (_parent.Camera.TileBounds().Intersects(a.Bounds))
                {
                    // add a new renderer.
                    a.Type.CreateRenderer(_parent, a, _agentRenderers);
                }
            }
        }


        private void DrawChunks( SpriteBatch sb)
        {
            // render them
            for (int i = 0; i < _chunkRenderers.Count;)
            {

                Subview r = _chunkRenderers[i];


                // repeat the previous index if this renderer was destroyed.
                if (r.Draw(sb))
                    i++;
            }
        }

        private void DrawAgents(SpriteBatch sb)
        {
           
            // render them. We cannot use a foreach because children can occasionally die.
            for (int i = 0; i < _agentRenderers.Count; i++)
            {
                AgentView ar = (AgentView)_agentRenderers[i];
                ar.Draw(sb);

                if (_agentRenderers.IndexOf(ar)==-1)
                {
                    i--;
                }
            }
            
        }


        internal void AddRenderer(Subview renderer)
        {

            if (renderer is ChunkView)
            {
                _chunkRenderers.Add((ChunkView)renderer);
                return;
            }

            if (renderer is AgentView)
            {
                _agentRenderers.Add((AgentView)renderer);
                return;
            }

            throw new ArgumentException("Could not add renderer of unkown type: " + renderer);
        }

        internal void RemoveRenderer(Subview renderer)
        {
            if (renderer is ChunkView)
            {
                _chunkRenderers.Remove((ChunkView)renderer);
                return;
            }

            if (renderer is AgentView)
            {
                _agentRenderers.Remove((AgentView)renderer);
                return;
            }

            throw new ArgumentException("Could not add renderer of unkown type: " + renderer);

        }


        private void DrawGhosts( SpriteBatch sb)
        {
            foreach(AgentGhost gh in _ghosts)
            {
                gh.Draw(sb);
            }
        }

    }
}
