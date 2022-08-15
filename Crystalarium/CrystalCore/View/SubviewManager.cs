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

        private List<Subview> _chunkViews; // list of chunk renderers currently in existence


        private List<Subview> _agentViews; // the list of agent renderers currently in existance.

        private List<Subview> _beamViews; // the list of beam renderers currently rendering;

        private List<AgentGhost> _ghosts; // the ghosts currently in existance. usually only one.


        // properties

        internal List<Subview> ChunkViews
        {
            get => _chunkViews;
        }

        internal List<Subview> AgentViews
        {
            get => _agentViews;
        }


        internal List<Subview> BeamViews
        {
            get => _beamViews;
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
            _chunkViews = new List<Subview>();
            _agentViews = new List<Subview>();
            _beamViews = new List<Subview>();

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
            DrawObjects(sb, _chunkViews);

            // do the same with agents.
            if (Parent.DoAgentRendering)
            {
             
                AddAgents();
                foreach(AgentView av in _agentViews)
                {
                    av.DrawBackground(sb);
                }
              
                AddBeams();
                DrawObjects(sb, _beamViews);


                DrawObjects(sb, _agentViews);


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

                    if(!_chunkViews.ViewExistsFor(ch))
                    {
                        new ChunkView(_parent, ch, _chunkViews, Parent.RenderConfig);
                    }

                 

                }
            }
        }


        // adds all visible agents.
        private void AddAgents()
        {
            // find agents that need rendered
            foreach (ChunkView chr in _chunkViews)
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

                if(!_agentViews.ViewExistsFor(a))
                {
                    // add a new renderer.
                    a.Type.CreateRenderer(_parent, a, _agentViews);
                }
            }
        }

        private void AddBeams()
        {
            // find agents that need rendered
            foreach (ChunkView chr in _chunkViews)
            {

                foreach( ChunkMember cm in ((Chunk)chr.RenderData).MembersWithin)
                {

                    if (!(cm is Beam))
                    {
                        continue;
                       
                    }

                    Beam beam = (Beam)cm;

         
                    if (!_beamViews.ViewExistsFor(beam))
                    {
                        // uhhhhhh....
                        // that's a lot of stuff...
                        // it's temporary, don't worry.
                        new BeamView (_parent, beam, _beamViews, beam.Start.Parent.Type.Ruleset.BeamRenderConfig);

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
