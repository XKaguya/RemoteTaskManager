using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RemoteTaskManager.API
{
    public class Handler
    {
        public static async Task<string> GetProcessInfo()
        {
            var processes = (await Task.WhenAll(Process.GetProcesses()
                    .Select(async p => new
                    {
                        Pid = p.Id,
                        Name = p.ProcessName,
                        MemoryUsage = p.WorkingSet64 / (1024 * 1024),
                        IsResponding = p.Responding,
                        CpuUsage = await GetCpuUsageByPid(p.Id)
                    })))
                .ToList();
                            
            string msg = JsonConvert.SerializeObject(processes, Formatting.Indented);
            
            return msg;
        }

        private static async Task<double> GetCpuUsageByPid(int pid)
        {
            try
            {
                var startTime = DateTime.UtcNow;
                var startCpuUsage = Process.GetProcessById(pid).TotalProcessorTime;

                await Task.Delay(500);

                var endTime = DateTime.UtcNow;
                var endCpuUsage = Process.GetProcessById(pid).TotalProcessorTime;

                var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
                var totalMsPassed = (endTime - startTime).TotalMilliseconds;

                var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);

                return Math.Round(cpuUsageTotal * 100, 1);
            }
            catch (Exception ex)
            {
                /*Console.WriteLine($"Failed to get CPU usage for process with ID {pid}: {ex.Message}");*/
                return -1;
            }
        }
        
        public static bool KillProcess(int pid, bool superMacy)
        {
            if (superMacy)
            {
                try
                {
                    if (Environment.Is64BitOperatingSystem)
                    {
                        Process process = new Process();
                        process.StartInfo.FileName = "ntsd_x64.exe";
                        process.StartInfo.Arguments = $"-c q -p {pid}";
                        process.Start();
                    }
                    else
                    {
                        Process process = new Process();
                        process.StartInfo.FileName = "ntsd_x86.exe";
                        process.StartInfo.Arguments = $"-c q -p {pid}";
                        process.Start();
                    }
                    
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
            else
            {
                try
                {
                    Process process = new Process();
                    process.StartInfo.FileName = "taskkill";
                    process.StartInfo.Arguments = "/f /pid " + pid;
                    process.Start();
                    
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }   
    }
}