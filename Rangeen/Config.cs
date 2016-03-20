using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Rangeen
{
    /// <summary>
    /// This class contains common info about Agent
    /// </summary>
    class Config
    {
        /// <summary>
        /// Unique identificator
        /// </summary>
        public string AgentId { get; private set; }

        /// <summary>
        /// Version
        /// </summary>
        public string Version { get; private set; }

        /// <summary>
        /// Interval between connections to server
        /// </summary>
        public int Interval { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Config()
        {
            AgentId = Guid.NewGuid().ToString();
            Version = Program.AgentVersion;
            Interval = 5000;
        }

        /// <summary>
        /// Represent config in json string
        /// </summary>
        public string ToJsonString()
        {
            var dict = new Dictionary<string, string>
            {
                {"AgentId", AgentId},
                {"Version", Version},
                {"Interval", Interval.ToString()}
            };

            var stream1 = new MemoryStream();
            var ser = new DataContractJsonSerializer(dict.GetType());
            ser.WriteObject(stream1, dict);
            stream1.Position = 0;
            var sr = new StreamReader(stream1);

            return sr.ReadToEnd();
        }
    }
}
