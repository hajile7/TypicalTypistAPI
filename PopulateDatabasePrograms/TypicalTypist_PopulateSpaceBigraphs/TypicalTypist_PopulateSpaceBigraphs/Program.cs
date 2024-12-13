using System;
using System.Data.SqlClient;

class PopulateBigraphsProgram
{
    static void Main()
    {
        string connectionString = "Data Source=.;Initial Catalog=TypicalTypistDB; Integrated Security=SSPI;Encrypt=false;TrustServerCertificate=True;";

        // List of letters 'a' to 'z'
        char[] letters = "abcdefghijklmnopqrstuvwxyz".ToCharArray();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            // Insert space-related bigraphs
            foreach (char letter in letters)
            {
                // Insert leading space bigraph
                InsertBigraph(connection, $" {letter}", null);

                // Insert trailing space bigraph
                InsertBigraph(connection, $"{letter} ", null);
            }

            Console.WriteLine("52 possible space bigraphs inserted successfully!");
        }
    }

    static void InsertBigraph(SqlConnection connection, string bigraph, int? wordId)
    {
        string insertBigraphQuery = "INSERT INTO Bigraphs (Bigraph, WordId) VALUES (@Bigraph, @WordId)";
        using (SqlCommand insertCommand = new SqlCommand(insertBigraphQuery, connection))
        {
            insertCommand.Parameters.AddWithValue("@Bigraph", bigraph);
            insertCommand.Parameters.AddWithValue("@WordId", DBNull.Value); // WordId is null for space bigraphs
            insertCommand.ExecuteNonQuery();
        }
    }
}