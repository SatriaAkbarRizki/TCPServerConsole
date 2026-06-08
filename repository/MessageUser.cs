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

    public List<MessageModel> ambilRiwayatPesan()
    {
        var listRiwayat = new List<MessageModel>();
        
        string query = "SELECT sender_name, message FROM messages ORDER BY created_at ASC";

        using var connection = new MySqlConnection(_connectionString);
        try
        {
            using var command = new MySqlCommand(query, connection);
            connection.Open();

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {

                var pesan = new MessageModel
                {
                    SenderName = reader["sender_name"].ToString()!,
                    MessageText = reader["message"].ToString()!
                };
                listRiwayat.Add(pesan);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DATABASE ERROR] Gagal mengambil riwayat: {ex.Message}");
        }

        return listRiwayat;
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