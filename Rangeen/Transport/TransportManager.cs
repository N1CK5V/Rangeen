using System;
using System.Collections.Generic;
using System.Linq;
using UtilsLib;

namespace Rangeen.Transport
{
    public class TransportManager
    {
        /// <summary>
        /// List of available transports
        /// </summary>
        private readonly List<ITransport> _transports = new List<ITransport>();

        /// <summary>
        /// Number of current transport for send
        /// </summary>
        private int CurrentTransportNum
        {
            get
            {
                return _currentTransportNum;
            }
            set
            {
                _currentTransportNum = value > _transports.Count ? 0 : value;
            }
        }
        private int _currentTransportNum;

        public void AddTransport(ITransport transport)
        {
            if (transport != null)
                _transports.Add(transport);
        }

        private ITransport SelectTransport()
        {
            // TODO: SelectTransport()
            return _transports.Count > 0 ? _transports[_currentTransportNum] : null;
        }

        public List<byte[]> Download()
        {
            var allDownloaded = new List<byte[]>();
            try
            {
                // download data via all transports
                foreach (var downloaded in _transports.Select(transport => transport.Download()))
                    allDownloaded.AddRange(downloaded);

                // This LINQ dectypt all received data
                return allDownloaded.Select(entry => Cryptor.Decrypt(entry, Cryptor.DEBUG_key)).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine("[TransportManager] Download Exception: " + e.Message + "\n" + e.StackTrace);
                return null;
            }
        }

        public bool Upload(byte[] binData, byte[] info)
        {
            try
            {
                var transport = SelectTransport();
                var flag =  transport != null && transport.Upload(Cryptor.Crypt(binData, Cryptor.DEBUG_key), info);

                // if unsuccess
                if (!flag)
                    _currentTransportNum++;

                return flag;
            }
            catch (Exception e)
            {
                _currentTransportNum++;
                Console.WriteLine("[TransportManager] Upload Exception: " + e.Message + "\n" + e.StackTrace);
                return false;
            }
        }

    }
}