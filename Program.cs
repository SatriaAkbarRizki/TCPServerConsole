using TcpServer;

using Microsoft.Extensions.Configuration;



class Program
{
    public static IConfiguration? Configuration {get; set;}
    static void Main()
    {
        

        var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        Configuration = builder.Build();
        string connString = Configuration.GetConnectionString("DefaultConnection")!;

        var tcpserver = new TcpServerTest(connString!);
        tcpserver.start();
    }
}