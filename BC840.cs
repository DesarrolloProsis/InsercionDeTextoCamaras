using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;

namespace TextInsertion
{
    class BC840 : Camera
    {
        public enum CGICommands
        {
            XPosition = 0,
            YPosition = 1,
            FontSize = 2,
            EnableOSD = 3,
            TimeServ = 4,
            Text = 5,
            TextAndDate = 6,
            TextColor = 7,
        }
        private static String USERPASS = "http://Admin:1234@";
        private static String CGIPARAM = @"/params/set.cgi?";
        private static String DateTime = @"DT(%Y-%m-%d %H:%M:%S)";

        NetworkCredential NChttp = new NetworkCredential("Admin", "1234");
        Tuple<WebResponse, String> WR;

        public BC840()
        {                       
              
        }
        public void Configure(Camera mContext, IPAddress ip)
        {
            Program.l.CType = CameraType.BC840;
            TextInsertion.Logger.MessageLog("Configuring BC840 ip:" + ip);
            Camera.HTTPRequest(((BC840)mContext).BuildCommand(ip, BC840.CGICommands.XPosition, 1, "2"), NChttp, "GET");
            Camera.HTTPRequest(((BC840)mContext).BuildCommand(ip, BC840.CGICommands.YPosition, 1, "5"), NChttp, "GET");
            Camera.HTTPRequest(((BC840)mContext).BuildCommand(ip, BC840.CGICommands.FontSize, 1, "15"), NChttp, "GET");
            Camera.HTTPRequest(((BC840)mContext).BuildCommand(ip, BC840.CGICommands.EnableOSD, 1, "True"), NChttp, "GET");
            Camera.HTTPRequest(((BC840)mContext).BuildCommand(ip, BC840.CGICommands.TextColor, 1, "Yellow"), NChttp, "GET");
            Camera.HTTPRequest(((BC840)mContext).BuildCommand(ip, BC840.CGICommands.XPosition, 2, "2"), NChttp, "GET");
            Camera.HTTPRequest(((BC840)mContext).BuildCommand(ip, BC840.CGICommands.YPosition, 2, "10"), NChttp, "GET");
            Camera.HTTPRequest(((BC840)mContext).BuildCommand(ip, BC840.CGICommands.FontSize, 2, "15"), NChttp, "GET");
            Camera.HTTPRequest(((BC840)mContext).BuildCommand(ip, BC840.CGICommands.EnableOSD, 2, "True"), NChttp, "GET");
            Camera.HTTPRequest(((BC840)mContext).BuildCommand(ip, BC840.CGICommands.TextColor, 2, "Yellow"), NChttp, "GET");
            Camera.HTTPRequest(((BC840)mContext).BuildCommand(ip, BC840.CGICommands.XPosition, 3, "2"), NChttp, "GET");
            Camera.HTTPRequest(((BC840)mContext).BuildCommand(ip, BC840.CGICommands.YPosition, 3, "15"), NChttp, "GET");
            Camera.HTTPRequest(((BC840)mContext).BuildCommand(ip, BC840.CGICommands.FontSize, 3, "15"), NChttp, "GET");
            Camera.HTTPRequest(((BC840)mContext).BuildCommand(ip, BC840.CGICommands.EnableOSD, 3, "True"), NChttp, "GET");
            Camera.HTTPRequest(((BC840)mContext).BuildCommand(ip, BC840.CGICommands.TextColor, 3, "Yellow"), NChttp, "GET");
            Camera.HTTPRequest(((BC840)mContext).BuildCommand(ip, BC840.CGICommands.TimeServ, 1, "True"), NChttp, "GET");
            TextInsertion.Logger.MessageLog("Configuration DONE");
        }

        public String BuildCommand(IPAddress ip, CGICommands cmd,int line = 1, String sValue = "")
        {
            String CGI = null;
            switch (cmd)
            {                 
                case CGICommands.XPosition:
                    CGI = "Video-1.Osd-" + line +"XPosition=" + sValue;
                    break;
                case CGICommands.YPosition:
                    CGI = "Video-1.Osd-" + line + "YPosition=" + sValue;
                    break;
                case CGICommands.FontSize:
                    CGI = "Video-1.Osd-" + line + "FontSize=" + sValue;
                    break;
                case CGICommands.EnableOSD:
                    CGI = "Video-1.Osd-" + line + ".Enable=" + sValue;
                    break;
                case CGICommands.TimeServ:
                    CGI = "Services.TimeManagement-&.TimeServiceEnable=" + sValue;
                    break;
                case CGICommands.Text:
                    CGI = "Video-1.Osd-" + line + ".Text=" + sValue;
                    break;
                case CGICommands.TextAndDate:
                    CGI = "Video-1.Osd-" + line + ".Text=" + sValue + DateTime;
                    break;
                case CGICommands.TextColor:
                    CGI = "Video-1.Osd-" + line + ".TextColor=" + sValue;
                    break;
            }
            CGI = USERPASS + ip + CGIPARAM + CGI;
            CGI = StripSpaces(CGI);
            return CGI;
        }
        public void Send(Camera mContext, IPAddress ip, int line, String Data = "")
        {
            Program.l.CType = CameraType.BC840;
            TextInsertion.Logger.MessageLog("Line:" + line + " =>" + Data);
            String TextToInsert  = ((BC840)mContext).BuildCommand(ip, BC840.CGICommands.Text, line, Data);
            switch (line)
            {
                case 2:
                    TextToInsert = Program.TrimWhiteSpaces(TextToInsert);
                    break;
                default:
                    break;
            }
            try
            {
                Camera.HTTPRequest(TextToInsert, NChttp, "GET");
            }
            catch (Exception e)
            {
                TextInsertion.Logger.ErrorMessages(e);
            }
        }

    }
}
