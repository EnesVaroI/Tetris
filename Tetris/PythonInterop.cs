using System;
using System.Diagnostics;
using System.IO;

namespace Tetris
{
    public static class PythonInterop
    {
        private const string PythonInterpreterPath = "python";

        private static readonly string PythonScriptPath = $@"{Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + @"\fuzzy-logic-model.py"}";

        public static int InferRotation(int height, int type, int groundStateLeft, int groundStateRight)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = PythonInterpreterPath,
                Arguments = $"{PythonScriptPath} {height} {type} {groundStateLeft} {groundStateRight}",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process { StartInfo = psi })
            {
                process.Start();

                using (StreamWriter sw = process.StandardInput)
                {
                    if (sw.BaseStream.CanWrite)
                    {
                        sw.WriteLine(height);
                        sw.WriteLine(type);
                        sw.WriteLine(groundStateLeft);
                        sw.WriteLine(groundStateRight);
                    }
                }

                string output = process.StandardOutput.ReadToEnd();

                process.WaitForExit();

                if (output == "Total area is zero in defuzzification.\r\n")
                {
                    throw new ZeroAreaException("Total area is zero in defuzzification.");
                }

                return Convert.ToInt32(output);
            }
        }
    }

    public class ZeroAreaException : Exception
    {
        public ZeroAreaException(string message) : base(message)
        {
        }
    }
}