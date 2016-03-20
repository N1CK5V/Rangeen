using System;
using System.IO;
using System.Threading;
using UtilsLib;

namespace TaskExample
{
    public class TaskCore
    {
        private Action<byte[], object> _resultAcceptor;

        public void Execute(string[] argsString, Action<byte[], object> resultAcceptor, object taskInfo)
        {
            _resultAcceptor = resultAcceptor;

            Console.WriteLine("[TaskExample] runned. Args Len: {0}", argsString.Length);
            for (int i = 0; i < argsString.Length; i++)
                Console.WriteLine("\t[TaskExample] Arg #{0}: {1}", i, argsString[i]);
            Console.WriteLine("[TaskExample] sleep 7 sec");
            Thread.Sleep(7000);

            try
            {
                Console.WriteLine("[TaskExample] filename: {0}, content: {1}", argsString[0], argsString[1]);
                var stwr = new StreamWriter(File.Create(argsString[0]));
                stwr.Write(argsString[1]);
                stwr.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                throw;
            }

            _resultAcceptor.Invoke("! some message !".GetBytes(), taskInfo);
        }

        public void Cancel()
        {

        }

    }
}
