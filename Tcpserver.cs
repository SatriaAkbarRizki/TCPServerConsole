using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace TcpServer
{
    class TcpServerTest
    {

        private Repository repository = new Repository();

        private readonly string _connectionString; 
        public TcpServerTest(String connectionString)
        {
            _connectionString = connectionString;
        }

        public void start()
        {
            const int port = 8888; 
            var listener = new TcpListener(IPAddress.Any, port);

            Console.WriteLine($"Starting TCP Server on port {port}...");
            Console.WriteLine("Waiting for a connection...");
            Console.WriteLine("===================");
            listener.Start();


            int countClient = 0;
            while (true)
            {
                countClient++;
                var client = listener.AcceptTcpClient(); 
                Console.WriteLine($"Client {countClient} connected!");

                Thread clientThread = new Thread(() => HandleClient(client));
                clientThread.Start();

            }
        }

            private void HandleClient(TcpClient client)
            {
                using (client)
                {
                    var stream = client.GetStream();
                    var buffer = new byte[1024];
                    int bytesRead;

                    bool isLogging = false;

                    try
                    {
                        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            var jsonRequest = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            if (isLogging == false){
                            
                                User userData = JsonSerializer.Deserialize<User>(jsonRequest);
                                AuthEmail auth = new AuthEmail(userData.Email, userData.Password, _connectionString);
                                
                                string jsonResponse = auth.checkValidate();
                                var jsonData = JsonSerializer.Deserialize<ServerResponse>(jsonResponse);
                                var responseServer = Encoding.UTF8.GetBytes(jsonResponse);

                                if (jsonData.Status == "200")
                                {
                                    isLogging = true;
                                    stream.Write(responseServer, 0, responseServer.Length);
                                }
                                else stream.Write(responseServer, 0, responseServer.Length);
                                

                            }else{
                            repository.insert(jsonRequest);
                            Console.WriteLine(jsonRequest);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Terjadi kesalahan pada client {client.Client.RemoteEndPoint}: {ex.Message}");
                    }

                    Console.WriteLine($"Client {client.Client.RemoteEndPoint} terputus.");
                }
            }
    }
}