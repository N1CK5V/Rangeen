using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using Microsoft.Win32;
//using Microsoft.Win32.TaskScheduler;

namespace Rangeen
{
    class Autorun
    {
        public static bool IsAdministrator()
        {
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent()))
                    .IsInRole(WindowsBuiltInRole.Administrator);
        }

        private bool IsStartupItem()
        {
            // The path to the key where Windows looks for startup applications
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (rkApp.GetValue("Rangeen") == null)
                // The value doesn't exist, the application is not set to run at startup
                return false;
            else
                // The value exists, the application is set to run at startup
                return true;
        }

        private void AddHKCU()
        {
            // The path to the key where Windows looks for startup applications
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (!IsStartupItem())
                // Add the value in the registry so that the application runs at startup
                rkApp.SetValue("Rangeen", System.Reflection.Assembly.GetEntryAssembly().Location);

        }

        private void DeleteHKCU()
        {
            // The path to the key where Windows looks for startup applications
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (IsStartupItem())
                // Remove the value from the registry so that the application doesn't start
                rkApp.DeleteValue("Rangeen", false);
        }

        private void AddHKLM()
        {
            // The path to the key where Windows looks for startup applications
            RegistryKey rkApp = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (!IsStartupItem())
                // Add the value in the registry so that the application runs at startup
                rkApp.SetValue("Rangeen", System.Reflection.Assembly.GetEntryAssembly().Location);

        }

        private void DeleteHKLM()
        {
            // The path to the key where Windows looks for startup applications
            RegistryKey rkApp = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (IsStartupItem())
                // Remove the value from the registry so that the application doesn't start
                rkApp.DeleteValue("Rangeen", false);
        }

        //private void AddTaskScheduler()
        //{
        //    // Get the service on the local machine
        //    using (TaskService ts = new TaskService())
        //    {
        //        // Create a new task definition and assign properties
        //        TaskDefinition td = ts.NewTask();
        //        td.RegistrationInfo.Description = "Boost up loading";

                
        //        td.Triggers.Add(new LogonTrigger());

        //        // Create an action that will launch Notepad whenever the trigger fires
        //        td.Actions.Add(new ExecAction(System.Reflection.Assembly.GetEntryAssembly().Location, null, null));

        //        // Register the task in the root folder
        //        ts.RootFolder.RegisterTaskDefinition(@"Rangeen", td);

                
        //    }
        //}

        //private void DeleteTaskScheduler()
        //{
        //    // Get the service on the local machine
        //    using (TaskService ts = new TaskService())
        //    {
        //        // Remove the task we just created
        //        ts.RootFolder.DeleteTask("Rangeen");
        //    }
        //}

        public void AddAutoRuns()
        {
            AddHKCU();
            if (IsAdministrator())
            {
                AddHKLM();
                //AddTaskScheduler();
            }
        }

        public void DeleteAutoRuns()
        {
            DeleteHKCU();
            if (IsAdministrator())
            {
                DeleteHKLM();
                //DeleteTaskScheduler();
            }

        }

    }
}
