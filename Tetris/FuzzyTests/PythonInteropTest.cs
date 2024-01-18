using System;
using System.Diagnostics;
using System.IO;

public static class PythonInteropTest
{
    public static void Main()
    {
        int value = InferRotation(15, 2, 1, 1);

        Console.WriteLine(value);

        Console.ReadKey();
    }

    const string PythonInterpreterPath = "python";

    static readonly string PythonScriptPath = $@"{Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + @"\FuzzyTests\fuzzy-logic-model-test.py"}";

    static int InferRotation(int height, int type, int groundStateLeft, int groundStateRight)
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
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();

            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception($"Python script execution failed with error:\n{error}");
            }

            if (output == "Total area is zero in defuzzification.\r\n")
                return -1;

            return Convert.ToInt32(output);
        }
    }
}