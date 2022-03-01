using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using System.Xml;

namespace TextInsertion
{

    public enum CameraType
    {
        Not_Connected = -1,
        Default = 0,
        BC620 = 1,
        BC840 = 2,
        IFD820 = 3,
        BC1103 = 4,
        FD1103 = 5,
    }
    public abstract class Camera
    {
        IPAddress mIP;
        String[] mTextInsertlines;
        Camera mCAM;
        static NetworkCredential NCcgi = new NetworkCredential("Admin", "1234");
        static NetworkCredential NChttp = new NetworkCredential("admin", "admin1234");
        //No se usa
        //static CookieContainer c = new CookieContainer();
        public Timer mtSyncTimer;
        //Task tSyncTimerTask;
        public Camera()
        {
            mtSyncTimer = new Timer(60000);
            mtSyncTimer.AutoReset = false;
            mtSyncTimer.Elapsed += new System.Timers.ElapsedEventHandler(TimeSyncLane);
        }
        //No se usa
        //public virtual String BuildCommand(IPAddress ip)
        //{
        //    String CGI = null;
        //    //String USERPASS = "http://Admin:1234@";
        //    CGI = ip + @"/params/get.cgi?General.ProductName";
        //    return CGI;
        //}
        //public virtual void Configure()
        //{

        //}
        //Devuelve el tipo de cámara
        public static CameraType DetectCameraType(IPAddress ip)
        {        
            Camera mCAM = new Default();
            //API Common Gateway Interface ("Interfaz de Entrada Común")
            String URLcgiContext = @"http://" + ip + "/params/get.cgi?General.ProductName";
            String URLcgiDomo = @"http://" + ip + "/cgi-bin/admin/param.cgi?action=list&group=Brand.ProdFullName";
            //API html
            String URLhttp = @"http://" + ip + "/ISAPI/System/deviceInfo";//Video/inputs/channels/1/overlays/text/2";
            String model = null;
            Tuple<WebResponse, String> WR;            
            
            try
            {//Si alguna de las peticiones HTTP con CGI genera un error en tiempo de ejecución, utiliza la petición html
                if (ip != null)
                {
                    WR = Camera.HTTPRequest(URLcgiContext, NCcgi, WebRequestMethods.Http.Get);//Método local
                    //Si la primera petición CGI devuelve una respuesta vacía, se utiliza la siguiente petición (URLcgiDomo)
                    if (WR.Item1 == null)
                    {
                        WR = Camera.HTTPRequest(URLcgiDomo, NCcgi, WebRequestMethods.Http.Get); 
                    }
                    if (((HttpWebResponse)WR.Item1).StatusDescription == "OK")//Continúa si hay una respuesta afirmativa de la petición http
                    {
                        //Si dentro de la respuesta del servidor se encuentra alguna de esas palabras, continúa
                        if (WR.Item2.Contains("ProductName") || WR.Item2.Contains("ProdFullName"))
                        {
                            //deviceName = WR.Item2.Substring(WR.Item2.IndexOf('=')+1, WR.Item2.IndexOf('-')+1 - WR.Item2.IndexOf('='));
                            //Se busca dentro del texto de la respuesta del servidor un fracmento que coincida con uno de los tipos de las cámaras
                            //que se tienen. Se asigna la constante correspondiente del enum CameraType, genera una entrada en el log y regresa esa constante
                            if (WR.Item2.Contains("BC620"))
                            {
                                Program.l.CType = CameraType.BC620;
                                TextInsertion.Logger.MessageLog("Camera with IP:" + ip + " is a: BC620 Camera");
                                return CameraType.BC620;
                            }
                            else if (WR.Item2.Contains("BC840"))
                            {
                                Program.l.CType = CameraType.BC840;
                                TextInsertion.Logger.MessageLog("Camera with IP:" + ip + " is a: BC840 Camera");
                                return CameraType.BC840;
                            }
                            else if (WR.Item2.Contains("IFD820"))
                            {
                                Program.l.CType = CameraType.IFD820;
                                TextInsertion.Logger.MessageLog("Camera with IP:" + ip + " is a: IFD820 Camera");
                                return CameraType.IFD820;
                            }
                            else
                            {
                                Program.l.CType = CameraType.Default;
                                TextInsertion.Logger.MessageLog("Camera with IP:" + ip + " is not a Siqura Camera");
                                return CameraType.Default;
                            }
                        }
                    }
                    else
                    {
                        Program.l.CType = CameraType.Default;
                        TextInsertion.Logger.MessageLog("Camera with IP:" + ip + " is not a Siqura Camera");
                        return CameraType.Default;
                    }
                }
                else
                {
                    Program.l.CType = CameraType.Not_Connected;
                    TextInsertion.Logger.MessageLog("Camera with IP:" + ip + " is not connected");
                    return CameraType.Not_Connected;
                }
            }
            catch (Exception)
            {
                Program.l.CType = CameraType.Default;
                //Cuando se genera una error devido a que la cámara no cuenta con interfaz CGI (utlizada en el paso anterior), genera una entrada en el log
                TextInsertion.Logger.MessageLog("Camera with IP:" + ip + " has no CGI interface");
                //Petición http para obtener información de la cámara
                WR = Camera.HTTPRequest(URLhttp, NChttp, WebRequestMethods.Http.Get);
                //Continúa si la respuesta de la petición no está vacía
                if (WR.Item1 != null)
                {   //Si el estatus de la respuesta es afirmativo =>
                    if (((HttpWebResponse)WR.Item1).StatusDescription == "OK")
                    {
                        xmlreader xmlR = new xmlreader();
                        model = xmlR.XmlParser(WR.Item2, "model");
                        //Dentro de la respuesta del servidor se busca alguna coincidencia con los modelos de cámara que se tienen para html.
                        //Se asigna la constante correspondiente del enum CameraType, genera una entrada en el log y regresa esa constante
                        if (model.Contains("BC1103"))
                        {
                            Program.l.CType = CameraType.BC1103;
                            TextInsertion.Logger.MessageLog("Camera with IP:" + ip + " is a: " + model + " Camera");
                            //mtSyncTimer.Start();
                            return CameraType.BC1103;
                        }
                        else if (model.Contains("FD1103"))
                        {
                            Program.l.CType = CameraType.FD1103;
                            TextInsertion.Logger.MessageLog("Camera with IP:" + ip + " is a: " + model + " Camera");
                            //mtSyncTimer.Start();
                            return CameraType.FD1103;
                        }
                        else
                        {
                            Program.l.CType = CameraType.Default;
                            TextInsertion.Logger.MessageLog("Camera with IP:" + ip + " is not a Siqura Camera");
                            return CameraType.Default;
                        }
                    }
                    else
                    {
                        Program.l.CType = CameraType.Not_Connected;
                        TextInsertion.Logger.MessageLog("Camera with IP:" + ip + " is not connected");
                        return CameraType.Not_Connected;
                    }
                }
                else
                {
                    Program.l.CType = CameraType.Not_Connected;
                    TextInsertion.Logger.MessageLog("Camera with IP:" + ip + " is not connected");
                    return CameraType.Not_Connected;
                }
            }            
            return CameraType.Default;
        }

