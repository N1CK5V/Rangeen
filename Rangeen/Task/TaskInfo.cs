using System;

namespace Rangeen.Task
{
    /// <summary>
    /// This class contains info about executing task
    /// </summary>
    public class TaskInfo
    {
        public TaskModuleInfo TaskModuleInfo { get; private set; }
        public string[] TaskArguments { get; private set; }
        public string TaskId { get; private set; }

        /// <summary>
        /// link to task name
        /// </summary>
        public String Name { get { return TaskModuleInfo.Name; } }

        /// <summary>
        /// Constructor
        /// </summary>
        public TaskInfo(TaskModuleInfo taskModuleInfo, string[] args)
        {
            TaskId = Guid.NewGuid().ToString();
            TaskModuleInfo = taskModuleInfo;
            TaskArguments = args;
        }
    }
}