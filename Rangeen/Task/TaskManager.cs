using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UtilsLib;

namespace Rangeen.Task
{
    public class TaskManager
    {
        public List<TaskResult> Results { get; private set; }
        public List<TaskModuleInfo> TaskModules { get; private set; }
        public List<TaskInfo> RunningTasks { get; private set; }


        /// <summary>
        /// Constructor
        /// </summary>
        public TaskManager()
        {
            Results = new List<TaskResult>();
            TaskModules = new List<TaskModuleInfo>();
            RunningTasks = new List<TaskInfo>();
        }

        /// <summary>
        /// Start new task by TaskInfo
        /// </summary>
        public void StartNewTask(TaskInfo taskInfo)
        {
            var taskCore = taskInfo.TaskModuleInfo.TaskCore;   // get class
            var obj = Activator.CreateInstance(taskCore);      // create object of this class
            var executeMethod = taskCore.GetMethod("Execute"); // get method

            object[] args =                                    // create arguments array
            {
                taskInfo.TaskArguments,                        // arguments for task
                new Action<byte[], object>(ReceiveResult),     // delegate to result acceptor
                taskInfo                                       // info about this task
            };
            executeMethod.InvokeOnNewThread(obj, args);        // run task
        }

        /// <summary>
        /// Create TaskInfo and start new task
        /// </summary>
        public void StartNewTask(string tname, string[] targs)
        {
            // this LINQ exp finds taskModule from TaskModules list with name tname
            var tinfo = TaskModules.Where(module => module.Name == tname).Select(module => new TaskInfo(module, targs)).FirstOrDefault();
            if (tinfo != null)
                StartNewTask(tinfo);
        }

        /// <summary>
        /// split binData to discrete taskData.
        /// get taskname and args from taskData.
        /// start new task
        /// </summary>
        public void ParseTasks(List<byte[]> binList)
        {
            if (binList == null)
                return;

            foreach (var binData in binList)
            {
                if (binData == null || binData.Length == 2)
                    return;

                var result = binData.GetString();

                // TODO: make command format and remove "|some...separators|"

                var tasksStrings = result.Split(new[] { "|someTaskSeparator|" }, StringSplitOptions.None); // WARNING: Hardcoded TaskSeparator
                foreach (var taskString in tasksStrings)
                {
                    var taskFields = taskString.Split(new[] { "|someFieldSeparator|" }, StringSplitOptions.None); // WARNING: Hardcoded FieldSeparator
                    if (taskFields.Length < 2) // WARNING: Hardcoded taskFields.Length
                        continue;

                    var currName = taskFields[0];
                    var currArgs = taskFields[1].Split(new[] { "|someArgsSeparator|" }, StringSplitOptions.None); // WARNING: Hardcoded FieldSeparator

                    StartNewTask(currName, currArgs);
                }
            }
        }

        /// <summary>
        /// Load .dll with task
        /// </summary>
        public void AddNewTaskModule(string taskName)
        {
            try
            {
                if (taskName.EndsWith(".dll")) // Load from .dll
                {
                    string taskPath = Path.GetFullPath(taskName);
                    taskName = taskName.Substring(0, taskName.Length - 4);

                    var taskAssembly = Assembly.LoadFile(taskPath);
                    var taskCore = taskAssembly.GetType(taskName + ".TaskCore");
                    TaskModules.Add(new TaskModuleInfo(taskName, taskAssembly, taskCore));
                }
                else // load built-in Task 
                {
                    var taskCore = Type.GetType("Rangeen.BuiltInTasks." + taskName);
                    TaskModules.Add(new TaskModuleInfo(taskName, null, taskCore));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[TaskManager] AddNewTaskModule Exception: " + e.Message + "\n" + e.StackTrace);
            }

        }

        /// <summary>
        /// Callback for task threads
        /// </summary>
        public void ReceiveResult(byte[] binData, object taskInfo)
        {
            Results.Add(new TaskResult(binData, (TaskInfo)taskInfo));
        }

    }
}