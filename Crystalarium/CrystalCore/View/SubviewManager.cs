using CrystalCore.Model.Communication;
using CrystalCore.Model.Objects;
using CrystalCore.View.Configs;
using CrystalCore.View.Subviews;
using CrystalCore.View.Subviews.Agents;
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
         */

        // fields

        private GridView _parent; // the gridview that we help manage.

        private List<Subview> _chunkRenderers; // list of chunk renderers currently in existence


        private List<Subview> _agentRenderers; // the list of agent renderers currently in existance.

        private List<Subview> _beamRenderers; // the list of beam renderers currently rendering;

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


        internal List<Subview> BeamViews
        {
            get => _beamRenderers;
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
            _beamRenderers = new List<Subview>();

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
            DrawObjects(sb, _chunkRenderers);

            // do the same with agents.
            if (Parent.DoAgentRendering)
            {
             
                AddAgents();
                foreach(AgentView av in _agentRenderers)
                {
                    av.DrawBackground(sb);
                }
              
                AddBeams();
                DrawObjects(sb, _beamRenderers);


                DrawObjects(sb, _agentRenderers);


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

                    if(!_chunkRenderers.ViewExistsFor(ch))
                    {
                        Parent.RenderConfig.CreateRenderer(_parent, ch, _chunkRenderers);
                    }

                 

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
            

            foreach (ChunkMember cm in ch.Children)
            {
                if(!(cm is Agent))
                {
                    continue;
                }

                Agent a = (Agent)cm;

                // does this agent need rendered?
                if (!_parent.Camera.TileBounds().Intersects(a.Bounds))
                {
                    continue;
                }

                if(!_agentRenderers.ViewExistsFor(a))
                {
                    // add a new renderer.
                    a.Type.CreateRenderer(_parent, a, _agentRenderers);
                }
            }
        }

        private void AddBeams()
        {
            // find agents that need rendered
            foreach (ChunkView chr in _chunkRenderers)
            {

                foreach( ChunkMember cm in ((Chunk)chr.RenderData).MembersWithin)
                {

                    if (!(cm is Beam))
                    {
                        continue;
                       
                    }

                    Beam beam = (Beam)cm;

         
                    if (!_beamRenderers.ViewExistsFor(beam))
                    {
                        // uhhhhhh....
                        // that's a lot of stuff...
                        beam.Start.Parent.Type.Ruleset.BeamRenderConfig.CreateRenderer(_parent, beam, _beamRenderers);
                    }  
                }
            }
        }


        private void DrawObjects(SpriteBatch sb, List<Subview> list)
        {
            // render them
            for (int i = 0; i < list.Count;)
            {

                Subview r = list[i];


                // repeat the previous index if this renderer was destroyed.
                if (r.Draw(sb))
                    i++;
            }
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
