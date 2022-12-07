﻿using CrystalCore.Model.Elements;
using CrystalCore.Model.Objects;
using CrystalCore.Model.Rules;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

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

            using (XmlHelper xml = new XmlHelper(path, writing: true))
            {

                XmlWriter writer = xml.Writer;

                writer.WriteStartElement("Map");

                    writer.WriteStartElement("Ruleset");

                        writer.WriteValue(m.Ruleset.Name);

                    writer.WriteEndElement();

                    writer.WriteStartElement("Geometry");

                    xml.WritePoint( m.grid.Origin);
                    xml.WritePoint(m.grid.Size);
                
                    writer.WriteEndElement();

                    writer.WriteStartElement("Agents");
                        List<Agent> agents = m.AgentsWithin(m.Bounds);

                        foreach(Agent a in agents)
                        {
                            WriteAgent(xml, a);
                        }
                    writer.WriteEndElement();

                writer.WriteEndElement();
            }

        }

        

        private void WriteAgent(XmlHelper xml, Agent a)
        {
           
            xml.Writer.WriteStartElement("Agent");

            xml.Writer.WriteStartElement("Type");
            xml.Writer.WriteValue(a.Type.Name);
            xml.Writer.WriteEndElement();

            xml.WritePoint(a.Bounds.Location);
            xml.WriteDirection(a.Facing);

            WriteTransmissions(xml, a.Ports);
            xml.Writer.WriteEndElement(); 

        }


        private void WriteTransmissions(XmlHelper xml, List<List<Port>> ports)
        {
            xml.Writer.WriteStartElement("Transmissions");
                
            for(int i = 0; i<ports.Count; i++)
            {
               
                List < Port > portList = ports[i];

                for (int j = 0; j<portList.Count; j++)
                {
                    Port port = portList[j];

                    if (port.TransmittingValue!=0)
                    {
                        WriteTransmission(xml, new PortTransmission(port.TransmittingValue, j, (CompassPoint)i));
                    }
                }

            }

            xml.Writer.WriteEndElement();

        }

        private void WriteTransmission(XmlHelper xml, PortTransmission pt)
        {
            xml.Writer.WriteStartElement("Transmission");

            xml.Writer.WriteStartElement("Value");
            xml.Writer.WriteValue(pt.value);
            xml.Writer.WriteEndElement();

            xml.Writer.WriteStartElement("Facing");
            xml.Writer.WriteValue((int)pt.portID.Facing);
            xml.Writer.WriteEndElement();

            xml.Writer.WriteStartElement("ID");
            xml.Writer.WriteValue(pt.portID.ID);
            xml.Writer.WriteEndElement();

            xml.Writer.WriteEndElement();



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

                using (XmlHelper xml = new XmlHelper(path, writing: false))
                {
                    xml.Reader.Read();

                    xml.Reader.ReadStartElement("Map");

                    xml.VerifyElementToRead("Ruleset");
                    string ruleName = xml.Reader.ReadElementContentAsString();

                    // get the ruleset.
                    m.Ruleset = GetRuleset(ruleName);



                    LoadGeometry(xml, m);
                    
                    LoadAgents(xml, m);


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

        private void LoadGeometry(XmlHelper xml, Map m)
        {

          

            try
            {

                xml.Reader.ReadStartElement("Geometry");

                Point origin = xml.ReadPoint();
                Point size = xml.ReadPoint();

                origin *= new Point(Chunk.SIZE);
                size *= new Point(Chunk.SIZE);
                m.ExpandToFit(new Rectangle(origin, size));

                xml.Reader.ReadEndElement();
            }
            catch(MapLoadException e)
            {
                throw new MapLoadException("Could not find the Geometry of this save file.\n"+e.Message);
            }
        }

     
        
        private void LoadAgents(XmlHelper xml, Map m )
        {


            xml.Reader.ReadStartElement("Agents");

            //XmlReader reader = bigReader.ReadSubtree();


            try
            {
                while (xml.Reader.NodeType == XmlNodeType.Element)
                {
                    LoadAgent(xml, m);
                }

            }
            catch(MapLoadException e)
            {
                throw new MapLoadException("Could not load an agent in this save file.\n" + e.Message);
            }
            
        }

        private void LoadAgent(XmlHelper xml, Map m)
        {
            
            xml.Reader.ReadStartElement("Agent");


            xml.VerifyElementToRead("Type");
            string typeString = xml.Reader.ReadElementContentAsString();
            AgentType type = m.Ruleset.GetAgentType(typeString);

            if (type == null)
            {
                throw new MapLoadException("Error at "+xml.FormattedReaderPosition+" of save file: No Agent of type '"+type+"' exists in ruleset '"+m.Ruleset.Name+"'.");
            }

            Point loc = xml.ReadPoint();

            Direction d = xml.ReadDirection();

            new Agent(m, loc, type, d);

            xml.Reader.ReadEndElement();
            
        }


    }
}
