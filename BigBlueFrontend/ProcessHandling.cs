using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Management;
using System.IO;

namespace BigBlue
{
    public static class ProcessHandling
    {
        public static void AddPathBinariesToProcessList(string path, List<string> processes)
        {
            IEnumerable<string> binaries = FileHandling.GetFilesByPath("exe", path);

            if (binaries != null)
            {
                foreach (string binary in binaries)
                {
                    string binaryName = Path.GetFileNameWithoutExtension(binary);

                    if (!processes.Contains(binaryName) && !binaryName.Equals("conhost", StringComparison.InvariantCultureIgnoreCase))
                    {
                        processes.Add(binaryName);
                    }
                }
            }
        }

        public static void GetChildProcesses(Process parentProcess, List<string> processes)
        {
            try
            {
                using (ManagementObjectSearcher mos = new ManagementObjectSearcher(string.Format("SELECT * FROM Win32_Process WHERE ParentProcessID={0}", parentProcess.Id)))
                {
                    using (ManagementObjectCollection results = mos.Get())
                    {
                        foreach (ManagementObject mo in results)
                        {
                            string processPath = mo["ExecutablePath"].ToString();
                            string processName = Path.GetFileNameWithoutExtension(processPath);

                            // you only want to get unique ones
                            if (!processes.Contains(processName) && !processName.Equals("conhost", StringComparison.InvariantCultureIgnoreCase))
                            {
                                processes.Add(processName);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public static bool KillAllProcessesByParentId(int parentProcessId)
        {
            bool success = false;

            string queryString = "SELECT * FROM Win32_Process WHERE ParentProcessId=" + parentProcessId;

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(queryString))
            {
                using (ManagementObjectCollection collection = searcher.Get())
                {
                    if (collection.Count > 0)
                    {
                        foreach (var item in collection)
                        {
                            int childProcessId = Convert.ToInt32((UInt32)item["ProcessId"]);

                            if (childProcessId != Process.GetCurrentProcess().Id)
                            {
                                KillAllProcessesByParentId(childProcessId);

                                Process childProcess = Process.GetProcessById(childProcessId);

                                if (!childProcess.CloseMainWindow())
                                {
                                    childProcess.Kill();
                                }

                                childProcess.Dispose();
                                success = true;
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            Process parent = Process.GetProcessById(parentProcessId);

                            if (parent != null)
                            {
                                if (!parent.CloseMainWindow())
                                {
                                    parent.Kill();
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }
                        finally
                        {
                            success = true;
                        }
                    }
                }
            }

            return success;
        }

        public static void CloseAllProcesses(List<string> processNamesOnDeathRow)
        {
            // if the list isn't null, proceed
            if (processNamesOnDeathRow != null)
            {
                // loop through the list backwards
                for (int i = (processNamesOnDeathRow.Count - 1); i >= 0; i--)
                {
                    // try to get the process by name
                    Process[] procs = Process.GetProcessesByName(processNamesOnDeathRow[i]);

                    // if we got something, close it
                    if (procs.Count() > 0)
                    {
                        foreach (Process p in procs)
                        {
                            p.Kill();
                        }
                    }
                    else
                    {
                        // if we got nothing, remove it from the list
                        processNamesOnDeathRow.RemoveAt(i);
                    }
                }

                // we need to wait until everything is closed
                while (processNamesOnDeathRow.Count > 0)
                {
                    // keep looping through and checking to see if it's null
                    for (int i = (processNamesOnDeathRow.Count - 1); i >= 0; i--)
                    {
                        Process[] procs = Process.GetProcessesByName(processNamesOnDeathRow[i]);

                        // if it's null, remove it from the list
                        if (procs.Count() == 0)
                        {
                            processNamesOnDeathRow.RemoveAt(i);
                        }
                    }
                }
            }
        }

        public static Process GetFirstChildProcess(int processId)
        {
            Process child = null;

            using (ManagementObjectSearcher mos = new ManagementObjectSearcher(String.Format("SELECT * FROM Win32_Process WHERE ParentProcessID={0}", processId)))
            {
                using (ManagementObjectCollection results = mos.Get())
                {
                    foreach (ManagementObject mo in results)
                    {
                        try
                        {
                            child = Process.GetProcessById(Convert.ToInt32(mo["ProcessID"]));
                            break;
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }

            return child;
        }
    }
}
