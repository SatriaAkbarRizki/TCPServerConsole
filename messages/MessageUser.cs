using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;

class MessageUser
{
    private String datamessage;

    private String nameSend;
    private String message;

    private readonly string _connectionString; 


    public MessageUser (String connectionString){
        _connectionString = connectionString;
    }


    public void insertMessage(String data)
    {
        parsingString(data);
        string query = "INSERT INTO messages (sender_name, message) VALUES (@Sender, @Message)";
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Sender", nameSend);
            command.Parameters.AddWithValue("@Message", message);

            connection.Open();
            command.ExecuteNonQuery(); 
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DATABASE ERROR] Gagal menyimpan pesan: {ex.Message}");
        }

    }


    public void parsingString(String input)
    {
        string pattern = @"^\[(.*?)\]:\s*(.*)$";

        int openBracket = input.IndexOf('[');
        int closeBracket = input.IndexOf(']');
        int colon = input.IndexOf(':');
        if (openBracket != -1 && closeBracket != -1 && colon != -1)
        {
     
             nameSend = input.Substring(openBracket + 1, closeBracket - openBracket - 1);
             message = input.Substring(colon + 1).Trim(); 
        }
    }
}