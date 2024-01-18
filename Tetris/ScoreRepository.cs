using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace Tetris
{
    public static class ScoreRepository
    {
        private static string connectionString = $@"
            Data Source={Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + @"\Score Database.db"};
            Version=3;";

        public static void InitializeDatabase()
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string sql = @"
                    CREATE TABLE IF NOT EXISTS Scores (
                        Name TEXT NOT NULL,
                        Score INTEGER NOT NULL
                    );";

                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void SaveScore(string playerName, int score)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string sql = $"INSERT INTO Scores (Name, Score) VALUES (@name, @score)";

                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@name", playerName);
                    command.Parameters.AddWithValue("@score", score);

                    command.ExecuteNonQuery();
                }
            }
        }
        
        public static List<(string Name, Int64 Score)> GetTopScores()
        {
            var topScores = new List<(string Name, Int64 Score)>();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string sql = @"
                    SELECT Name, Score
                    FROM Scores
                    ORDER BY Score DESC
                    LIMIT 12;";

                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string name = (string)reader["Name"];
                            Int64 score = (Int64)reader["Score"];

                            topScores.Add((name, score));
                        }
                    }
                }
            }

            return topScores;
        }
    }
}