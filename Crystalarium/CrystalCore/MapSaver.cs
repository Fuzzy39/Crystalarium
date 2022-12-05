using CrystalCore.Model.Elements;
using CrystalCore.Model.Objects;
using CrystalCore.Model.Rules;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace CrystalCore
{



    public class MapSaver
    {

        private Engine engine;
        internal MapSaver(Engine e)
        {
           engine = e;
        }


        public void Save(string path, Map m)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            using (XmlWriter writer = XmlWriter.Create(path, settings))
            {

                writer.WriteStartElement("Map");

                    writer.WriteStartElement("Ruleset");

                        writer.WriteValue(m.Ruleset.Name);

                    writer.WriteEndElement();

                    writer.WriteStartElement("Geometry");

                    WritePoint(writer, m.grid.Origin, "ChunkOrigin");
                    WritePoint(writer, m.grid.Size, "ChunkSize");
                
                    writer.WriteEndElement();

                    writer.WriteStartElement("Agents");
                        List<Agent> agents = m.AgentsWithin(m.Bounds);

                        foreach(Agent a in agents)
                        {
                            WriteAgent(writer, a);
                        }
                    writer.WriteEndElement();

            writer.WriteEndElement();
            }

        }

        private void WritePoint(XmlWriter writer, Point p, string name )
        {
            writer.WriteStartElement(name);
            
            writer.WriteStartElement("X");
            writer.WriteValue(p.X);
            writer.WriteEndElement();

            writer.WriteStartElement("Y");
            writer.WriteValue(p.Y);
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        private void WriteAgent(XmlWriter writer, Agent a)
        {
            writer.WriteStartElement("Agent");
            writer.WriteStartElement("Type");
            writer.WriteValue(a.Type.Name);
            writer.WriteEndElement();
            WritePoint(writer, a.Bounds.Location, "Location");
            writer.WriteStartElement("Direction");
            writer.WriteString(a.Facing.ToString());
            writer.WriteEndElement();
            writer.WriteEndElement(); 

        }


        public void Load(string path, Map m)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.CloseInput = true;
            settings.IgnoreComments = true;
            settings.IgnoreWhitespace = true;
            settings.ValidationType = ValidationType.Schema;

          

            try
            {

                using (XmlReader reader = XmlReader.Create(new FileStream(path, FileMode.Open), settings))
                {
                    reader.Read();

                    reader.ReadStartElement("Map");                    

                    if(!reader.Name.Equals("Ruleset"))
                    {
                        throw new MapLoadException("Could not Find the Ruleset of this Save file.");
                    }
                    string ruleName = reader.ReadElementContentAsString();

                    // get the ruleset.
                    m.Ruleset = GetRuleset(ruleName);

                    

                    LoadGeometry(reader, m);
                    reader.Read();
                    reader.Read();
                    LoadAgents(reader, m);


                }
            }
            catch (XmlException e)
            {
                throw new MapLoadException(e.Message);
            }

            
        }



        private Ruleset GetRuleset(string ruleName)
        {
            foreach(Ruleset rs in engine.Rulesets)
            {
                if(rs.Name.Equals(ruleName))
                {
                    return rs;
                }
            }

            throw new MapLoadException("Ruleset: '"+ruleName+"' Does not exist.");
        }

        private void LoadGeometry(XmlReader reader, Map m)
        {

            reader.ReadStartElement("Geometry");

            try
            {
                Point origin = LoadPoint(reader, "ChunkOrigin");
                Point size = LoadPoint(reader, "ChunkSize");
                origin *= new Point(Chunk.SIZE);
                size *= new Point(Chunk.SIZE);

                m.ExpandToFit(new Rectangle(origin, size));

            }
            catch(MapLoadException e)
            {
                throw new MapLoadException("Could not Find the Geometry of this Save file.\n"+e.Message);
            }
        }

        private Point LoadPoint(XmlReader reader, string name)
        {
            if (!reader.Name.Equals(name))
            {
                if (!reader.ReadToFollowing(name))
                {
                    throw new MapLoadException("Could not find point '" + name + "'.");
                }
            }
            Point toReturn =new Point(0);
            reader.Read();

            if (!reader.Name.Equals("X"))
            {
                throw new MapLoadException("Malformed point '" + name + "'.");
            }

            toReturn.X = reader.ReadElementContentAsInt();

            if (!reader.Name.Equals("Y"))
            {
                throw new MapLoadException("Malformed point '" + name + "'.");
            }

            toReturn.Y = reader.ReadElementContentAsInt();

            return toReturn;
        }
        
        private void LoadAgents(XmlReader bigReader, Map m )
        {
            
            if (!bigReader.Name.Equals("Agents"))
            {
                throw new MapLoadException("Could not find the agent listing for this map");
            }

            XmlReader reader = bigReader.ReadSubtree();
            try
            {

                while (LoadAgent(reader, m));
               
                
            }
            catch(MapLoadException e)
            {
                throw new MapLoadException("Could not load agents.\n" + e.Message);
            }
        }

        private bool LoadAgent(XmlReader reader, Map m)
        {
            if(!reader.ReadToFollowing("Agent"))
            {
                return false;
            }

            if(!reader.ReadToFollowing("Type"))
            {
                throw new MapLoadException("Could not find expected agent type.");
            }
            string typeS = reader.ReadElementContentAsString();
            AgentType type = m.Ruleset.GetAgentType(typeS);
            if (type == null)
            {
                throw new MapLoadException("No Agent of type '"+type+"' exists in ruleset '"+m.Ruleset.Name+"'.");
            }

       
            Point loc = LoadPoint(reader, "Location");

            if(!reader.ReadToFollowing("Direction"))
            {
                throw new MapLoadException("Malformed Agent. No direction");
            }
            string dir = reader.ReadElementContentAsString();
            Direction d;
            if(!Enum.TryParse<Direction>(dir, out d))
            {
                throw new MapLoadException("Malformed agent direction.");
            }

            new Agent(m, loc, type, d);


            return true;
            
        }


       


    }
}
