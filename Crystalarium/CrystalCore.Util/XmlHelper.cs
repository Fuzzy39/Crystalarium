using Microsoft.Xna.Framework;
using System.Xml;

namespace CrystalCore.Util
{
    /// <summary>
    /// 
    /// XmlHelper serves as a wrapper to XML reader and writer to add aditional functionality, which hopefully is helpful.
    /// when reading an element, the functions in xmlhelper expect the reader to be exactly on that element.
    /// </summary>
    public class XmlHelper : IDisposable
    {
        private bool disposedValue;

        public XmlWriter Writer { private set; get; }
        public XmlReader Reader { private set; get; }

        public Point ReaderPosition
        {
            get
            {
                Point toReturn = new Point(-1);
                if (Reader != null
                    && typeof(IXmlLineInfo).IsInstanceOfType(Reader)
                    && ((IXmlLineInfo)Reader).HasLineInfo())

                {
                    IXmlLineInfo li = (IXmlLineInfo)Reader;
                    toReturn = new Point(li.LineNumber, li.LinePosition);
                }

                return toReturn;
            }
        }

        public string FormattedReaderPosition
        {
            get
            {
                Point p = ReaderPosition;

                if (p.X == -1)
                {
                    return "(Unknown)";
                }

                return "(" + p.X + ", " + p.Y + ")";
            }
        }


        public XmlHelper(string path, bool writing)
        {


            disposedValue = false;




            if (writing)
            {

                XmlWriterSettings writeSettings = new XmlWriterSettings();
                writeSettings.Indent = true;

                Writer = XmlWriter.Create(path, writeSettings);

                return;
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("bad path given to xmlhelper");
            }

            XmlReaderSettings readSettings = new XmlReaderSettings();
            readSettings.CloseInput = true;
            readSettings.IgnoreComments = true;
            readSettings.IgnoreWhitespace = true;

            Reader = XmlReader.Create(new FileStream(path, FileMode.Open), readSettings);


        }

        // Writing methods 

        public void WritePoint(Point p)
        {
            if (Writer == null)
            {
                throw new InvalidOperationException("xmlHelper must be created in write mode to write");
            }

            Writer.WriteStartElement("Point");

            Writer.WriteStartElement("X");
            Writer.WriteValue(p.X);
            Writer.WriteEndElement();

            Writer.WriteStartElement("Y");
            Writer.WriteValue(p.Y);
            Writer.WriteEndElement();

            Writer.WriteEndElement();
        }


        public void WriteDirection(Direction d)
        {

            if (Writer == null)
            {
                throw new InvalidOperationException("xmlHelper must be created in write mode to write");
            }

            Writer.WriteStartElement("Direction");
            Writer.WriteString(d.ToString());
            Writer.WriteEndElement();
        }


        // Reading methods:

        public void VerifyElementToRead(string name)
        {
            if (Reader == null)
            {
                throw new InvalidOperationException("xmlHelper must be created in read mode to read");
            }

            if (!Reader.Name.Equals(name))
            {
                throw new MapLoadException("Expected to find element '" + name + "' at " + FormattedReaderPosition + ", but found '" + Reader.Name + "' instead.");

            }

        }


        public Point ReadPoint()
        {
            if (Reader == null)
            {
                throw new InvalidOperationException("xmlHelper must be created in read mode to read");
            }


            Reader.ReadStartElement("Point");

            Point toReturn = new Point(0);

            VerifyElementToRead("X");
            toReturn.X = Reader.ReadElementContentAsInt();



            VerifyElementToRead("Y");
            toReturn.Y = Reader.ReadElementContentAsInt();

            Reader.ReadEndElement();

            return toReturn;
        }


        public Direction ReadDirection()
        {
            VerifyElementToRead("Direction");

            string dir = Reader.ReadElementContentAsString();

            Direction d;

            if (!Enum.TryParse(dir, out d))
            {
                throw new MapLoadException("Could not parse '" + dir + "' at " + FormattedReaderPosition + " as a direction.");
            }

            return d;
        }



        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)

                    if (Writer != null) { Writer.Dispose(); }
                    if (Reader != null)
                    {
                        Reader.Dispose();
                    }
                }


                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
