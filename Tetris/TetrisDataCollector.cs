using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using static Tetris.PythonInterop;
using static Tetris.Tetromino;

namespace Tetris
{
    internal static class TetrisDataCollector
    {
        public static string filePath = $@"{Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + @"\Tetris Database.json"}";
        public static void StoreTetrisData(int height, Shape type, int[] groundState, int humanOutput)
        {
            int? fuzzyOutput;
            try
            {
                fuzzyOutput = InferRotation(height, (int)type, groundState[0], groundState[1]);
            }
            catch (ZeroAreaException)
            {
                fuzzyOutput = null;
            }

            List<TetrisDataEntry> data = LoadData();

            var entry = new TetrisDataEntry
            {
                Height = height,
                Type = type.ToString(),
                GroundState = groundState,
                Rotation = new Output
                {
                    HumanOutput = 90 * humanOutput,
                    FuzzyOutput = 90 * fuzzyOutput
                }
            };

            data.Add(entry);

            SaveData(data);
        }

        private static List<TetrisDataEntry> LoadData()
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<List<TetrisDataEntry>>(json) ?? new List<TetrisDataEntry>();
            }
            else
            {
                throw new FileNotFoundException("Tetris Database.json file not found.");
            }
        }

        private static void SaveData(List<TetrisDataEntry> data)
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
    }

    public class TetrisDataEntry
    {
        public int Height { get; set; }
        public string Type { get; set; }
        public int[] GroundState { get; set; }
        public Output Rotation { get; set; }
    }

    public class Output
    {
        public int HumanOutput { get; set; }
        public int? FuzzyOutput { get; set; }
    }
}