using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TextInsertion
{
    class IFD820 : Camera
    {
        public enum CGICommands
        {
            PositionSubtitle = 0,
            PositionClock = 1,
            OverlaySize = 2,
            EnableText = 3,
            EnableSubtitle = 4,
            EnableClock = 5,
            EnableDate = 6,
            TimeServ = 7,
            Subtitle = 8,
            Text = 9,
            Date = 10,
            Color = 11,
            TitleEnable = 12,
            StringPosition = 13,
            StringAlign = 14,
            String = 15,
            TimeZone = 16,
        }
        String ClockInsert;
        NetworkCredential NChttp = new NetworkCredential("Admin", "1234");
        Tuple<WebResponse, String> WR;
        
        private static String USERPASS = "http://Admin:1234@";
        private static String CGIACTION = @"/cgi-bin/admin/param.cgi?action=";
        private static String DateTime = @"DT(%Y-%m-%d %H:%M:%S)";

        public IFD820(String[] TextInsertLines, IPAddress ip)
        {
        }        
        public void Configure(Camera mDomo, IPAddress ip)
        {
            Program.l.CType = CameraType.IFD820;
            TextInsertion.Logger.MessageLog("Configuring IFD820 ip:" + ip);
            Camera.HTTPRequest(((IFD820)mDomo).BuildCommand(ip, IFD820.CGICommands.PositionSubtitle, 1, "0_4"), NChttp, "GET");
            Camera.HTTPRequest(((IFD820)mDomo).BuildCommand(ip, IFD820.CGICommands.EnableClock, 1, "yes"), NChttp, "GET");
            Camera.HTTPRequest(((IFD820)mDomo).BuildCommand(ip, IFD820.CGICommands.PositionClock, 1, "60_0"), NChttp, "GET");
            Camera.HTTPRequest(((IFD820)mDomo).BuildCommand(ip, IFD820.CGICommands.OverlaySize, 1, "4"), NChttp, "GET");
            Camera.HTTPRequest(((IFD820)mDomo).BuildCommand(ip, IFD820.CGICommands.EnableText, 1, "yes"), NChttp, "GET");
            Camera.HTTPRequest(((IFD820)mDomo).BuildCommand(ip, IFD820.CGICommands.EnableDate, 1, "yes"), NChttp, "GET");
            Camera.HTTPRequest(((IFD820)mDomo).BuildCommand(ip, IFD820.CGICommands.EnableSubtitle, 1, "yes"), NChttp, "GET");
            Camera.HTTPRequest(((IFD820)mDomo).BuildCommand(ip, IFD820.CGICommands.TitleEnable, 1, "yes"), NChttp, "GET");
            Camera.HTTPRequest(((IFD820)mDomo).BuildCommand(ip, IFD820.CGICommands.Color, 1, "yellow"), NChttp, "GET");
            Camera.HTTPRequest(((IFD820)mDomo).BuildCommand(ip, IFD820.CGICommands.TimeServ, 1, "PC"), NChttp, "GET");
            Camera.HTTPRequest(((IFD820)mDomo).BuildCommand(ip, IFD820.CGICommands.TimeZone, 1, "GMT-6"), NChttp, "GET");
            //root.Time.SynSource=PC
            //root.Time.TimeZone=GMT-6
            TextInsertion.Logger.MessageLog("Configuration DONE");
        }
        public String BuildCommand(IPAddress ip, CGICommands cmd, int line = 1, String sValue = "")
        {
            String CGI = null;
            switch (cmd)
            {
                case CGICommands.PositionSubtitle:
                    CGI = "update&Image.I0.Text.SubtitlePosition=" + sValue;//0_4
                    break;
                case CGICommands.PositionClock:
                    CGI = "update&Image.I0.Text.DatePosition=" + sValue;//60_0
                    break;
                case CGICommands.OverlaySize:
                    CGI = "overlaycolor=yellow&overlaysize=" + sValue;//4
                    break;
                case CGICommands.EnableText:
                    CGI = "update&Image.I0.Text.TextEnabled=" + sValue;//yes
                    break;
                case CGICommands.EnableDate:
                    CGI = "update&Image.I0.Text.DateEnabled=" + sValue;//yes
                    break;
                case CGICommands.EnableClock:
                    CGI = "update&Image.I0.Text.ClockEnabled=" + sValue;//yes
                    break;
                case CGICommands.EnableSubtitle:
                    CGI = "update&Image.I0.Text.SubtitleEnabled=" + sValue;//yes
                    break;
                case CGICommands.TimeServ:
                    CGI = "Time.SynSource=" + sValue;//PC
                    break;
                case CGICommands.Text:
                    CGI = "update&Image.I0.Text.Subtitle" + line + "=" + sValue;
                    break;
                case CGICommands.Date:
                    CGI = "update&Image.I0.Text.String=" + ClockInsert;
                    break;
                case CGICommands.Color:
                    CGI = "update&Image.I0.Text.Color=" + sValue;
                    break;
                case CGICommands.TitleEnable:
                    CGI = "update&Image.I0.Text.Enabled=" + sValue;//yes
                    break;
                case CGICommands.StringAlign:
                    CGI = "update&Image.I0.Text.StringAlign=" + sValue;//left
                    break;
                case CGICommands.StringPosition:
                    CGI = "update&Image.I0.Text.StringPosition=" + sValue;//undefined
                    break;
                case CGICommands.String:
                    CGI = "update&Image.I0.Text.String=" + sValue;//28/02/2017
                    break;
                case CGICommands.TimeZone:
                    CGI = "Time.TimeZone=" + sValue;//GMT-6
                    break;
            }
            CGI = USERPASS + ip + CGIACTION + CGI;
            CGI = StripSpaces(CGI);
            return CGI;
        }
        public void Send(Camera mDomo, IPAddress ip, int line, Boolean IsOpenLane, String Data = "")
        {
            Program.l.CType = CameraType.IFD820;
            
            TextInsertion.Logger.MessageLog("Line:" + line + " =>" + Data);
            if (IsOpenLane)
            {
                switch (line)
                {
                    case 1:
                        WR = Camera.HTTPRequest(((IFD820)mDomo).BuildCommand(ip, IFD820.CGICommands.String, line, ExtractClock(Data).Item2), NChttp, "GET");
                        Data = Program.TrimWhiteSpaces(ExtractClock(Data).Item1);
                        break;
                    case 2:
                        if (Data.Contains("DT("))
                        {
                            Data = Data.Remove(Data.IndexOf("DT("));
                        }                        
                        Data = Program.TrimWhiteSpaces(Data);
                        break;
                    case 3:
                        Data = Program.TrimWhiteSpaces(Data);
                        break;
                }
            }
            else
            {
                Data = Program.TrimWhiteSpaces(Data);
            }
            WR = Camera.HTTPRequest(((IFD820)mDomo).BuildCommand(ip, IFD820.CGICommands.Text, line, Data),NChttp,"GET","");
        }
        public Tuple<string,string> ExtractClock(String DataLine)
        {
            DataLine = Uri.UnescapeDataString(DataLine);
            ClockInsert = DataLine.Split(':')[2].Substring(3, 10) + "%20" + DataLine.Split(':')[2].Substring(14, 2) + ":" + DataLine.Split(':')[3] + ":" + DataLine.Split(':')[4];
            DataLine = Uri.EscapeUriString(DataLine);
            String RemainData = DataLine.Split(':')[0] + ":" + DataLine.Split(':')[1] + ":" + DataLine.Split(':')[2].Substring(0, 2);
            Tuple<string, string> TClock = new Tuple<string, string>(RemainData, ClockInsert);
            return TClock;
        }
        public String CLOCK
        {
            get { return ClockInsert; }
        }
    }
}
