using System;
using System.Diagnostics;
using System.Management;
using System.Threading;
using UtilsLib;

//Params need to be:
//1)Path to be executed (do with CD so we can do this without ShellExecute,also control other disk path)
//2)Command to execute
//3)Time to timeout
//4)Kill or not to kill after
//5)Maximum memory usage of Started CMD
//6)PowerShell?
namespace Rangeen.BuiltInTasks
{
    public class CmdTask
    {
        private Action<byte[], object> _resultAcceptor;

        public void Execute(string[] argsString, Action<byte[], object> resultAcceptor, object taskInfo)
        {
            _resultAcceptor = resultAcceptor;

            Console.WriteLine("[TaskCmd] runned. Args Len: {0}", argsString.Length);
            for (int i = 0; i < argsString.Length; i++)
            {
                Console.WriteLine("\t[TaskCmd] Arg #{0}: {1}", i, argsString[i]);
            }

            int ps;
            string command;
            string openprocess;
            if (!int.TryParse(argsString[5], out ps))
                ps = -1;

            if (ps != 1)
            {
                command = "/c " + argsString[1];
                openprocess = "cmd";
            }
            else
            {
                command = argsString[1];
                openprocess = "powershell";
            }

            int time;
            if (!int.TryParse(argsString[2], out time))
                time = -1;
            int kill;
            if (!int.TryParse(argsString[3], out kill))
                kill = -1;
            int maxMem;
            if (!int.TryParse(argsString[4], out maxMem))
                maxMem = -1;
            string output;
            int procId = 0;

            try
            {
                var cmdStartInfo = new ProcessStartInfo(openprocess, command)
                {
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = argsString[0]
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

            _resultAcceptor.Invoke(output.GetBytes(), taskInfo);
        }

        private static void SizeControl(int maxMem, Process cmdProcess, int procId)
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
                var mo = (ManagementObject)o;
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

        public void Cancel()
        {

        }

    }
}