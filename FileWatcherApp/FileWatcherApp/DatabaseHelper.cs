using System.Data.SqlClient;
using System;

public class DatabaseHelper
{
    private string _connectionString;

    public DatabaseHelper(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void InsertFileChange(string operation, string fileName, long size, DateTime lastModified)
    {
        string insertCommand = "INSERT INTO FileTable (Operation, FileName, Size, LastModified) VALUES (@Operation, @FileName, @Size, @LastModified)";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(insertCommand, connection))
            {
                command.Parameters.AddWithValue("@Operation", operation);
                command.Parameters.AddWithValue("@FileName", fileName);
                command.Parameters.AddWithValue("@Size", size);
                command.Parameters.AddWithValue("@LastModified", lastModified);
                command.ExecuteNonQuery();
            }
        }
    } }