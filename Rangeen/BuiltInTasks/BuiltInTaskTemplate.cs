using System;
using Rangeen.Transport;
using UtilsLib;

namespace Rangeen.BuiltInTasks
{
    /// <summary>
    /// BuiltInTaskTemplate description
    /// </summary>
    public class BuiltInTaskTemplate
    {
        private Action<byte[], object> _resultAcceptor;

        public void Execute(string[] args, Action<byte[], object> resultAcceptor, object taskInfo)
        {
            _resultAcceptor = resultAcceptor;

            Console.WriteLine("[BuiltInTaskTemplate] runned. Args Len: {0}", args.Length);
            for (int i = 0; i < args.Length; i++)
            {
                Console.WriteLine("\t[BuiltInTaskTemplate] Arg #{0}: {1}", i, args[i]);
            }


            var output = "BuiltInTaskTemplate end";
            _resultAcceptor.Invoke(output.GetBytes(), taskInfo);
        }

    }
}