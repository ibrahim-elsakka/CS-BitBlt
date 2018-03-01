using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.IO.Pipes;
using System.Threading;

namespace PimpMan
{
    class PipeHandler
    {
        private static bool isgoodpipe = false;
        public static bool isscreenshot = false;
        public static void bgprocess()
        {
            while (true)
            {
                NamedPipeClientStream pipe = new NamedPipeClientStream(".", "ColtononPipe", PipeDirection.InOut);
                pipe.Connect();

                using (StreamReader rdr = new StreamReader(pipe, Encoding.Unicode))
                {
                    int code = 0;
                    try
                    {
                        code = int.Parse(rdr.ReadLine());
                    } catch (Exception) { }
                    if (code == 1)
                    {
                        isgoodpipe = true;
                    }
                    if (code == 2)
                    {
                        isscreenshot = true;
                    }
                    Thread.Sleep(50);
                }
                Thread.Sleep(50);
            }
        }
    }

    public class Pimp
    {

        public enum BlockMethod
        {
            None,
            Zero,
            Block
        };

        private BlockMethod bm = BlockMethod.None;
        /// <summary>
        /// Settings to use for the hooker.  Cannot be changed after initialization.
        /// </summary>
        /// <param name="blocking">The blocking method for screenshots.  None doesn't block, Zero sets the resolution to 1x1px, and Block returns NULL</param>
        /// <param name="popup">Shows a messagebox whenever a screenshot is hooked.</param>
        /// <param name="delaythread">Delays the thread 1000ms before blocking/returning the screenshot.  Give's overlays time to disable rendering.</param>
        public Pimp(BlockMethod blocking, bool popup, bool delaythread, bool allocconsole)
        {
            bm = blocking;


            string line = (allocconsole ? 1 : 0).ToString();
            line += (delaythread ? 1 : 0).ToString();
            if (blocking == BlockMethod.Block) { line +="1";} else { line+="0"; }
            line += (popup ? 1 : 0).ToString();
            if (blocking == BlockMethod.Zero) { line += "1"; } else { line += "0"; }
            string dir = Directory.GetCurrentDirectory() + "/cfgtmp.txt";
            if (File.Exists(dir)) File.Delete(dir);
            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(dir))
            {
                        file.Write(line);
            }

        }


        /// <summary>
        /// Returns a bool on whether the game is requesting a screenshot or not.  Note that after calling this, it'll be reset to false, so set it locally if you need it in multiple places.
        /// </summary>
        public bool IsScreenShot()
        {
            if (PipeHandler.isscreenshot)
            {
                PipeHandler.isscreenshot = false;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Injects a bitblt hooker into the below process
        /// </summary>
        /// <param name="Target">The process (not window name) to inject the hooker into</param>
        public bool Inject(string Target)
        {
            Process[] targetProcess = Process.GetProcessesByName(Target);
            if (targetProcess.Length == 0) return false;
            if (!File.Exists(Directory.GetCurrentDirectory() + "/Hooker.dll")) return false;
            //bj.Inject();
            Injector.doshit(Target);
           

            Thread BgThread;
            BgThread = new Thread(new
                ThreadStart(PipeHandler.bgprocess));
            BgThread.Start();
            return true;

        }
    }
}