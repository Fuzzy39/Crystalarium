using CrystalCore.Model.Elements;
using CrystalCore.Model.Objects;
using CrystalCore.View.Core;
using CrystalCore.View.Subviews;
using CrystalCore.View.Subviews.Agents;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.View
{
    internal class SubviewManager : IRenderable
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


        internal List<Subview> SignalViews
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
            Reset();

        }


        // methods

        internal void Reset()
        {
            // remove all curent views and ghosts: they are now outdated.

            // initialize our lists
            _chunkViews = new List<Subview>();
            _agentViews = new List<Subview>();
            _beamViews = new List<Subview>();

            _ghosts = new List<AgentGhost>();
        }


        internal void AddGhost(AgentGhost gh)
        {
            if(!_parent.AllowMultipleGhosts)
            {
                _ghosts.Clear();
            }

            _ghosts.Add(gh);
            
        }


        internal void Draw(IRenderer rend)
        {
            // first update the chunk list and draw chunks.
            AddChunks();
            DrawObjects(rend, _chunkViews);
            
            // do the same with agents.
            if (Parent.DoAgentRendering)
            {
                AddAgents();
                foreach(AgentView av in _agentViews)
                {
                    av.DrawBackground(rend);
                }

                AddSignals();
                DrawObjects(rend, _beamViews);

                DrawObjects(rend, _agentViews);


               



            }
            DrawGhosts(rend);

        }



        // adds chunks to be rendered, if needbe.
        private void AddChunks()
        {
            foreach (List<Chunk> list in Parent.Map.grid.Elements)
            {
                foreach (Chunk ch in list)
                {

                    if (!Parent.Camera.TileBounds().Intersects(ch.Bounds))
                    {
                        continue;
                    }

                    if(!_chunkViews.ViewExistsFor(ch))
                    {
                        new ChunkView(_parent, ch, _chunkViews, Parent.CurrentSkin.ChunkConfig);
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

                AgentView av = (AgentView)(_agentViews.GetViewFor(a));
                
                if(av!= null && av.Type != a.Type)
                {
                    av.Destroy();
                    av = null;
                }

                if (av == null)
                {
                    // add a new renderer.
                    new AgentView(_parent, a, _agentViews, Parent.CurrentSkin.GetAgentViewConfig(a.Type));
                }
            }
        }

        private void AddSignals()
        {
            // find agents that need rendered
            foreach (ChunkView chr in _chunkViews)
            {

                foreach( ChunkMember cm in ((Chunk)chr.RenderData).MembersWithin)
                {

                    if (!(cm is Connection))
                    {
                        continue;
                       
                    }

                    Connection beam = (Connection)cm;

         
                    if (!_beamViews.ViewExistsFor(beam))
                    {
                        
                        new SignalView (_parent, beam, _beamViews, Parent.CurrentSkin.SignalConfig);

                    }
                }
            }
        }


        private void DrawObjects(IRenderer rend, List<Subview> list)
        {
            // render them
            for (int i = 0; i < list.Count;)
            {

                Subview r = list[i];


                // repeat the previous index if this renderer was destroyed.
                if (r.Draw(rend))
                    i++;
            }
        }

        private void DrawGhosts( IRenderer rend)
        {
            foreach(AgentGhost gh in _ghosts)
            {
                gh.Draw(rend);
            }
        }

    }
}
