using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace CivilightEterna
{
    public class HdiffHelper
    {
        public Action<string> Output { get; set; }

        /// <summary>
        /// 生成 HDiff 补丁
        /// </summary>
        public async Task CreatePatchAsync(
            string oldPath,
            string newPath,
            string patchPath,
            int memoryMB = 256,
            bool useCompress = true,
            string compressMethod = "zstd",
            string hashMethod = "xxh3",
            string otherargs = "-f -D")
        {
            await Task.Run(() =>
            {
                try
                {
                    var args = useCompress ? $" -c-{compressMethod}" : "";      // 压缩
                    args += $" -m-{memoryMB}";              // 内存限制
                    args += $" -C-{hashMethod} {otherargs} \"{oldPath}\" \"{newPath}\" \"{patchPath}\"";

                    var psi = new ProcessStartInfo
                    {
                        FileName = "hdiffz.exe",
                        Arguments = args,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        StandardOutputEncoding = System.Text.Encoding.UTF8,
                        StandardErrorEncoding = System.Text.Encoding.UTF8,
                        CreateNoWindow = true
                    };
                    Output?.Invoke($"CreatePatchAsync hdiffz {args}");
                    using (var proc = Process.Start(psi)){

                        proc.OutputDataReceived += (s, e) =>
                        {
                            Output?.Invoke(e.Data);
                        };
                        var stdoutTask = ReadStreamFull(proc.StandardOutput, s => Output?.Invoke(s));
                        var stderrTask = ReadStreamFull(proc.StandardError, s => Output?.Invoke($"[ERR] {s}"));

                        proc.WaitForExit();
                        Task.WaitAll(stdoutTask, stderrTask); 

                        //proc.BeginOutputReadLine();
                        //proc.WaitForExit();

                        if (proc.ExitCode != 0)
                            throw new Exception($"<code>: {proc.ExitCode}");
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception($"生成差分文件失败\n{ex.Message}", ex);
                }
            });
        }

        /// <summary>
        /// 应用 HDiff 补丁
        /// </summary>
        public async Task ApplyPatchAsync(
            string oldPath,
            string patchPath,
            string outPath,
            int memoryMB = 256,
            string otherargs = "-f")
        {
            await Task.Run(() =>
            {
                try
                {
                    var args = $" -s-{memoryMB}";
                    args += $" {otherargs} \"{oldPath}\" \"{patchPath}\" \"{outPath}\"";

                    var psi = new ProcessStartInfo
                    {
                        FileName = "hpatchz.exe",
                        Arguments = args,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        StandardOutputEncoding = System.Text.Encoding.UTF8,
                        StandardErrorEncoding = System.Text.Encoding.UTF8,
                        CreateNoWindow = true
                    };
                    Output?.Invoke($"ApplyPatchAsync hpatchz {args}");
                    using (var proc = Process.Start(psi))
                    {

                        proc.OutputDataReceived += (s, e) =>
                        {
                            Output?.Invoke(e.Data);
                        };
                        var stdoutTask = ReadStreamFull(proc.StandardOutput, s => Output?.Invoke(s));
                        var stderrTask = ReadStreamFull(proc.StandardError, s => Output?.Invoke($"[ERR] {s}"));


                        proc.WaitForExit();
                        Task.WaitAll(stdoutTask, stderrTask); 

                        if (proc.ExitCode != 0)
                            throw new Exception($"<code>: {proc.ExitCode}");
                    }
                    
                }
                catch (Exception ex)
                {
                    throw new Exception($"复原文件失败\n{ex.Message}", ex);
                }
            });
        }
        /// <summary>
        /// 校验 HDiff 补丁
        /// </summary>
        public async Task VerifyPatchAsync(
            string oldPath,
            string patchPath,
            string outPath,
            int memoryMB = 256)
        {
            await Task.Run(() =>
            {
                try
                {
                    var args = $" -s-{memoryMB}";
                    args += $" -f -t \"{oldPath}\" \"{outPath}\" \"{patchPath}\"";

                    var psi = new ProcessStartInfo
                    {
                        FileName = "hdiffz.exe",
                        Arguments = args,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        StandardOutputEncoding = System.Text.Encoding.UTF8,
                        StandardErrorEncoding = System.Text.Encoding.UTF8,
                        CreateNoWindow = true
                    };
                    Output?.Invoke($"VerifyPatchAsync hdiffz {args}");
                    using (var proc = Process.Start(psi))
                    {

                        proc.OutputDataReceived += (s, e) =>
                        {
                            Output?.Invoke(e.Data);
                        };
                        var stdoutTask = ReadStreamFull(proc.StandardOutput, s => Output?.Invoke(s));
                        var stderrTask = ReadStreamFull(proc.StandardError, s => Output?.Invoke($"[ERR] {s}"));


                        proc.WaitForExit();
                        Task.WaitAll(stdoutTask, stderrTask); 

                        if (proc.ExitCode != 0)
                            throw new Exception($"<code>: {proc.ExitCode}");
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception($"校验文件失败\n{ex.Message}", ex);
                }
            });
        }

        /// <summary>
        /// 补丁信息
        /// </summary>
        public void PatchInfo(string patchPath)
        {
            try
            {
                string args = " -v -info"; 
                args += $" \"{patchPath}\"";

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "hdiffz.exe",
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    StandardOutputEncoding = System.Text.Encoding.UTF8,
                    StandardErrorEncoding = System.Text.Encoding.UTF8,
                    CreateNoWindow = true
                };
                Output?.Invoke($"PatchInfo hdiffz {args}\n");
                using (var proc = Process.Start(psi))
                {

                    var stdoutTask = ReadStreamFull(proc.StandardOutput, s => Output?.Invoke(s));
                    var stderrTask = ReadStreamFull(proc.StandardError, s => Output?.Invoke($"[ERR] {s}"));

                    proc.WaitForExit();
                    Task.WaitAll(stdoutTask, stderrTask); 

                    //proc.OutputDataReceived += (s, e) =>
                    //{
                    //    Output?.Invoke(e.Data);
                    //};

                    //proc.BeginOutputReadLine();
                    //proc.WaitForExit();

                    if (proc.ExitCode != 0)
                        throw new Exception($"<code>: {proc.ExitCode}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"校验文件失败\n{ex.Message}", ex);
            }
        }
        private async Task ReadStreamFull(StreamReader reader, Action<string> onLine)
        {
            char[] buffer = new char[4096];
            int read;

            while ((read = await reader.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                string part = new string(buffer, 0, read);
                onLine(part); 
            }
        }
    }
}
