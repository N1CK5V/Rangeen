using System;

namespace Rangeen.Task
{
    public class TaskResult
    {
        /// <summary>
        /// Data with result
        /// </summary>
        public byte[] BinData { get; private set; }

        /// <summary>
        /// Task which have this result
        /// </summary>
        public TaskInfo Task { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public TaskResult(byte[] binData, TaskInfo task)
        {
            BinData = binData;
            Task = task;
        }
    }
}