        private void TimeSyncLane(object sender, ElapsedEventArgs e)
        {
            mtSyncTimer.Stop();
            switch (Program.CTypeContext)
            {
                case CameraType.BC620:
                    Program.L.CType = CameraType.BC620;
                    //((BC620)Program.mContext).Send(Program.mContext, Program.IPM.IPCONTEXT, 1, );
                    break;
                case CameraType.BC840:
                    Program.L.CType = CameraType.BC840;
                    //((BC620)Program.mContext).Send(Program.mContext, Program.IPM.IPCONTEXT, 1, );
                    break;
                case CameraType.BC1103:
                    Program.L.CType = CameraType.BC1103;
                    ((BC1103)Program.mContext).SendTimeSync(Program.IPM.IPCONTEXT);
                    break;
            }
            switch (Program.CTypeContext_2)
            {
                case CameraType.BC620:
                    Program.L.CType = CameraType.BC620;
                    //((BC620)Program.mContext).Send(Program.mContext, Program.IPM.IPCONTEXT, 1, );
                    break;
                case CameraType.BC840:
                    Program.L.CType = CameraType.BC840;
                    //((BC620)Program.mContext).Send(Program.mContext, Program.IPM.IPCONTEXT, 1, );
                    break;
                case CameraType.BC1103:
                    Program.L.CType = CameraType.BC1103;
                    ((BC1103)Program.mContext_2).SendTimeSync(Program.IPM.IPCONTEXT_2);
                    break;
            }
            switch (Program.CTypeDomo)
            {
                case CameraType.IFD820:
                    Program.L.CType = CameraType.IFD820;
                    //((BC620)Program.mContext).Send(Program.mContext, Program.IPM.IPCONTEXT, 1, );
                    break;
                case CameraType.FD1103:
                    Program.L.CType = CameraType.FD1103;
                    ((FD1103)Program.mDomo).SendTimeSync(Program.IPM.IPDOME);
                    break;
            }
            switch (Program.CTypeDomo_2)
            {
                case CameraType.IFD820:
                    Program.L.CType = CameraType.IFD820;
                    //((BC620)Program.mContext).Send(Program.mContext, Program.IPM.IPCONTEXT, 1, );
                    break;
                case CameraType.FD1103:
                    Program.L.CType = CameraType.FD1103;
                    ((FD1103)Program.mDomo_2).SendTimeSync(Program.IPM.IPDOME_2);
                    break;
            }
            //mtSyncTimerTask = Task.Factory.StartNew(() => TimeSyncLane(interval, ct));
            mtSyncTimer.Start();
        }

