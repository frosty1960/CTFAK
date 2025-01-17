﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web.Caching;
using System.Windows.Forms;
using CTFAK.GUI;
using CTFAK.MMFParser.EXE;
using CTFAK.MMFParser.EXE.Loaders;
using CTFAK.MMFParser.EXE.Loaders.Banks;
using CTFAK.MMFParser.Translation;
using CTFAK.Utils;
using Joveler.Compression.ZLib;

namespace CTFAK
{
    public class Program
    {
        public static MainForm MyForm;
        public static GameData CleanData;

        public delegate void DumperEvent(object obj);


        [STAThread]
        private static void Main(string[] args)
        {
            //Kill the program remotely
            //using (var wc = new WebClient())
            //{
            //    var data = wc.DownloadString(@"https://ctfak.000webhostapp.com/FILE_CTFAK_CHECK");
            //    var fromBase64 = Convert.FromBase64String(data);
            //    for (int i = 0; i < fromBase64.Length; i++)
            //    {
            //        fromBase64[i] = (byte)(fromBase64[i] ^ 4);
            //    }

            //    var decryptedString = Encoding.ASCII.GetString(fromBase64);
            //    if (decryptedString == "CTFAKPASSGETECNRYPTEDCHECK") Logger.Log("Check passed, starting");
            //    else Environment.Exit(-69420);
            //}
            InitNativeLibrary();
            
            if (!File.Exists("settings.sav"))
            {
                File.Create("settings.sav");
            }
            // LoadableSettings.FromFile("settings.sav");
            //
            // MFAGenerator.WriteTestMFA();
            // MFAGenerator.ReadTestMFA();
            // Environment.Exit(0);
            Thread.CurrentThread.CurrentUICulture=new CultureInfo("en-US");
            AppDomain.CurrentDomain.FirstChanceException += (sender, eventArgs) =>
            {
                
                if (eventArgs.Exception is ThreadAbortException) return;
                var ex = (Exception) eventArgs.Exception;
                Logger.Log("ERROR: ");
                Logger.Log(ex.ToString());
            };
            // AppDomain.CurrentDomain.UnhandledException += (a,b) =>
            // {
                
                // var ex = (Exception)b.ExceptionObject;
                // if (ex is ThreadAbortException) return;
                // Logger.Log("ERROR: ");
                // Logger.Log(ex.ToString());
            // };
            
            Settings.UseGUI = true;


            if (args.Length > 0)
            {
                Console.WriteLine("CTFAK by 1987kostya, pete7201, and ClickNinYT");
                ImageBank.Load = false;
                ReadFile(args[0], true, false, true);
                //Console.ReadKey();
                
                //MFAGenerator.BuildMFA();
            }
            else if(args.Length==0)
            {
                Console.WriteLine("Usage: CTFAK.EXE: [full_path_to_game]");
                Console.WriteLine("Press any key to exit...");
                //Console.ReadKey();
                //MyForm = new MainForm(Color.FromArgb(223, 114, 38));
                //Application.Run(MyForm);
            }

            /*if (args.Length > 0 && (args[0] == "-h" || args[0] == "-help"))
            {
                Logger.Log("DotNetCTFDumper: 0.0.5", true, ConsoleColor.Green);
                Logger.Log("Launch Args:", true, ConsoleColor.Green);
                Logger.Log("   Filename - path to your exe or mfa", true, ConsoleColor.Green);
                Logger.Log("   Info - Dump debug info to console(default:true)", true, ConsoleColor.Green);
                Logger.Log("   DumpImages - Dump images to 'DUMP\\[your game]\\ImageBank'(default:false)", true,
                    ConsoleColor.Green);
                Logger.Log("   DumpSounds - Dump sounds to 'DUMP\\[your game]\\SoundBank'(default:true)\n", true,
                    ConsoleColor.Green);
                Logger.Log("Example: DotNetCTFDumper.exe E:\\SisterLocation.exe true true false true", true,
                    ConsoleColor.Green);
                Console.ReadKey();
                Environment.Exit(0);
            }

            if (args.Length > 1) ReadFile(path, verbose, dumpImages, dumpSounds);*/
            Console.WriteLine("CTFAK exiting...");
        }

