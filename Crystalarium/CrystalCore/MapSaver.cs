using CrystalCore.Model.Communication;
using CrystalCore.Model.Core;
using CrystalCore.Model.Physical;
using CrystalCore.Model.Rules;
using CrystalCore.Model.Simulation;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Xml;

namespace CrystalCore
{



    public class MapSaver
    {

        private const int FORMAT_VERSION = 0;
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
                writer.WriteAttributeString("FormatVersion", FORMAT_VERSION.ToString());

                writer.WriteStartElement("Ruleset");

                writer.WriteValue(m.Ruleset.Name);

                writer.WriteEndElement();

                writer.WriteStartElement("Geometry");

                xml.WritePoint(m.Grid.ChunkOrigin);
                xml.WritePoint(m.Grid.ChunkSize);

                writer.WriteEndElement();

                writer.WriteStartElement("Agents");
                List<Agent> agents = m.AgentsWithin(m.Grid.Bounds);

                foreach (Agent a in agents)
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

            xml.WritePoint(a.Node.Physical.Bounds.Location);
            xml.WriteDirection(a.Node.Facing);

            WriteTransmissions(xml, a.Node.PortList);
            xml.Writer.WriteEndElement();

        }


        private void WriteTransmissions(XmlHelper xml, List<Port> ports)
        {
            xml.Writer.WriteStartElement("Transmissions");

            for (int i = 0; i < ports.Count; i++)
            {

                Port port = ports[i];
                WriteTransmission(xml, new PortTransmission(port.Output, port.Descriptor));

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
            xml.Writer.WriteValue((int)pt.descriptor.Facing);
            xml.Writer.WriteEndElement();

            xml.Writer.WriteStartElement("ID");
            xml.Writer.WriteValue(pt.descriptor.ID);
            xml.Writer.WriteEndElement();

            xml.Writer.WriteEndElement();



        }

        public string GetRulesetName(string path)
        {
            // we assume the path is a ruleset
            try
            {
                using (XmlHelper xml = new XmlHelper(path, writing: false))
                {

                    xml.Reader.Read();

                    xml.Reader.ReadStartElement("Map");

                    xml.VerifyElementToRead("Ruleset");
                    string ruleset = xml.Reader.ReadElementContentAsString();
                    GetRuleset(ruleset);
                    return ruleset;


                }
            }
            catch (Exception e) when (e is XmlException || e is MapLoadException)
            {
                return "?";
            }
        }


        public void Load(string path, Map m)
        {

            int version = -1;
            try
            {

                using (XmlHelper xml = new XmlHelper(path, writing: false))
                {
                    xml.Reader.Read();

                    xml.Reader.ReadStartElement("Map");

                    if (xml.Reader.AttributeCount > 0)
                    {
                        int.TryParse(xml.Reader.GetAttribute(0), out version);
                    }

                    xml.VerifyElementToRead("Ruleset");
                    string ruleName = xml.Reader.ReadElementContentAsString();



                    // get the ruleset.
                    m.Ruleset = GetRuleset(ruleName); // triggers a map reset

                    LoadGeometry(xml, m); // triggers another map reset, with the correct size.

                    // resetting twice is kinda awkward

                    LoadAgents(xml, m);


                }
            }
            catch (Exception e) when (e is XmlException || e is MapLoadException)
            {
                string tothrow = e.Message;

                if (version == -1)
                {
                    tothrow = "NOTE: could not identify save file format version.\n" + tothrow;
                }
                else if (version != FORMAT_VERSION)
                {
                    tothrow = "NOTE: this file was created with format version " + version + ", and the current format version is " + FORMAT_VERSION + ".\n" + tothrow;
                }

                throw new MapLoadException(tothrow);
            }


        }



        private Ruleset GetRuleset(string ruleName)
        {
            foreach (Ruleset rs in engine.Rulesets)
            {
                if (rs.Name.Equals(ruleName))
                {
                    return rs;
                }
            }

            throw new MapLoadException("Ruleset: '" + ruleName + "' Does not exist.");
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
                m.Reset(new Rectangle(origin, size));

                xml.Reader.ReadEndElement();
            }
            catch (XmlException e)
            {
                throw new MapLoadException("Could not find the Geometry of this save file.\n" + e.Message);
            }
            catch (MapLoadException e)
            {
                throw new MapLoadException("Could not find the Geometry of this save file.\n" + e.Message);
            }
        }



        private void LoadAgents(XmlHelper xml, Map m)
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
            catch (XmlException e)
            {
                throw new MapLoadException("Could not load an agent in this save file.\n" + e.Message);
            }
            catch (MapLoadException e)
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
                throw new MapLoadException("Error at " + xml.FormattedReaderPosition + " of save file: No Agent of type '" + type + "' exists in ruleset '" + m.Ruleset.Name + "'.");
            }

            Point loc = xml.ReadPoint();

            Direction d = xml.ReadDirection();

            Agent a = m.CreateAgent(type, loc, d);

            LoadTransmissions(xml, a);

            xml.Reader.ReadEndElement();

        }

        private void LoadTransmissions(XmlHelper xml, Agent a)
        {
            xml.Reader.ReadStartElement("Transmissions");

            List<PortTransmission> transmissions = new List<PortTransmission>();

            while (xml.Reader.NodeType == XmlNodeType.Element)
            {
                transmissions.Add(LoadTransmission(xml));
            }


            foreach (PortTransmission trans in transmissions)
            {
                Port p = a.Node.GetPort(trans.descriptor);
                p.Output= trans.value;
            }



            if (xml.Reader.Name.Equals("Transmissions"))
            {

                xml.Reader.ReadEndElement();
            }


        }

        private PortTransmission LoadTransmission(XmlHelper xml)
        {
            xml.Reader.ReadStartElement("Transmission");



            xml.VerifyElementToRead("Value");
            int value = xml.Reader.ReadElementContentAsInt();


            xml.VerifyElementToRead("Facing");
            CompassPoint facing = (CompassPoint)xml.Reader.ReadElementContentAsInt();

            xml.VerifyElementToRead("ID");
            int id = xml.Reader.ReadElementContentAsInt();


            xml.Reader.ReadEndElement();

            return new PortTransmission(value, id, facing);
        }


    }
}
