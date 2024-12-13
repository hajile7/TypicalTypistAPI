using System;
using System.Data.SqlClient;
class PopulateBigraphsProgram
{
    static void Main()
    {

        string connectionString = "Data Source=.;Initial Catalog=TypicalTypistDB; Integrated Security=SSPI;Encrypt=false;TrustServerCertificate=True;";

        // Read all words and their IDs to a list
        List<(int WordId, string Word)> words = new List<(int, string)>();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            SqlCommand selectCommand = new SqlCommand("SELECT WordId, Word FROM Words", connection);
            SqlDataReader reader = selectCommand.ExecuteReader();

            while (reader.Read())
            {
                int wordId = reader.GetInt32(0);
                string word = reader.GetString(1);
                words.Add((wordId, word));
            }

            reader.Close();
        }

        // Insert bigraphs after reading all words
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            foreach (var (wordId, word) in words)
            {
                // Process each word
                for (int i = 0; i < word.Length - 1; i++)
                {
                    string bigraph = word.Substring(i, 2);

                    // Insert each bigraph into the Bigraphs table
                    string insertBigraphQuery = "INSERT INTO Bigraphs (Bigraph, WordId) VALUES (@Bigraph, @WordId)";
                    using (SqlCommand insertCommand = new SqlCommand(insertBigraphQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@Bigraph", bigraph);
                        insertCommand.Parameters.AddWithValue("@WordId", wordId);
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }

            Console.WriteLine("Bigraphs table populated successfully!");
        }
    }
}