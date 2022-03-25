using System;
using System.Net;
//CAMARA NUEVA
//Common Gateway Interface
namespace TextInsertion
{
    class BC950 : Camera
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
        NetworkCredential NChttp = new NetworkCredential("Admin", "Admin1234");
        Tuple<WebResponse, String> WR;

        private static String USERPASS = "http://Admin:Admin1234@";
        private static String CGIACTION = @"/cgi-bin/admin/param.cgi?action=";

        public BC950()
        {
        }

        public void Configure(Camera mContext, IPAddress ip)
        {
            Program.l.CType = CameraType.BC950;
            TextInsertion.Logger.MessageLog("Configuring BC950 ip:" + ip);
            Camera.HTTPRequest(((BC950)mContext).BuildCommand(ip, BC950.CGICommands.PositionSubtitle, 1, "0_0"), NChttp, "GET");
            Camera.HTTPRequest(((BC950)mContext).BuildCommand(ip, BC950.CGICommands.EnableClock, 1, "yes"), NChttp, "GET");
            Camera.HTTPRequest(((BC950)mContext).BuildCommand(ip, BC950.CGICommands.PositionClock, 1, "60_100"), NChttp, "GET");
            Camera.HTTPRequest(((BC950)mContext).BuildCommand(ip, BC950.CGICommands.OverlaySize, 1, "0"), NChttp, "GET");
            Camera.HTTPRequest(((BC950)mContext).BuildCommand(ip, BC950.CGICommands.EnableText, 1, "yes"), NChttp, "GET");
            Camera.HTTPRequest(((BC950)mContext).BuildCommand(ip, BC950.CGICommands.EnableDate, 1, "yes"), NChttp, "GET");
            Camera.HTTPRequest(((BC950)mContext).BuildCommand(ip, BC950.CGICommands.EnableSubtitle, 1, "yes"), NChttp, "GET");
            //Camera.HTTPRequest(((BC950)mDomo).BuildCommand(ip, BC950.CGICommands.TitleEnable, 1, "yes"), NChttp, "GET");
            Camera.HTTPRequest(((BC950)mContext).BuildCommand(ip, BC950.CGICommands.Color, 1, "yellow"), NChttp, "GET");
            //Camera.HTTPRequest(((BC950)mDomo).BuildCommand(ip, BC950.CGICommands.TimeServ, 1, "PC"), NChttp, "GET");
            //Camera.HTTPRequest(((BC950)mDomo).BuildCommand(ip, BC950.CGICommands.TimeZone, 1, "GMT-6"), NChttp, "GET");
            TextInsertion.Logger.MessageLog("Configuration DONE");
        }

        public String BuildCommand(IPAddress ip, CGICommands cmd, int line = 1, String sValue = "")
        {
            String CGI = null;
            switch (cmd)
            {
                //1.- Posición del texto completo x_y
                case CGICommands.PositionSubtitle:
                    CGI = "update&Image.I0.Text.SubtitlePosition=" + sValue;//0_0
                    break;
                //2.- Posición de la fecha-hora
                case CGICommands.PositionClock:
                    CGI = "update&Image.I0.Text.DatePosition=" + sValue;//60_0
                    break;
                //3.- Tamaño del texto (1 a 4)
                case CGICommands.OverlaySize:
                    //CGI = "overlaycolor=yellow&overlaysize=" + sValue;//4
                    CGI = "update&Image.I0.Text.Size=" + sValue;//0
                    break;
                //4.- Incluir cadena de texto
                case CGICommands.EnableText:
                    CGI = "update&Image.I0.Text.TextEnabled=" + sValue;//yes
                    break;
                //5.- Incluir la fecha
                case CGICommands.EnableDate:
                    CGI = "update&Image.I0.Text.DateEnabled=" + sValue;//yes
                    break;
                //6.- Incluir el reloj
                case CGICommands.EnableClock:
                    CGI = "update&Image.I0.Text.ClockEnabled=" + sValue;//yes
                    break;
                //7.- Incluir los subtítulos
                case CGICommands.EnableSubtitle:
                    CGI = "update&Image.I0.Text.SubtitleEnabled=" + sValue;//yes
                    break;
                //8.- No pass
                case CGICommands.TimeServ:
                    CGI = "Time.SynSource=" + sValue;//PC
                    break;
                //9.- Modifica el valor de la línea indicada
                case CGICommands.Text:
                    CGI = "update&Image.I0.Text.Subtitle" + line + "=" + sValue;
                    break;
                //10.- Modifica la cadena de texto
                case CGICommands.Date:
                    CGI = "update&Image.I0.Text.String=" + ClockInsert;
                    break;
                //11.- Cambia el color de todos los textos
                case CGICommands.Color:
                    CGI = "update&Image.I0.Text.Color=" + sValue;
                    break;
                //12.- No pass
                case CGICommands.TitleEnable:
                    CGI = "update&Image.I0.Text.Enabled=" + sValue;//yes
                    break;
                //13.- Alineación de la cadena de texto
                case CGICommands.StringAlign:
                    CGI = "update&Image.I0.Text.StringAlign=" + sValue;//left
                    break;
                //14.- Posición de la cadena de texto
                case CGICommands.StringPosition:
                    CGI = "update&Image.I0.Text.StringPosition=" + sValue;//undefined
                    break;
                //15.- Modifica la cadena de texto
                case CGICommands.String:
                    CGI = "update&Image.I0.Text.String=" + sValue;//28/02/2017
                    break;
                //16.- No pass
                case CGICommands.TimeZone:
                    CGI = "Time.TimeZone=" + sValue;//GMT-6
                    break;
            }
            CGI = USERPASS + ip + CGIACTION + CGI;
            CGI = StripSpaces(CGI);
            return CGI;
        }
        public void Send(Camera mContext, IPAddress ip, int line, Boolean IsOpenLane, String Data = "")
        {
            Program.l.CType = CameraType.BC950;

            TextInsertion.Logger.MessageLog("Line:" + line + " =>" + Data);
            if (IsOpenLane)
            {
                switch (line)
                {
                    case 1:
                        WR = Camera.HTTPRequest(((BC950)mContext).BuildCommand(ip, BC950.CGICommands.String, line, ExtractClock(Data).Item2), NChttp, "GET");
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
            WR = Camera.HTTPRequest(((BC950)mContext).BuildCommand(ip, BC950.CGICommands.Text, line, Data), NChttp, "GET", "");
        }
        public Tuple<string, string> ExtractClock(String DataLine)
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
