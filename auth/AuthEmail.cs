using System.Data;
using System.Text.Json;
using Microsoft.VisualBasic;
using MySql.Data.MySqlClient;

class AuthEmail
{
    private readonly string Email;
    private readonly string Password;

    private readonly string _connectionString; 

    ServerResponse response = new ServerResponse();

    public AuthEmail(String email, String pass, String connectionString)
    {
        Email = email;
        Password = pass;
        _connectionString = connectionString;
    }


    public String checkValidate()
    {
        string query = "SELECT name, email, password FROM user WHERE email = @Email";

        using MySqlConnection connection = new MySqlConnection(_connectionString);
        try
        {
            using MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Email", Email);

            connection.Open();

            using MySqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                string hashedPasswordDB = reader["password"].ToString()!;
                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(Password, hashedPasswordDB);

                if (isPasswordValid)
                {
                    response.Status = "200";
                    response.Message = "Berhasil Login";
                    response.Data = new Dictionary<string, string>
                    {
                        { "name", reader["name"].ToString()! },
                        { "email", reader["email"].ToString()! }
                    };
                }
                else
                {
                    response.Status = "401";
                    response.Message = "password salah, berikan yang benar";
                }

            }
            else
            {
                    response.Status = "404";
                    response.Message = "Email tidak ditemukan";
            }
        }
        catch (Exception ex)
        {
            
            response.Status = "500";
            response.Message = $"Database Error: ${ex}";
        }

        String json = JsonSerializer.Serialize(response);
        return json;
    }
}