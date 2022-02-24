using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace TextInsertion
{
    class Program
    {
        static Boolean FirstOccurence = true;
        static FileSystemWatcher FSW = new FileSystemWatcher();
        static CameraType mcTypeContext = CameraType.Default;
        public static Camera mContext;
        static CameraType mcTypeContext_2 = CameraType.Default;        
        public static Camera mContext_2;       
        static CameraType mcTypeDomo = CameraType.Default;
        public static Camera mDomo;
        static CameraType mcTypeDomo_2 = CameraType.Default;
        public static Camera mDomo_2;

        static Boolean OpenLane;
        
        public static Logger l = new Logger();        
        public static IPTextInsertionManager IPM = new IPTextInsertionManager();

        static NetworkCredential NChttp = new NetworkCredential("Admin", "1234");

        static bool createdNew = false;
        static Mutex instanceMutex = new Mutex(true, "TextInsertion", out createdNew);
        static OnChanged_StackManager OC_SM = new OnChanged_StackManager();
        //public static int FSWBufferSize = 65536;

        [STAThread]
        static void Main()
        {
            //Console.WriteLine("Please Type the Buffer Size");
            //Program.FSWBufferSize = int.Parse(Console.ReadLine());

            //Evento para controlar excepciones que no fueron cachadas
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            try
            {
                using (new SingleGlobalInstance(1000))
                {
                    IPTextInsertionManager mIP = new IPTextInsertionManager();
                    Run();
                }

            }
            catch (Exception)
            {
                //TextInsertion.Logger.ErrorMessages(e);
                Environment.Exit(1);
            }
        }
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            TextInsertion.Logger.ErrorMessages(e);
        }
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static void Run()
        {            
            IPM.GetLocalIPAddress();

            //IPM.IPCONTEXT = IPAddress.Parse("172.30.16.12");
            //IPM.IPCONTEXT_2 = IPAddress.Parse("172.30.16.13");
            //IPM.IPDOME = IPAddress.Parse("172.30.16.14");
            //IPM.IPDOME_2 = IPAddress.Parse("172.30.16.15");

            //IPM.IPCONTEXT = IPAddress.Parse("192.168.0.133");
            //IPM.IPCONTEXT_2 = IPAddress.Parse("192.168.0.134");
            //IPM.IPDOME = IPAddress.Parse("192.168.0.135");
            //IPM.IPDOME_2 = IPAddress.Parse("192.168.0.136");

            //IPM.IPCONTEXT = IPAddress.Parse("10.3.25.32");
            //
            //IPM.IPDOME = IPAddress.Parse("10.3.25.34");
            //IPM.IPDOME_2 = IPAddress.Parse("10.3.25.35");

            mcTypeContext = Camera.DetectCameraType(IPM.IPCONTEXT);
            mcTypeContext_2 = Camera.DetectCameraType(IPM.IPCONTEXT_2);
            mcTypeDomo = Camera.DetectCameraType(IPM.IPDOME);
            mcTypeDomo_2 = Camera.DetectCameraType(IPM.IPDOME_2);
            String[] TextInsertLines = null;
            String[] TextInsertLinesOld = null;

            TextInsertLines = File.ReadAllLines(@"C:\CAPUFE\VOIE\VIDEO\VideoTxt.txt");
            //TextInsertLines = File.ReadAllLines(@"\\192.168.0.132\CAPUFE\VOIE\VIDEO\VideoTxt.txt");
            TextInsertLinesOld = EscapeS(TextInsertLines);
            

            if (TextInsertLines.Length >= 3) 
            { 
                OpenLane = true;
            }
            else
            {
                OpenLane = false;
            }

            switch (mcTypeContext)
            {
                case CameraType.BC620:
                    mContext = new BC620();
                    ((BC620)mContext).Configure(mContext, IPM.IPCONTEXT);
                    ((BC620)mContext).Send(mContext, IPM.IPCONTEXT, 1, TextInsertLinesOld[0]);
                    ((BC620)mContext).Send(mContext, IPM.IPCONTEXT, 2, TextInsertLinesOld[1]);
                    break;
                case CameraType.BC840:
                    mContext = new BC840();
                    ((BC840)mContext).Configure(mContext, IPM.IPCONTEXT);
                    ((BC840)mContext).Send(mContext, IPM.IPCONTEXT, 1, TextInsertLinesOld[0]);
                    ((BC840)mContext).Send(mContext, IPM.IPCONTEXT, 2, TextInsertLinesOld[1]);
                    break;
                case CameraType.BC1103:
                    mContext = new BC1103(IPM.IPCONTEXT);
                    ((BC1103)mContext).Configure(IPM.IPCONTEXT);                    
                    ((BC1103)mContext).Send(IPM.IPCONTEXT, 1, TextInsertLines[0]);
                    ((BC1103)mContext).Send(IPM.IPCONTEXT, 2, TextInsertLines[1]);
                    if (OpenLane)
                    {
                        ((BC1103)mContext).Send(IPM.IPCONTEXT, 3, TextInsertLines[2]);
                    }
                    else
                    {
                        ((BC1103)mContext).Send(IPM.IPCONTEXT, 3, "");
                    }
                    break;
            }
            switch (mcTypeContext_2)
            {
                case CameraType.BC620:
                    mContext_2 = new BC620();
                    ((BC620)mContext_2).Configure(mContext_2, IPM.IPCONTEXT_2);
                    ((BC620)mContext_2).Send(mContext_2, IPM.IPCONTEXT_2, 1, TextInsertLinesOld[0]);
                    ((BC620)mContext_2).Send(mContext_2, IPM.IPCONTEXT_2, 2, TextInsertLinesOld[1]);
                    break;
                case CameraType.BC840:
                    mContext_2 = new BC840();
                    ((BC840)mContext_2).Configure(mContext_2, IPM.IPCONTEXT_2);
                    ((BC840)mContext_2).Send(mContext_2, IPM.IPCONTEXT_2, 1, TextInsertLinesOld[0]);
                    ((BC840)mContext_2).Send(mContext_2, IPM.IPCONTEXT_2, 2, TextInsertLinesOld[1]);
                    break;
                case CameraType.BC1103:
                    mContext_2 = new BC1103(IPM.IPCONTEXT_2);
                    ((BC1103)mContext_2).Configure(IPM.IPCONTEXT_2);
                    ((BC1103)mContext_2).Send(IPM.IPCONTEXT_2, 1, TextInsertLines[0]);
                    ((BC1103)mContext_2).Send(IPM.IPCONTEXT_2, 2, TextInsertLines[1]);
                    if (OpenLane)
                    {
                        ((BC1103)mContext_2).Send(IPM.IPCONTEXT_2, 3, TextInsertLines[2]);
                    }
                    else
                    {
                        ((BC1103)mContext_2).Send(IPM.IPCONTEXT_2, 3, "");
                    }
                    break;
            }
            switch (mcTypeDomo)
            {
                case CameraType.IFD820:
                    mDomo = new IFD820(TextInsertLines, IPM.IPDOME);
                    ((IFD820)mDomo).Configure(mDomo,IPM.IPDOME);                    
                    ((IFD820)mDomo).Send(mDomo, IPM.IPDOME, 1, OpenLane, TextInsertLinesOld[0]);
                    ((IFD820)mDomo).Send(mDomo, IPM.IPDOME, 2, OpenLane, TextInsertLinesOld[1]);
                    if (OpenLane)
                    {
                        ((IFD820)mDomo).Send(mDomo, IPM.IPDOME, 3, OpenLane, TextInsertLinesOld[2]);
                    }
                    else
                    {
                        ((IFD820)mDomo).Send(mDomo, IPM.IPDOME, 3, OpenLane, "");
                    }
                    break;
                case CameraType.FD1103:
                    mDomo = new FD1103(IPM.IPDOME);
                    ((FD1103)mDomo).Configure(IPM.IPDOME);
                    ((FD1103)mDomo).Send(IPM.IPDOME, 1, TextInsertLines[0]);
                    ((FD1103)mDomo).Send(IPM.IPDOME, 2, TextInsertLines[1]);
                    if (OpenLane)
                    {
                        ((FD1103)mDomo).Send(IPM.IPDOME, 3, TextInsertLines[2]);
                    }
                    else
                    {
                        ((FD1103)mDomo).Send(IPM.IPDOME, 3, "");
                    }
                    break;
            }
            switch (mcTypeDomo_2)
            {
                case CameraType.IFD820:
                    mDomo_2 = new IFD820(TextInsertLines, IPM.IPDOME_2);
                    ((IFD820)mDomo_2).Configure(mDomo_2, IPM.IPDOME_2);
                    ((IFD820)mDomo_2).Send(mDomo_2, IPM.IPDOME_2, 1, OpenLane, TextInsertLinesOld[0]);
                    ((IFD820)mDomo_2).Send(mDomo_2, IPM.IPDOME_2, 2, OpenLane, TextInsertLinesOld[1]);
                    if (OpenLane)
                    {
                        ((IFD820)mDomo_2).Send(mDomo_2, IPM.IPDOME_2, 3, OpenLane, TextInsertLinesOld[2]);
                    }
                    else
                    {
                        ((IFD820)mDomo_2).Send(mDomo_2, IPM.IPDOME_2, 3, OpenLane, "");
                    }
                    break;
                case CameraType.FD1103: 
                    mDomo_2 = new FD1103(IPM.IPDOME_2);
                    ((FD1103)mDomo_2).Configure(IPM.IPDOME_2);
                    ((FD1103)mDomo_2).Send(IPM.IPDOME_2, 1, TextInsertLines[0]);
                    ((FD1103)mDomo_2).Send(IPM.IPDOME_2, 2, TextInsertLines[1]);
                    if (OpenLane)
                    {
                        ((FD1103)mDomo_2).Send(IPM.IPDOME_2, 3, TextInsertLines[2]);
                    }
                    else
                    {
                        ((FD1103)mDomo_2).Send(IPM.IPDOME_2, 3, "");
                    }
                    break;
            }
            //Camera.TSyncTimer = new Timer(Camera.TimeSyncLane, null, 0, 60000);
            if (mcTypeContext== CameraType.BC1103)
            {
                mContext.TSyncTimer.Start();
            }
            if (mcTypeContext_2 == CameraType.BC1103)
            {
                mContext_2.TSyncTimer.Start();
            }
            if (mcTypeDomo == CameraType.FD1103)
            {
                mDomo.TSyncTimer.Start();
            }
            if (mcTypeDomo_2 == CameraType.FD1103)
            {
                mDomo_2.TSyncTimer.Start();
            }

            String StartFolder = @"C:\CAPUFE\VOIE\VIDEO\";
            //String StartFolder = @"\\192.168.0.132\CAPUFE\VOIE\VIDEO\";

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            String FileVersion = fvi.FileVersion;            

            FSW.Path = StartFolder;
            FSW.NotifyFilter = NotifyFilters.LastWrite;
            FSW.Filter = "VideoTxt.txt";
            FSW.InternalBufferSize = 16*1024;
            FSW.IncludeSubdirectories = false;

            FSW.Changed += new FileSystemEventHandler(OnChanged);

            FSW.EnableRaisingEvents = true;

            FSW.Error += new ErrorEventHandler(OnFSWError);

            while (Console.ReadLine() != "q")
            {

            }
        }

        private static void OnFSWError(object source, ErrorEventArgs e)
        {
            Logger.ErrorMessages(e);

            //FSW.BeginInit();
            //if (e.GetException().GetType() == typeof(InternalBufferOverflowException))
            //{
            //    //Logger.MessageLog(("The file system watcher experienced an internal buffer overflow: " + e.GetException().Message));
            //}
        }

        public static void OnChanged(object source, FileSystemEventArgs e)
        {
            FSW.EnableRaisingEvents = false;
            l.CType = CameraType.Default;           
            OC_SM.ONCHANGEDBC.Add(e);  
            FSW.EnableRaisingEvents = true;            
        }
        public class OnChanged_StackManager
        {
            int interval = 1;
            public static Boolean IsRunning = false;
            public static BlockingCollection<FileSystemEventArgs> mOnChangedBC;
            public OnChanged_StackManager()
            {
                try
                {
                    mOnChangedBC = new BlockingCollection<FileSystemEventArgs>();
                }
                catch (Exception e)
                {
                    Logger.ErrorMessages(e);
                }
                runAsyncTasks();
            }
            private void runAsyncTasks()
            {
                CancellationToken ct = new CancellationToken();
                Task StackMangerTask = Task.Factory.StartNew(() => ConsumeAsync(interval, ct));
            }
            public void ConsumeAsync(int interval, CancellationToken ct)
            {
                IsRunning = true;
                try
                {
                    while (IsRunning)
                    {
                        if (mOnChangedBC.Count > 0)
                        {
                            Task StackManagerTask = Task.Factory.StartNew(() => InsertTextOnChanged(interval, ct, mOnChangedBC.Take()));
                            try
                            {
                                StackManagerTask.Wait();
                            }
                            catch (AggregateException ex)
                            {
                                Logger.ErrorMessages(ex);
                            }        
                        }                
                    }
                }
                catch(Exception e)
                {
                    Logger.ErrorMessages(e);
                    runAsyncTasks();
                }
            }
            public BlockingCollection<FileSystemEventArgs> ONCHANGEDBC
            {
                get { return mOnChangedBC; }
                set { mOnChangedBC = value; }
            }
        }
        public static void InsertTextOnChanged(int interval, CancellationToken ct, FileSystemEventArgs e)
        {
            String[] TextInsertLines;
            String[] TextInsertLinesOld; 
            try
            {
                Logger.MessageLog("File: " + e.FullPath + " " + e.ChangeType);
                
                TextInsertLines = File.ReadAllLines(e.FullPath);
                TextInsertLinesOld = EscapeS(TextInsertLines);

                if (TextInsertLines.Length >= 3)
                {
                    OpenLane = true;
                }
                else
                {
                    OpenLane = false;
                }

                if (FirstOccurence)
                {
                    switch (mcTypeContext)
                    {
                        case CameraType.BC620:
                            ((BC620)mContext).Send(mContext, IPM.IPCONTEXT, 1, TextInsertLinesOld[0]);
                            ((BC620)mContext).Send(mContext, IPM.IPCONTEXT, 2, TextInsertLinesOld[1]);
                            break;
                        case CameraType.BC840:                                    
                            ((BC840)mContext).Send(mContext, IPM.IPCONTEXT, 1, TextInsertLinesOld[0]);
                            ((BC840)mContext).Send(mContext, IPM.IPCONTEXT, 2, TextInsertLinesOld[1]);
                            break;
                        case CameraType.BC1103:
                            ((BC1103)mContext).Send(IPM.IPCONTEXT, 1, TextInsertLines[0]);
                            ((BC1103)mContext).Send(IPM.IPCONTEXT, 2, TextInsertLines[1]);
                            break;                    
                    }             

                    if (mcTypeContext_2 != CameraType.Default || mcTypeContext_2 != CameraType.Not_Connected)
                    {
                        switch (mcTypeContext_2)
                        {
                            case CameraType.BC620:
                                ((BC620)mContext_2).Send(mContext_2, IPM.IPCONTEXT_2, 1, TextInsertLinesOld[0]);
                                ((BC620)mContext_2).Send(mContext_2, IPM.IPCONTEXT_2, 2, TextInsertLinesOld[1]);
                                break;
                            case CameraType.BC840:                            
                                ((BC840)mContext_2).Send(mContext_2, IPM.IPCONTEXT_2, 1, TextInsertLinesOld[0]);
                                ((BC840)mContext_2).Send(mContext_2, IPM.IPCONTEXT_2, 2, TextInsertLinesOld[1]);                            
                                break;
                            case CameraType.BC1103:
                                ((BC1103)mContext_2).Send(IPM.IPCONTEXT_2, 1, TextInsertLines[0]);
                                ((BC1103)mContext_2).Send(IPM.IPCONTEXT_2, 2, TextInsertLines[1]);
                                break; 
                        }                    
                    }
                    switch (mcTypeDomo)
                    {
                        case CameraType.IFD820:
                            ((IFD820)mDomo).Send(mDomo, IPM.IPDOME, 1, OpenLane, TextInsertLinesOld[0]);//((IFD820)mDomo).Send(mDomo, IPM.IPDOME, 1, ((IFD820)mDomo).ExtractClock(TextInsertLinesOld[0]).Item1);
                            ((IFD820)mDomo).Send(mDomo, IPM.IPDOME, 2, OpenLane, TextInsertLinesOld[1]);                        
                            break;
                        case CameraType.FD1103:                       
                            ((FD1103)mDomo).Send(IPM.IPDOME, 1, TextInsertLines[0]);
                            ((FD1103)mDomo).Send(IPM.IPDOME, 2, TextInsertLines[1]);
                            break;
                    }
                    if (mcTypeDomo_2 != CameraType.Default || mcTypeDomo_2 != CameraType.Not_Connected)
                    {
                        switch (mcTypeDomo_2)
                        {
                        case CameraType.IFD820:
                            ((IFD820)mDomo_2).Send(mDomo_2, IPM.IPDOME_2, 1, OpenLane, TextInsertLinesOld[0]);    //((IFD820)mDomo_2).Send(mDomo_2, IPM.IPDOME_2, 1, ((IFD820)mDomo).ExtractClock(TextInsertLinesOld[0]).Item1);
                            ((IFD820)mDomo_2).Send(mDomo_2, IPM.IPDOME_2, 2, OpenLane, TextInsertLinesOld[1]);
                            break;
                        case CameraType.FD1103:                       
                            ((FD1103)mDomo_2).Send(IPM.IPDOME_2, 1, TextInsertLines[0]);
                            ((FD1103)mDomo_2).Send(IPM.IPDOME_2, 2, TextInsertLines[1]);
                            break;
                        }
                    }
                    if (OpenLane)
                    {
                        switch (mcTypeContext)
                        {
                            case CameraType.BC620:
                                l.CType = CameraType.BC620;
                                ((BC620)mContext).Send(mContext, IPM.IPCONTEXT, 3, TextInsertLinesOld[2]);
                                break;
                            case CameraType.BC840:
                                l.CType = CameraType.BC840;
                                ((BC840)mContext).Send(mContext, IPM.IPCONTEXT, 3, TextInsertLinesOld[2]);
                                break;
                            case CameraType.BC1103:
                                l.CType = CameraType.BC1103;
                                if (TextInsertLines.Count() > 2)
                                {
                                    ((BC1103)mContext).Send(IPM.IPCONTEXT, 3, TextInsertLines[2]);
                                }
                                break;
                        }                                      
                        if (mcTypeContext_2 != CameraType.Default || mcTypeContext_2 != CameraType.Not_Connected)
                        {
                            switch (mcTypeContext_2)
                            {
                                case CameraType.BC620:
                                    l.CType = CameraType.BC620;
                                    ((BC620)mContext_2).Send(mContext_2, IPM.IPCONTEXT_2, 3, TextInsertLinesOld[2]);
                                    break;
                                case CameraType.BC840:
                                    l.CType = CameraType.BC840;
                                    ((BC840)mContext_2).Send(mContext_2, IPM.IPCONTEXT_2, 3, TextInsertLinesOld[2]);
                                    break;
                                case CameraType.BC1103:
                                    l.CType = CameraType.BC1103;
                                    if (TextInsertLines.Count() > 2)
                                    {
                                        ((BC1103)mContext_2).Send(IPM.IPCONTEXT_2, 3, TextInsertLines[2]);
                                    }
                                    break;  
                            }
                        }
                        switch (mcTypeDomo)
                        {
                            case CameraType.IFD820:
                                l.CType = CameraType.IFD820;
                                ((IFD820)mDomo).Send(mDomo, IPM.IPDOME, 3, OpenLane, TextInsertLinesOld[2]);
                                //((IFD820)mDomo).BuildCommand(IPM.IPDOME, IFD820.CGICommands.Date, 3, ((IFD820)mDomo).ExtractClock(TextInsertLinesOld[0]).Item2);
                                break;
                            case CameraType.FD1103:
                                l.CType = CameraType.FD1103;
                                if (TextInsertLines.Count() > 2)
                                {
                                    ((FD1103)mDomo).Send(IPM.IPDOME, 3, TextInsertLines[2]);
                                }
                                break;
                        }
                        if (mcTypeDomo_2 != CameraType.Default || mcTypeDomo_2 != CameraType.Not_Connected)
                        {
                            switch (mcTypeDomo_2)
                            {
                            case CameraType.IFD820:
                                    l.CType = CameraType.IFD820;
                                    ((IFD820)mDomo_2).Send(mDomo_2, IPM.IPDOME_2, 3, OpenLane, TextInsertLinesOld[2]);
                                    //((IFD820)mDomo_2).BuildCommand(IPM.IPDOME_2, IFD820.CGICommands.Date, 3, ((IFD820)mDomo_2).ExtractClock(TextInsertLinesOld[0]).Item2);
                                break;
                            case CameraType.FD1103:
                                    l.CType = CameraType.FD1103;
                                    if (TextInsertLines.Count() > 2)
                                    {
                                        ((FD1103)mDomo_2).Send(IPM.IPDOME_2, 3, TextInsertLines[2]);
                                    }                                                 
                                break;
                            }
                        }

                    }
                    else
                    {
                        switch (mcTypeContext)
                        {
                            case CameraType.BC620:
                                l.CType = CameraType.BC620;
                                Camera.HTTPRequest(((BC620)mContext).BuildCommand(IPM.IPCONTEXT, BC620.CGICommands.TextAndDate, 3, ""), NChttp, "GET");
                                break;
                            case CameraType.BC840:
                                l.CType = CameraType.BC840;
                                Camera.HTTPRequest(((BC840)mContext).BuildCommand(IPM.IPCONTEXT, BC840.CGICommands.TextAndDate, 3, ""), NChttp, "GET");
                                break;
                            case CameraType.BC1103:
                                l.CType = CameraType.BC1103;
                                if (TextInsertLines.Count() > 2)
                                {
                                    ((BC1103)mContext).Send(IPM.IPCONTEXT, 3, TextInsertLines[2]);
                                }
                                break;
                        }
                        if (mcTypeContext_2 != CameraType.Default || mcTypeContext_2 != CameraType.Not_Connected)
                        {
                            switch (mcTypeContext_2)
                            {
                                case CameraType.BC620:
                                    l.CType = CameraType.BC620;
                                    Camera.HTTPRequest(((BC620)mContext_2).BuildCommand(IPM.IPCONTEXT_2, BC620.CGICommands.TextAndDate, 3, ""), NChttp, "GET");
                                    break;
                                case CameraType.BC840:
                                    l.CType = CameraType.BC840;
                                    Camera.HTTPRequest(((BC840)mContext_2).BuildCommand(IPM.IPCONTEXT_2, BC840.CGICommands.TextAndDate, 3, ""), NChttp, "GET");
                                    break;
                                case CameraType.BC1103:
                                    l.CType = CameraType.BC1103;
                                    if (TextInsertLines.Count() > 2)
                                    {
                                        ((BC1103)mContext_2).Send(IPM.IPCONTEXT_2, 3, TextInsertLines[2]);
                                    }
                                    break;
                            }
                        }
                        switch (mcTypeDomo)
                        {
                            case CameraType.IFD820:
                                l.CType = CameraType.IFD820;
                                ((IFD820)mDomo).Send(mDomo, IPM.IPDOME, 3, OpenLane, TextInsertLinesOld[2]);
                                //((IFD820)mDomo).BuildCommand(IPM.IPDOME, IFD820.CGICommands.Date, 3, ((IFD820)mDomo).ExtractClock(TextInsertLinesOld[0]).Item2);
                                break;
                            case CameraType.FD1103:
                                l.CType = CameraType.FD1103;
                                if (TextInsertLines.Count() > 2)
                                {
                                    ((FD1103)mDomo).Send(IPM.IPDOME, 3, TextInsertLines[2]);
                                }
                                break;
                        }
                        if (mcTypeDomo_2 != CameraType.Default || mcTypeDomo_2 != CameraType.Not_Connected)
                        {
                            switch (mcTypeDomo_2)
                            {
                                case CameraType.IFD820:
                                    l.CType = CameraType.IFD820;
                                    ((IFD820)mDomo_2).Send(mDomo_2, IPM.IPDOME_2, 3, OpenLane, TextInsertLinesOld[2]);
                                    //((IFD820)mDomo).BuildCommand(IPM.IPDOME, IFD820.CGICommands.Date, 3, ((IFD820)mDomo).ExtractClock(TextInsertLinesOld[0]).Item2);
                                    break;
                                case CameraType.FD1103:
                                    l.CType = CameraType.FD1103;
                                    if (TextInsertLines.Count() > 2)
                                    {
                                        ((FD1103)mDomo_2).Send(IPM.IPDOME_2, 3, TextInsertLines[2]);
                                    }
                                    break;
                            }
                        }
          
                    }
                    FirstOccurence = false;
                }
                else
                {
                    switch (mcTypeContext)
                    {
                        case CameraType.BC620:
                            l.CType = CameraType.BC620;
                            ((BC620)mContext).Send(mContext, IPM.IPCONTEXT, 1, TextInsertLinesOld[0]);
                            break;
                        case CameraType.BC840:
                            l.CType = CameraType.BC840;
                            ((BC840)mContext).Send(mContext, IPM.IPCONTEXT, 1, TextInsertLinesOld[0]);
                            break;
                        case CameraType.BC1103:
                            ((BC1103)mContext).Send(IPM.IPCONTEXT, 1, TextInsertLines[0]);
                            break;
                    }
                    if (mcTypeContext_2 != CameraType.Default || mcTypeContext_2 != CameraType.Not_Connected)
                    {
                        switch (mcTypeContext_2)
                        {
                            case CameraType.BC620:
                                l.CType = CameraType.BC620;
                                ((BC620)mContext_2).Send(mContext_2, IPM.IPCONTEXT_2, 1, TextInsertLinesOld[0]);
                                break;
                            case CameraType.BC840:
                                l.CType = CameraType.BC840;
                                ((BC840)mContext_2).Send(mContext_2, IPM.IPCONTEXT_2, 1, TextInsertLinesOld[0]);
                                break;
                            case CameraType.BC1103:
                                l.CType = CameraType.BC1103;
                                ((BC1103)mContext_2).Send(IPM.IPCONTEXT_2, 1, TextInsertLines[0]);
                                break;
                        }
                    }
                    switch (mcTypeDomo)
                    {
                        case CameraType.IFD820:
                            //Removed to avoid blinking
                            //l.CType = CameraType.IFD820;                        
                            ((IFD820)mDomo).Send(mDomo, IPM.IPDOME, 1, OpenLane, TextInsertLinesOld[0]); //((IFD820)mDomo).Send(mDomo, IPM.IPDOME, 1, ((IFD820)mDomo).ExtractClock(TextInsertLinesOld[0]).Item1);
                            break;
                        case CameraType.FD1103:
                            ((FD1103)mDomo).Send(IPM.IPDOME, 1, TextInsertLines[0]);
                            break;
                    }
                    if (mcTypeDomo_2 != CameraType.Default || mcTypeDomo_2 != CameraType.Not_Connected)
                    {
                        switch (mcTypeDomo_2)
                        {
                            case CameraType.IFD820:
                                //Removed to avoid blinking
                                ((IFD820)mDomo_2).Send(mDomo_2, IPM.IPDOME_2, 1, OpenLane, TextInsertLinesOld[0]);//((IFD820)mDomo_2).Send(mDomo_2, IPM.IPDOME_2, 1, ((IFD820)mDomo).ExtractClock(TextInsertLinesOld[0]).Item1);
                                break;
                            case CameraType.FD1103:
                                ((FD1103)mDomo_2).Send(IPM.IPDOME_2, 1,TextInsertLines[0]);
                                break;
                        }
                    }
                    if (OpenLane)
                    {
                        switch (mcTypeContext)
                        {
                            case CameraType.BC620:
                                l.CType = CameraType.BC620;
                                ((BC620)mContext).Send(mContext, IPM.IPCONTEXT, 2, TextInsertLinesOld[1]);
                                ((BC620)mContext).Send(mContext, IPM.IPCONTEXT, 3, TextInsertLinesOld[2]);
                                break;
                            case CameraType.BC840:
                                l.CType = CameraType.BC840;
                                ((BC840)mContext).Send(mContext, IPM.IPCONTEXT, 2, TextInsertLinesOld[1]);
                                ((BC840)mContext).Send(mContext, IPM.IPCONTEXT, 3, TextInsertLinesOld[2]);
                                break;
                            case CameraType.BC1103:
                                ((BC1103)mContext).Send(IPM.IPCONTEXT, 2, TextInsertLines[1]);
                                ((BC1103)mContext).Send(IPM.IPCONTEXT, 3, TextInsertLines[2]);
                                break;
                        }
                        if (mcTypeContext_2 != CameraType.Default || mcTypeContext_2 != CameraType.Not_Connected)
                        {
                            switch (mcTypeContext_2)
                            {
                                case CameraType.BC620:
                                    l.CType = CameraType.BC620;
                                    ((BC620)mContext_2).Send(mContext_2, IPM.IPCONTEXT_2, 2, TextInsertLinesOld[1]);
                                    ((BC620)mContext_2).Send(mContext_2, IPM.IPCONTEXT_2, 3, TextInsertLinesOld[2]);
                                    break;
                                case CameraType.BC840:
                                    l.CType = CameraType.BC840;
                                    ((BC840)mContext_2).Send(mContext_2, IPM.IPCONTEXT_2, 2, TextInsertLinesOld[1]);
                                    ((BC840)mContext_2).Send(mContext_2, IPM.IPCONTEXT_2, 3, TextInsertLinesOld[2]);
                                    break;
                                case CameraType.BC1103:
                                    l.CType = CameraType.BC1103;
                                    ((BC1103)mContext_2).Send(IPM.IPCONTEXT_2, 2, TextInsertLines[1]);
                                    ((BC1103)mContext_2).Send(IPM.IPCONTEXT_2, 3, TextInsertLines[2]);
                                    break;
                            }
                        }
                        switch (mcTypeDomo)
                        {
                            case CameraType.IFD820:
                                l.CType = CameraType.IFD820;
                                //Removed to avoid blinking
                                //((IFD820)mDomo).Send(mDomo, IPM.IPDOME, 2, OpenLane, TextInsertLinesOld[1]);
                                ((IFD820)mDomo).Send(mDomo, IPM.IPDOME, 3, OpenLane, TextInsertLinesOld[2]);
                                break;
                            case CameraType.FD1103:
                                ((FD1103)mDomo).Send(IPM.IPDOME, 2, TextInsertLines[1]);
                                ((FD1103)mDomo).Send(IPM.IPDOME, 3, TextInsertLines[2]);
                                break;
                        }
                        if (mcTypeDomo_2 != CameraType.Default || mcTypeDomo_2 != CameraType.Not_Connected)
                        {
                            switch (mcTypeDomo_2)
                            {
                                case CameraType.IFD820:
                                    //Removed to avoid blinking
                                    //((IFD820)mDomo_2).Send(mDomo_2, IPM.IPDOME_2, 2, OpenLane, TextInsertLinesOld[1]);
                                    ((IFD820)mDomo_2).Send(mDomo_2, IPM.IPDOME_2, 3, OpenLane, TextInsertLinesOld[2]);
                                    break;
                                case CameraType.FD1103:
                                    ((FD1103)mDomo_2).Send(IPM.IPDOME_2, 2, TextInsertLines[1]);
                                    ((FD1103)mDomo_2).Send(IPM.IPDOME_2, 3, TextInsertLines[2]);
                                    break;
                            }
                        }
                    }
                    else
                    {
                        switch (mcTypeContext)
                        {
                            case CameraType.BC620:
                                l.CType = CameraType.BC620;
                                ((BC620)mContext).Send(mContext, IPM.IPCONTEXT, 2, TextInsertLinesOld[1]);
                                Camera.HTTPRequest(((BC620)mContext).BuildCommand(IPM.IPCONTEXT, BC620.CGICommands.TextAndDate, 3, ""), NChttp, "GET");
                                break;
                            case CameraType.BC840:
                                l.CType = CameraType.BC840;
                                ((BC840)mContext).Send(mContext, IPM.IPCONTEXT, 2, TextInsertLinesOld[1]);
                                Camera.HTTPRequest(((BC840)mContext).BuildCommand(IPM.IPCONTEXT, BC840.CGICommands.TextAndDate, 3, ""), NChttp, "GET");
                                break;
                            case CameraType.BC1103:
                                ((BC1103)mContext).Send(IPM.IPCONTEXT, 2, TextInsertLines[1]);
                                ((BC1103)mContext).Send(IPM.IPCONTEXT, 3, "");
                                break;
                        }
                        if (mcTypeContext_2 != CameraType.Default || mcTypeContext_2 != CameraType.Not_Connected)
                        {
                            switch (mcTypeContext_2)
                            {
                                case CameraType.BC620:
                                    l.CType = CameraType.BC620;
                                    ((BC620)mContext_2).Send(mContext_2, IPM.IPCONTEXT_2, 2, TextInsertLinesOld[1]);
                                    Camera.HTTPRequest(((BC620)mContext_2).BuildCommand(IPM.IPCONTEXT_2, BC620.CGICommands.TextAndDate, 3, ""), NChttp, "GET");
                                    break;
                                case CameraType.BC840:
                                    l.CType = CameraType.BC840;
                                    ((BC840)mContext_2).Send(mContext_2, IPM.IPCONTEXT_2, 2, TextInsertLinesOld[1]);
                                    Camera.HTTPRequest(((BC840)mContext_2).BuildCommand(IPM.IPCONTEXT_2, BC840.CGICommands.TextAndDate, 3, ""), NChttp, "GET");
                                    break;
                                case CameraType.BC1103:
                                    l.CType = CameraType.BC1103;
                                    ((BC1103)mContext_2).Send(IPM.IPCONTEXT_2, 2, TextInsertLines[1]);
                                    ((BC1103)mContext_2).Send(IPM.IPCONTEXT_2, 3, "");
                                    break;
                            }
                        }
                        switch (mcTypeDomo)
                        {
                            case CameraType.IFD820:
                                l.CType = CameraType.IFD820;
                                ((IFD820)mDomo).Send(mDomo, IPM.IPDOME, 2, OpenLane, TextInsertLinesOld[1]);
                                //((IFD820)mDomo).BuildCommand(IPM.IPDOME, IFD820.CGICommands.Date, 3, ((IFD820)mDomo).ExtractClock(TextInsertLinesOld[0]).Item2);                            
                                break;
                            case CameraType.FD1103:
                                ((FD1103)mDomo).Send(IPM.IPDOME, 2, TextInsertLines[1]);
                                ((FD1103)mDomo).Send(IPM.IPDOME, 3, "");
                                break;
                        }
                        if (mcTypeDomo_2 != CameraType.Default || mcTypeDomo_2 != CameraType.Not_Connected)
                        {
                            switch (mcTypeDomo_2)
                            {
                                case CameraType.IFD820:                               
                                    ((IFD820)mDomo_2).Send(mDomo_2, IPM.IPDOME_2, 2, OpenLane, TextInsertLinesOld[1]);
                                    //((IFD820)mDomo_2).BuildCommand(IPM.IPDOME_2, IFD820.CGICommands.Date, 3, ((IFD820)mDomo).ExtractClock(TextInsertLinesOld[0]).Item2);
                                    break;
                                case CameraType.FD1103:
                                    ((FD1103)mDomo_2).Send(IPM.IPDOME_2, 2, TextInsertLines[1]);
                                    ((FD1103)mDomo_2).Send(IPM.IPDOME_2, 3, "");
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorMessages(ex);
            }
        }


        public static String[] EscapeS(String[] x)
        {          
            String[] y= {"","",""};
            y[1] = Camera.StripSpaces(x[1]);
            for (int i = 0; i < x.Length; i++)
            {
                y[i] = Uri.EscapeUriString(x[i]);
            }  
            return y;
        }
        public static String TrimWhiteSpaces(String x)
        {
            x = Uri.UnescapeDataString(x);
            x = Regex.Replace(x, @"\s+", " ");
            x = Uri.EscapeUriString(x);
            return x;
        }
        public static Logger L
        {
            get { return l; }
            set { l = value; }
        }
        public static CameraType CTypeContext
        {
            get { return mcTypeContext; }
            set { mcTypeContext = value; }
        }
        public static CameraType CTypeContext_2
        {
            get { return mcTypeContext_2; }
            set { mcTypeContext_2 = value; }
        }
        public static CameraType CTypeDomo
        {
            get { return mcTypeDomo; }
            set { mcTypeDomo = value; }
        }
        public static CameraType CTypeDomo_2
        {
            get { return mcTypeDomo_2; }
            set { mcTypeDomo_2 = value; }
        }
    }
}