        public static String StripSpaces(String input)
        {
            //return String.Join(" ", input.Split(new Char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            input = Regex.Replace(input, " {1,}", " ");
            //input = Uri.EscapeUriString(input);
            return input;
        }
        public static void ExecCurl(String Command)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("Curl.exe");
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.WindowStyle = ProcessWindowStyle.Minimized;
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.Arguments = Command;

            Process.Start(startInfo);
            Logger.MessageLog(Command);
        }

        /// <summary>
        /// Método local de petición http, sólo GET y PUT
        /// </summary>
        /// <param name="URL">Petición Web</param>
        /// <param name="nc">Usuario y contraseña del dispositivo</param>
        /// <param name="Method">Método del protocolo HTTP</param>
        /// <param name="Data">Comando en PUT, opcional</param>
        /// <returns></returns>
        public static Tuple<WebResponse, String> HTTPRequest(String URL, NetworkCredential nc, String Method, String Data = "") //, WebRequestMethods.Http WRMethod
        {
            //WebRequest Request = WebRequest.Create(URL);
            HttpWebRequest Request = ((HttpWebRequest)WebRequest.Create(URL));
            
            byte[] byteArray = Encoding.UTF8.GetBytes(Data);
            Stream dataStream = Stream.Null;
            StreamReader reader = StreamReader.Null;
            Request.Credentials = nc;
            Request.Method = Method;
            Request.KeepAlive = true;
            Request.Timeout = (5000);
            Request.CookieContainer = new CookieContainer();
            
            HttpWebResponse response = null;
            String responseFromServer = null;
            switch (Method)
            {
                case "GET":
                    try
                    {
                        //Regresa la respuesta del recurso de red
                        response = (HttpWebResponse)Request.GetResponse();
                        //Registra en el Log la petición Web
                        TextInsertion.Logger.MessageLog(URL);
                        //Asigana el flujo de datos del recurso de red solicitado
                        reader = new StreamReader(response.GetResponseStream());
                        responseFromServer = reader.ReadToEnd();
                        reader.Close();
                        //Registra en el Log la respuesta del recurso de red
                        TextInsertion.Logger.ResponseMessages(response.StatusDescription);
                        response.Close();
                    }
                    catch (Exception e)
                    {//Registra el error en el archivo de log
                        TextInsertion.Logger.ErrorMessages(e);
                    }
                    break;
                case "PUT":
                    Request.ContentType = "application/x-www-form-urlencoded";
                    //Request.ContentType = "application/xml";            
                    Request.ContentLength = byteArray.Length;
                    Request.Headers.Set("Authorization", "Basic YWRtaW46YWRtaW4xMjM0");
                    Request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:51.0) Gecko/20100101 Firefox/51.0";
                    Request.Headers.Set("cache-control", "no-cache");

                    dataStream = Request.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();
                    try
                    {
                        response = (HttpWebResponse)Request.GetResponse();
                        reader = new StreamReader(response.GetResponseStream());
                        responseFromServer = reader.ReadToEnd();
                        reader.Close();
                        TextInsertion.Logger.ResponseMessages(response.StatusDescription);
                        response.Close();
                    }
                    catch (Exception e)
                    {
                        TextInsertion.Logger.ErrorMessages(e);
                        //TextInsertion.Program.Run();
                    }                                        
                    break;
                case "POST":
                    break;
            }
            
