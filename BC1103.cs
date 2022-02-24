using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;
//using System.Xml;

namespace TextInsertion
{

    class BC1103 : Camera
    {
        public enum Functionnality
        {
            TextOverlay = 0,
            DateOverlay = 1,
            TimeSync    = 2,
        }
        public enum Parameter
        {
            XPosition = 0,
            YPosition = 1,
            FontSize = 2,
            EnableOSD = 3,
            TimeServ = 4,
            LocalTime = 5,
            TimeZone = 6,
            Text = 7,
            TextAndDate = 8,
            TextColor = 9,
        }

        IPAddress mip;
        String URLhttp = @"http://";
        Tuple<WebResponse, String> WRg;
        Tuple<WebResponse, String> WR;

        NetworkCredential NChttp = new NetworkCredential("admin", "admin1234");

        const String PathTextOverlay = "/ISAPI/System/Video/inputs/channels/1/overlays/text/";        
        String XMLTextOverlay = 
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?><TextOverlay version=\"2.0\"><id>1</id><enabled>true</enabled><positionX>0</positionX><positionY>0</positionY><displayText></displayText></TextOverlay>";// xmlns=""http://www.std-cgi.com/ver20/XMLSchema""        
        const String PathDateOverlay = "/ISAPI/System/Video/inputs/channels/1/overlays/dateTimeOverlay";
        String XMLDateOverlay =
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?><OsdDatetime version=\"2.0\"><enabled>true</enabled><positionX>0</positionX><positionY>0</positionY><dateStyle>MM-DD-YYYY</dateStyle><timeStyle>24hour</timeStyle><displayWeek>false</displayWeek></OsdDatetime>";
        const String PathConfigOverlay = "/ISAPI/System/Video/inputs/channels/1/overlays";

        String XMLConfigOVerlay =
                @"<?xml version=""1.0"" encoding=""UTF-8""?>
                    <VideoOverlay xmlns=""http://www.std-cgi.com/ver20/XMLSchema"" version=""2.0"">
                    	<normalizedScreenSize>
                    		<normalizedScreenWidth></normalizedScreenWidth>
                    		<normalizedScreenHeight></normalizedScreenHeight>
                    	</normalizedScreenSize>
                    	<attribute>
                    		<transparent>false</transparent>
                    		<flashing>false</flashing>
                    	</attribute>
                    	<fontSize>32*32</fontSize>
                    	<TextOverlayList size=""8"">
                    		<DateTimeOverlay>
                    			<enabled>true</enabled>
                    			<displayType>timeAndDate</displayType>
                    			<positionX>400</positionX>
                    			<positionY>48</positionY>
                    			<dateStyle>MM-DD-YYYY</dateStyle>
                    			<timeStyle>24hour</timeStyle>
                    			<displayWeek>false</displayWeek>
                    		</DateTimeOverlay>
                    		<frontColorMode>customize</frontColorMode>
                    		<frontColor>ffff00</frontColor>
                    		<edgeColor>000000</edgeColor>
                    		<alignment>customize</alignment>
                    		<TextOverlay>
                    			<id>1</id>
                    			<enabled>true</enabled>
                    			<positionX>0</positionX>
                    			<positionY>470</positionY>
                    			<displayText></displayText>
                    		</TextOverlay>
                    		<TextOverlay>
                    			<id>2</id>
                    			<enabled>true</enabled>
                    			<positionX>0</positionX>
                    			<positionY>440</positionY>
                    			<displayText></displayText>
                    		</TextOverlay>
                    		<TextOverlay>
                    			<id>3</id>
                    			<enabled>true</enabled>
                    			<positionX>0</positionX>
                    			<positionY>410</positionY>
                    			<displayText></displayText>
                    		</TextOverlay>
                    		</TextOverlayList>
                    </VideoOverlay>";

        String XMLTimeSync = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                            <Time version = ""2.0"" xmlns=""http://www.std-cgi.com/ver20/XMLSchema"">
                                <timeMode>manual</timeMode>
                                <localTime></localTime>
                                <timeZone>CST+6:00:00DST01:00:00,M4.1.0/02:00:00,M10.5.0/02:00:00</timeZone>
                            </Time>";

