using System;
using System.Reflection;

namespace Rangeen.Task
{
    /// <summary>
    /// This class contains info about .dll file
    /// </summary>
    public class TaskModuleInfo
    {
        public string Name { get; private set; }
        public Assembly TaskAssembly { get; private set; }
        public Type TaskCore { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public TaskModuleInfo(string name, Assembly taskAssembly, Type taskCore)
        {
            Name = name;
            TaskAssembly = taskAssembly;
            TaskCore = taskCore;
        }
    }
}