            Tuple<WebResponse, String> Tanswer = new Tuple<WebResponse, string>(response, responseFromServer);
            return Tanswer;
        }

        public static IRestResponse RestRequest(String URL,String id, String enabled, String positionX, String positionY, String displayText, String Data="")
        {
            var client = new RestClient(URL);//"http://192.168.0.133/ISAPI/System/Video/inputs/channels/1/overlays/text/1"
            var request = new RestRequest(Method.PUT);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/xml");
            request.AddHeader("authorization", "Basic YWRtaW46YWRtaW4xMjM0");
            //request.AddParameter("application/xml", "<?xml version=\"1.0\" encoding=\"UTF-8\"?><TextOverlay version=\"2.0\"><id>4</id><enabled>true</enabled><positionX>0</positionX><positionY>470</positionY><displayText>XXX TLALPAN ISRAEL PORTILLO AGUILE Carril:A06 Estado:NA 14/03/2016 15:47:11</displayText></TextOverlay>\n", ParameterType.RequestBody);
            //XmlNamespaceManager nsMgr = new XmlNamespaceManager();
            //nsMgr.AddNamespace("CGI","http://www.std-cgi.com/ver20/XMLSchema");
            //object x = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><TextOverlay version=\"2.0\"><id>"+id +"</id><enabled>"+enabled+"</enabled><positionX>"+positionX+"</positionX><positionY>"+positionY+"</positionY><displayText>"+displayText+"</displayText></TextOverlay>\n";
            String x = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><TextOverlay version=\"2.0\"><id>" + id + "</id><enabled>" + enabled + "</enabled><positionX>" + positionX + "</positionX><positionY>" + positionY + "</positionY><displayText>" + displayText + "</displayText></TextOverlay>\n";
            x = Uri.EscapeDataString(x);
            request.AddParameter("application/xml", x );
            
            //request.AddXmlBody(x);
            IRestResponse response = null;
            
            //String cmd = @"<?xml version=""1.0"" encoding=""UTF-8""?>
            //                <TextOverlay version=""2.0"" xmlns=""http://www.std-cgi.com/ver20/XMLSchema"">
            //                    <id>2</id>
            //                    <enabled>true</enabled>
            //                    <positionX>0</positionX>
            //                    <positionY>544</positionY>
            //                    <displayText>2222222222</displayText>
            //                </TextOverlay>";
            //IRestResponse response = null;
            //var client = new RestClient(URL);
            //var request = new RestRequest(Method.PUT);
            ////request.AddHeader("cache-control", "no-cache");
            ////request.AddHeader("content-type", "application/xml; charset=\"UTF-8\"");
            //request.AddHeader("authorization", "Basic YWRtaW46YWRtaW4xMjM0");
            ////request.AddParameter("text/xml", "<?xml version=\"1.0\" encoding=\"UTF-8\"?><TextOverlay version=\"2.0\" xmlns=\"http://www.std-cgi.com/ver20/XMLSchema\"><id>1</id><enabled>true</enabled><positionX>0</positionX><positionY>600</positionY><displayText>pd</displayText></TextOverlay>", ParameterType.RequestBody);
            //client.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            //request.AddParameter("application/xml", Data, ParameterType.RequestBody);
            ////request.AddBody(Data, "xmlns=\"http://www.isapi.org/ver20/XMLSchema\"");// "http://www.std-cgi.com/ver20/XMLSchema");
            //request.Credentials = NChttp;
            try
            {
                response = client.Execute(request);                
                TextInsertion.Logger.ResponseMessages(response.StatusCode.ToString());
                TextInsertion.Logger.ResponseMessages(response.Content.ToString());
            }
            catch (Exception e)
            {
                TextInsertion.Logger.ErrorMessages(e);
            }                        
            return response;
        }

        public Camera CAM
        {
            get { return mCAM; }
            set { mCAM = CAM; }
        }

        //public static Task mtSyncTimerTask { get; private set; }

        public Timer TSyncTimer
        {
            get { return mtSyncTimer; }
            set { mtSyncTimer = value; }
        }
    }
}