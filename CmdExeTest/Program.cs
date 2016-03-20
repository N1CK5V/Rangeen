using System;
using System.Diagnostics;
using System.Management;
using System.Threading;

namespace CmdExeTest
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("[TaskCmd] runned. Args Len: {0}", args.Length);
            for (int i = 0; i < args.Length; i++)
            {
                Console.WriteLine("\t[TaskCmd] Arg #{0}: {1}", i, args[i]);
            }

            var command = args[1];
            int time;
            if (!int.TryParse(args[2], out time))
                time = -1;
            int kill;
            if (!int.TryParse(args[3], out kill))
                kill = -1;
            int maxMem;
            if (!int.TryParse(args[4], out maxMem))
                maxMem = -1;
            string output;
            int procId = 0;

            try
            {
                var cmdStartInfo = new ProcessStartInfo("cmd", "/c " + command)
                {
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = args[0]
                };

                using (var cmdProcess = new Process())
                {
                    cmdProcess.StartInfo = cmdStartInfo;
                    cmdProcess.Start();

                    try
                    {
                        procId = cmdProcess.Id;
                    }

                    catch (InvalidOperationException) { }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                        throw;
                    }

                    if (maxMem != -1)
                    {
                        ThreadStart threadMain = delegate { SizeControl(maxMem, cmdProcess, procId); };
                        new Thread(threadMain).Start();
                    }


                    if (kill == 1 && time > 0)
                    {
                        if (!cmdProcess.WaitForExit(time))
                            KillProcessAndChildren(procId);

                        //cmdProcess.Kill();
                    }


                    output = cmdProcess.StandardOutput.ReadToEnd();

                    if (string.IsNullOrEmpty(output))
                        output = cmdProcess.StandardError.ReadToEnd();

                }

            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        static void SizeControl(int maxMem, Process cmdProcess, int procId)
        {
            try
            {
                while (!cmdProcess.HasExited)
                {
                    var totalBytesOfMemoryUsed = cmdProcess.WorkingSet64;
                    if (totalBytesOfMemoryUsed > maxMem)
                        KillProcessAndChildren(procId);
                    Thread.Sleep(2500);
                    cmdProcess.Refresh();
                }
                Thread.CurrentThread.Abort(); 
            }
            catch (Exception)
            {
                // Process already exited.
            }

        }

        private static void KillProcessAndChildren(int pid)
        {
            var searcher = new ManagementObjectSearcher
                ("Select * From Win32_Process Where ParentProcessID=" + pid);
            var moc = searcher.Get();
            foreach (var o in moc)
            {
                if (!o.GetType().IsAssignableFrom(typeof(ManagementObject)))
                    continue;
                var mo = (ManagementObject) o;
                KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));
            }
            try
            {
                var proc = Process.GetProcessById(pid);
                if (!proc.HasExited)
                    proc.Kill();
            }
            catch (Exception)
            {
                // Process already exited.
            }
        }

    }
}