        public static void ReadFile(string path, bool verbose = false, bool dumpImages = false, bool dumpSounds = true)
        {
            Settings.GamePath = path;
            PrepareFolders();

            Settings.DumpImages = dumpImages;
            Settings.DumpSounds = dumpSounds;
            Settings.Verbose = verbose;
            if (path.ToLower().EndsWith(".exe"))
            {
                var icon = new IconLoader(path);
                foreach (var ico in icon.GetAllIcons())
                {
                    if (File.Exists($"{Settings.IconPath}\\{ico.Width}x{ico.Height}.png")) continue;
                    ico.Save(new FileStream($"{Settings.IconPath}\\{ico.Width}x{ico.Height}.png",FileMode.CreateNew));
                }
                
                
                var exeReader = new ByteReader(path, FileMode.Open);
                var currentExe = new Exe();
                Exe.Instance = currentExe;
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                currentExe.ParseExe(exeReader);
                stopWatch.Stop();
                Logger.Log($"Game reading finished in {stopWatch.Elapsed.ToString("g")}, {exeReader.Size() - exeReader.Tell()} bytes left", true, ConsoleColor.Yellow);
                MFAGenerator.BuildMFA();
                Logger.Log($"Decompiling Finished", true, ConsoleColor.Green);

                var newWriter = new ByteWriter($"{Settings.DumpPath}\\repacked.exe", FileMode.Create);
            }
            else if (path.ToLower().EndsWith(".apk"))
            {
                var apk = new APK();
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                apk.ParseAPK(path);
                stopWatch.Stop();
                Logger.Log("Finished in "+stopWatch.Elapsed.ToString("g"), true, ConsoleColor.Yellow);
                
            }
            else if (path.ToLower().EndsWith(".ccn"))
            {
                Settings.GameType = GameType.Android;
                var data = new GameData();
                var dataReader = new ByteReader(path, FileMode.Open);
                CleanData = data;
                CleanData.Read(dataReader);
                MFAGenerator.BuildMFA();
            }
            else if (path.ToLower().EndsWith(".dat"))
            {
                Settings.GameType = GameType.NSwitch;
                var data = new GameData();
                var dataReader = new ByteReader(path, FileMode.Open);
                
                CleanData = data;
                CleanData.Read(dataReader);
                var newData = new ChunkList();
                newData.Read(dataReader);
            }
            
        }

        public static void PrepareFolders()
        {
            Directory.CreateDirectory($"{Settings.ImagePath}");
            Directory.CreateDirectory($"{Settings.SoundPath}");
            Directory.CreateDirectory($"{Settings.MusicPath}");
            Directory.CreateDirectory($"{Settings.ChunkPath}");
            Directory.CreateDirectory($"{Settings.ExtensionPath}");
            Directory.CreateDirectory($"{Settings.DLLPath}");
            Directory.CreateDirectory($"{Settings.EXEPath}");
            Directory.CreateDirectory($"{Settings.IconPath}");
            Directory.CreateDirectory($"{PluginAPI.PluginAPI.PluginPath}");
        }
        public static void InitNativeLibrary()
        {
            string arch = null;
            switch (RuntimeInformation.ProcessArchitecture)
            {
                case Architecture.X86:
                    arch = "x86";
                    break;
                case Architecture.X64:
                    arch = "x64";
                    break;
                case Architecture.Arm:
                    arch = "armhf";
                    break;
                case Architecture.Arm64:
                    arch = "arm64";
                    break;
            }
            string libPath = Path.Combine(arch, "zlibwapi.dll");

            if (!File.Exists(libPath))
                throw new PlatformNotSupportedException($"Unable to find native library [{libPath}].");

            ZLibInit.GlobalInit(libPath);
        }
    }
}