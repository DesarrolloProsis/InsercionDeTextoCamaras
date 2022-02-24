using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;


namespace TextInsertion
{
    class xmlreader
    {
        StringBuilder output = new StringBuilder();        

        public string XmlParser (String xmlString, String TAG_1, String TAG_2){
                String Sout = null;
                XmlReader reader = XmlReader.Create(new StringReader(xmlString));
                {
                    XmlWriterSettings ws = new XmlWriterSettings();
                    ws.Indent = true;
                    using (XmlWriter writer = XmlWriter.Create(output, ws))
                    {
                        Boolean IsRawDataField = false;

                        // Parse the file and display each of the nodes.
                        while (reader.Read())
                        {
                            switch (reader.NodeType)
                            {
                                case XmlNodeType.Element:
                                    writer.WriteStartElement(reader.Name);
                                    if (reader.Name == TAG_1)
                                    {
                                        //String xmlString_0 = xmlString.//xmlString.Substring(xmlString.IndexOf(TAG_1), xmlString.LastIndexOf(TAG_1));
                                        //xmlreader xmlR = new xmlreader();
                                        //xmlR.XmlParser(xmlString_0, TAG_2);
                                    }
                                    break;
                                case XmlNodeType.Text:
                                    writer.WriteString(reader.Value);
                                    if (IsRawDataField)
                                    {
                                        Sout = reader.Value;
                                    }
                                    break;
                                case XmlNodeType.XmlDeclaration:
                                case XmlNodeType.ProcessingInstruction:
                                    writer.WriteProcessingInstruction(reader.Name, reader.Value);
                                    break;
                                case XmlNodeType.Comment:
                                    writer.WriteComment(reader.Value);
                                    break;
                                case XmlNodeType.EndElement:
                                    writer.WriteFullEndElement();
                                    break;
                            }
                        }

                    }
            }
            return Sout;
        }
        public string XmlParser(String xmlString, String TAG)
        {
            String Sout = null;
            XmlReader reader = XmlReader.Create(new StringReader(xmlString));
            {
                XmlWriterSettings ws = new XmlWriterSettings();
                ws.Indent = true;
                using (XmlWriter writer = XmlWriter.Create(output, ws))
                {
                    Boolean IsRawDataField = false;

                    // Parse the file and display each of the nodes.
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                writer.WriteStartElement(reader.Name);
                                    if (reader.Name != TAG)
                                    {
                                        IsRawDataField = false;
                                    }
                                    else
                                    {
                                        IsRawDataField = true;
                                    }
                                break;
                            case XmlNodeType.Text:
                                writer.WriteString(reader.Value);
                                if (IsRawDataField)
                                {
                                    Sout = reader.Value;
                                }
                                break;
                            case XmlNodeType.XmlDeclaration:
                            case XmlNodeType.ProcessingInstruction:
                                writer.WriteProcessingInstruction(reader.Name, reader.Value);
                                break;
                            case XmlNodeType.Comment:
                                writer.WriteComment(reader.Value);
                                break;
                            case XmlNodeType.EndElement:
                                writer.WriteFullEndElement();
                                break;
                        }
                    }

                }
            }
            return Sout;
        }
    }

}