        public BC1103(IPAddress ip)
        {
            mip = ip;
            URLhttp += ip + PathTextOverlay;            
            WRg = HTTPRequest(URLhttp, NChttp, "GET"); 
        }
        public void Configure(IPAddress ip)
        {
            Program.l.CType = CameraType.BC1103;
            String URLhttp = @"http://" + ip + PathTextOverlay;
            String URLhttpDT = @"http://" + ip + PathDateOverlay;
            String URLhttpConfig = @"http://" + ip + PathConfigOverlay;
            Tuple<WebResponse, String> WR;
            TextInsertion.Logger.MessageLog("Configuring BC1103 ip:" + ip);       
            TextInsertion.Logger.MessageLog(URLhttpConfig);
            TextInsertion.Logger.MessageLog(XMLConfigOVerlay);
            WR = Camera.HTTPRequest(URLhttpConfig, NChttp, "PUT", XMLConfigOVerlay);
            WR = Camera.HTTPRequest(URLhttpConfig, NChttp, "PUT", XMLConfigOVerlay);
            TextInsertion.Logger.MessageLog("Configuration DONE");
        }
        public void Send(IPAddress ip, int line = 1, String Data = "")
        {
            Program.l.CType = CameraType.BC1103;
            String URLhttp = @"http://" + ip + "/ISAPI/System/Video/inputs/channels/1/overlays/text/"+ line;
            Tuple<WebResponse, String> WR;

            Data = StripSpaces(Data);
            switch (line)
            {
                case 1:
                    break;
                case 2:
                    if (Data.Contains("DT("))
                    {
                        Data = Data.Remove(Data.IndexOf("DT("));
                    }                    
                    break;
                case 3:
                    break;
            }
            TextInsertion.Logger.MessageLog("URL:" + URLhttp + "\n" + "Line:" + line + " =>" + Data);
            String cmd = BuildCommand(XMLTextOverlay, Functionnality.TextOverlay, Parameter.Text, line,  Data);
            try
            {
                WR = Camera.HTTPRequest(URLhttp, NChttp, "PUT", cmd);
                TextInsertion.Logger.MessageLog(cmd);
            }
            catch (Exception e)
            {
                TextInsertion.Logger.ErrorMessages(e);
            }           
        }
        public void SendTimeSync(IPAddress ip)
        {
            Program.l.CType = CameraType.BC1103;
            String URLhttp = @"http://" + ip + "/ISAPI/System/Time";
            Tuple<WebResponse, String> WR;
            String sSecs = (int.Parse(DateTime.Now.ToString("ss")) + 0).ToString();
            String sTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:" + sSecs) + "-06:00";
            TextInsertion.Logger.SyncTimeLog("URL:" + URLhttp + "\n" + "=>" + sTime);
            String cmd = BuildCommand(XMLTimeSync,Functionnality.TimeSync, Parameter.LocalTime,0,sTime);            
            try
            {
                WR = Camera.HTTPRequest(URLhttp, NChttp, "PUT", cmd);
                TextInsertion.Logger.SyncTimeLog(cmd);
            }
            catch (Exception e)
            {
                TextInsertion.Logger.ErrorMessages(e);
            }
        }
        public String BuildCommand(String XML, Functionnality f, Parameter cmd, int line = 0, String sValue = "")
        {
            String XMLout = null;
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(XML);
            XmlNamespaceManager nsMgr = new XmlNamespaceManager(xdoc.NameTable);
            nsMgr.AddNamespace("CGI","http://www.std-cgi.com/ver20/XMLSchema");
            XmlNode node;

            switch (f)
            {
                case Functionnality.TextOverlay:
                    switch (cmd)
                    {
                        case Parameter.XPosition:
                            node = xdoc.SelectSingleNode("//CGI:" + "positionX", nsMgr);
                            node.InnerText = sValue;
                            break;
                        case Parameter.YPosition:
                            node = xdoc.SelectSingleNode("//CGI:" + "positionY", nsMgr);
                            node.InnerText = sValue;
                            break;
                        case Parameter.EnableOSD:
                            node = xdoc.SelectSingleNode("//CGI:" + "enabled", nsMgr);
                            node.InnerText = sValue;
                            break;
                        case Parameter.Text:
                            node = xdoc.SelectSingleNode("//displayText");
                            node.InnerText = sValue;
                            node = xdoc.SelectSingleNode("//positionY");
                            switch (line)
                            {
                                case 1:
                                    node.InnerText = "470";//470
                                    node = xdoc.SelectSingleNode("//id");
                                    node.InnerText = "1";
                                    break;
                                case 2:
                                    node.InnerText = "440";//440
                                    node = xdoc.SelectSingleNode("//id");
                                    node.InnerText = "2";
                                    break;
                                case 3:
                                    node.InnerText = "410";//410
                                    node = xdoc.SelectSingleNode("//id");
                                    node.InnerText = "3";
                                    break;
                            }
                            break;
                    }
                    break;
                case Functionnality.DateOverlay:
                    switch (cmd)
                    {
                        case Parameter.XPosition:
                            node = xdoc.SelectSingleNode("//positionX");
                            node.InnerText = sValue;
                            break;
                        case Parameter.YPosition:
                            node = xdoc.SelectSingleNode("//positionY");
                            node.InnerText = sValue;
                            break;
                    }
                break;
                case Functionnality.TimeSync:
                    switch (cmd)
                    {
                        case Parameter.LocalTime:
                            node = xdoc.SelectSingleNode("//CGI:" + "localTime", nsMgr);
                            node.InnerText = sValue;
                            break;
                        case Parameter.TimeZone:
                            node = xdoc.SelectSingleNode("//CGI:" + "timeZone", nsMgr);
                            node.InnerText = sValue;
                            break;
                    }
                    break;
            }
            using (StringWriter SW = new StringWriter())
            using (XmlWriter XW = XmlWriter.Create(SW))
            {                
                xdoc.WriteContentTo(XW);
                
                XW.Flush();
                XMLout = SW.GetStringBuilder().ToString();
            }
            return XMLout;
        }
        public WebResponse WRG
        {
            get { return WRg.Item1; }
        }

    }
}
