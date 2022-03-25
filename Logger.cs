using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TextInsertion
{
    class Logger
    {        

        static String Dir = @"C:\CAPUFE\VOIE\Log\";
        static String LogPath = @"C:\CAPUFE\VOIE\Log\" + "TextInsertion@" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss_fff") + ".txt";
        static String LogType = "TextInsertion";
        static String FileVersion;
        static CameraType mCType = CameraType.Default;
        
        public Logger(string _logtype = "TextInsertion")
        {
            LogType = _logtype;
            //LogPath = LogPath + "@" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss_fff") + ".txt";
            //Boolean i = File.Exists(LogPath);
            Assembly Asy = Assembly.GetExecutingAssembly();
            FileVersion = FileVersionInfo.GetVersionInfo(Asy.Location).ProductVersion;//Obtiene la versión actual del TextInsertion
            if (!Directory.Exists(Dir))//Crea el directotio de Log si no existe
            {
                Directory.CreateDirectory(Dir);
            }
            if (!File.Exists(LogPath))//Crea un Log de TextInsertion usando la fecha y hora actual(hasta milisegundos), sólo si este aún no ha sido creado
            {
                using (StreamWriter SW = File.CreateText(LogPath))
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    SW.WriteLine("1|" + DateTime.Now.ToString("dd-MM-yyyy_HH:mm:ss.fff") + ">>-------------------- " + LogType + " Started On Version: " + FileVersion + "--------------------");
                }
            }
        }

        //Agrega el mensaje recidido como argumento al log. Incluye el número de línea, fecha, hora y el mensaje en cuestión. También muestra el mensaje en consola
        public static void MessageLog(string data)
        {
            TenThousandLines(LogPath);
            int LastLine = File.ReadLines(LogPath).Count();
            int Count = LastLine + 1;
            //Dependiendo del tipo de cámara se usa un color de texto diferente en consola, también se especifica el tipo de camara en el mensaje
            switch (mCType)
            {
                case CameraType.Default:
                    using (StreamWriter SW = File.AppendText(LogPath))
                    {
                        SW.WriteLine(Count.ToString() + "|" + DateTime.Now.ToString("dd-MM-yyyy_HH:mm:ss.fff") + ">>" + data);
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(DateTime.Now.ToString("dd-MM-yyyy_HH:mm:ss.fff") + ">>" + data);
                    }
                    break;
                case CameraType.BC620:
                    using (StreamWriter SW = File.AppendText(LogPath))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        SW.WriteLine(Count.ToString() + "|" + DateTime.Now.ToString("dd-MM-yyyy_HH:mm:ss.fff") + ">>BC620_" + data);
                        Console.WriteLine(DateTime.Now.ToString("dd-MM-yyyy_HH:mm:ss.fff") + ">>BC620_" + data);
                    }
                    break;
                case CameraType.BC840:
                    using (StreamWriter SW = File.AppendText(LogPath))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        SW.WriteLine(Count.ToString() + "|" + DateTime.Now.ToString("dd-MM-yyyy_HH:mm:ss.fff") + ">>BC840_" + data);
                        Console.WriteLine(DateTime.Now.ToString("dd-MM-yyyy_HH:mm:ss.fff") + ">>BC840_" + data);
                    }
                    break;
                case CameraType.IFD820:
                    using (StreamWriter SW = File.AppendText(LogPath))
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        SW.WriteLine(Count.ToString() + "|" + DateTime.Now.ToString("dd-MM-yyyy_HH:mm:ss.fff") + ">>IFD820_" + data);
                        Console.WriteLine(DateTime.Now.ToString("dd-MM-yyyy_HH:mm:ss.fff") + ">>IFD820_" + data);
                    }
                    break;
                case CameraType.BC950:
                    using (StreamWriter SW = File.AppendText(LogPath))
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        SW.WriteLine(Count.ToString() + "|" + DateTime.Now.ToString("dd-MM-yyyy_HH:mm:ss.fff") + ">>BC950_" + data);
                        Console.WriteLine(DateTime.Now.ToString("dd-MM-yyyy_HH:mm:ss.fff") + ">>BC950_" + data);
                    }
                    break;
                case CameraType.BC1103:
                    using (StreamWriter SW = File.AppendText(LogPath))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        SW.WriteLine(Count.ToString() + "|" + DateTime.Now.ToString("dd-MM-yyyy_HH:mm:ss.fff") + ">>BC1103_" + data);
                        Console.WriteLine(DateTime.Now.ToString("dd-MM-yyyy_HH:mm:ss.fff") + ">>BC1103_" + data);
                    }
                    break;
                case CameraType.FD1103:
                    using (StreamWriter SW = File.AppendText(LogPath))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                        SW.WriteLine(Count.ToString() + "|" + DateTime.Now.ToString("dd-MM-yyyy_HH:mm:ss.fff") + ">>FD1103_" + data);
                        Console.WriteLine(DateTime.Now.ToString("dd-MM-yyyy_HH:mm:ss.fff") + ">>FD1103_" + data);
                    }
                    break;
            }

        }

        //Agrega el mensaje recidido como argumento al log. Incluye el número de línea, fecha, hora y el mensaje en cuestión. También muestra el mensaje en consola
        public static void SyncTimeLog(string data)
        {
            TenThousandLines(LogPath);
            int LastLine = File.ReadLines(LogPath).Count();
            int Count = LastLine + 1;
            switch (mCType)
            {
                case CameraType.Default:
                    using (StreamWriter SW = File.AppendText(LogPath))
                    {
                        SW.WriteLine(Count.ToString() + "|" + DateTime.Now.ToString("dd-MM-yyyy_HH:mm:ss.fff") + ">>" + data);
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(DateTime.Now.ToString("dd-MM-yyyy_HH:mm:ss.fff") + ">>" + data);
                    }
                    break;
                case CameraType.BC620:
                    using (StreamWriter SW = File.AppendText(LogPath))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        SW.WriteLine(Count.ToString() + "|" + DateTime.Now.ToString("dd-MM-yyyy_HH:mm:ss.fff") + ">>BC620_" + data);
                        Console.WriteLine(DateTime.Now.ToString("dd-MM-yyyy_HH:mm:ss.fff") + ">>BC620_" + data);
                    }
                    break;
                case CameraType.BC840:
                    using (StreamWriter SW = File.AppendText(LogPath))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        SW.WriteLine(Count.ToString() + "|" + DateTime.Now.ToString("dd-MM-yyyy_HH:mm:ss.fff") + ">>BC840_" + data);
                        Console.WriteLine(DateTime.Now.ToString("dd-MM-yyyy_HH:mm:ss.fff") + ">>BC840_" + data);
                    }
                    break;
                case CameraType.IFD820:
                    using (StreamWriter SW = File.AppendText(LogPath))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        SW.WriteLine(Count.ToString() + "|" + DateTime.Now.ToString("dd-MM-yyyy_HH:mm:ss.fff") + ">>IFD820_" + data);
                        Console.WriteLine(DateTime.Now.ToString("dd-MM-yyyy_HH:mm:ss.fff") + ">>IFD820_" + data);
                    }
                    break;
                case CameraType.BC1103:
                    using (StreamWriter SW = File.AppendText(LogPath))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        SW.WriteLine(Count.ToString() + "|" + DateTime.Now.ToString("dd-MM-yyyy_HH:mm:ss.fff") + ">>BC1103_" + data);
                        Console.WriteLine(DateTime.Now.ToString("dd-MM-yyyy_HH:mm:ss.fff") + ">>BC1103_" + data);
                    }
                    break;
                case CameraType.FD1103:
                    using (StreamWriter SW = File.AppendText(LogPath))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        SW.WriteLine(Count.ToString() + "|" + DateTime.Now.ToString("dd-MM-yyyy_HH:mm:ss.fff") + ">>FD1103_" + data);
                        Console.WriteLine(DateTime.Now.ToString("dd-MM-yyyy_HH:mm:ss.fff") + ">>FD1103_" + data);
                    }
                    break;
            }

        }

        //Si el archivo Log de TextInsertion es mayor o igual a 10,000 líneas se crea un nuevo Log con la fecha y hora actual
        private static void TenThousandLines(string filepath)
        {
            if (File.ReadLines(LogPath).Count() > 9999)
            {
                LogPath = Dir + LogType + "@" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss_fff") + ".txt";
            }
            if (!File.Exists(LogPath))
            {
                using (StreamWriter SW = File.CreateText(LogPath))
                {
                    SW.WriteLine("1|" + DateTime.Now.ToString("dd-MM-yyyy_HH:mm:ss.fff") + ">>-------------------- " + LogType + " Continued on Version: " + FileVersion +"--------------------");
                }
            }
        }

        //Agrega un mensaje de error en el Log de TExtInsertion y también lo muestra en consola
        //Método sobre cargado, sólo cambia el tipo de error como parámetro
        public static void ErrorMessages(Exception e)
        {
            TenThousandLines(LogPath);
            int LastLine = File.ReadLines(LogPath).Count();
            int Count = LastLine + 1;
            using (StreamWriter SW = File.AppendText(LogPath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                SW.WriteLine(Count.ToString() + "|" + DateTime.Now.ToString("dd-MM-yyyy_HH:mm:ss.fff") + ">>ERROR => KO : " + e.Message + "\n" + e.StackTrace);
                Console.WriteLine(DateTime.Now.ToString("dd-MM-yyyy_HH:mm:ss.fff") + " 1>>ERROR KO : " + e.Message + "\n" + e.StackTrace);
            }
        }
        public static void ErrorMessages(ErrorEventArgs e)
        {
            TenThousandLines(LogPath);
            int LastLine = File.ReadLines(LogPath).Count();
            int Count = LastLine + 1;
            using (StreamWriter SW = File.AppendText(LogPath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                SW.WriteLine(Count.ToString() + "|" + DateTime.Now.ToString("dd-MM-yyyy_HH:mm:ss.fff") + ">>ERROR => KO : " + e.GetException().Message + "\n" + e.GetException().StackTrace);
                Console.WriteLine(DateTime.Now.ToString("dd-MM-yyyy_HH:mm:ss.fff") + " 2>>ERROR KO : " + e.GetException().Message + "\n" + e.GetException().StackTrace);
            }
        }
        public static void ErrorMessages(UnhandledExceptionEventArgs e)
        {
            TenThousandLines(LogPath);
            int LastLine = File.ReadLines(LogPath).Count();
            int Count = LastLine + 1;
            Exception ex = (Exception)e.ExceptionObject;
            using (StreamWriter SW = File.AppendText(LogPath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                SW.WriteLine(Count.ToString() + "|" + DateTime.Now.ToString("dd-MM-yyyy_HH:mm:ss.fff") + ">>ERROR => KO : " + ex.Message + "\n" + ex.StackTrace + "\n" + ex.Source);
                Console.WriteLine(DateTime.Now.ToString("dd-MM-yyyy_HH:mm:ss.fff") + " 3>>ERROR KO : " + ex.Message + "\n" + ex.StackTrace);
            }
        }
        //Recibe un mensaje y lo registra en el Log y consola
        public static void ResponseMessages(String e)
        {
            TenThousandLines(LogPath);
            int LastLine = File.ReadLines(LogPath).Count();
            int Count = LastLine + 1;
            using (StreamWriter SW = File.AppendText(LogPath))
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                SW.WriteLine(Count.ToString() + "|" + DateTime.Now.ToString("dd-MM-yyyy_HH:mm:ss.fff") + ">>RESPONSE => " + e);
                Console.WriteLine(DateTime.Now.ToString("dd-MM-yyyy_HH:mm:ss.fff") + ">>RESPONSE => " + e);
            }
        }

        public CameraType CType
        {
            set { mCType = value; }
        }
    }
}
