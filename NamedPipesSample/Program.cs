using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace NamedPipesSample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                await RunServer();
            }
            else
            {
                await RunClient(args[0]);
            }
        }

        private static async Task RunClient(string name)
        {
            using (var client = new NamedPipeClientStream(".", name, PipeDirection.Out))
            {
                await client.ConnectAsync(1000);

                using (var sw = new StreamWriter(client) { AutoFlush = true })
                {
                    await sw.WriteAsync("hello this is a message");
                }
            }
        }

        private static async Task RunServer()
        {
            var name = Guid.NewGuid().ToString();

            Console.WriteLine("Using pipe name: {0}", name);

            using (var server = new NamedPipeServerStream(name, PipeDirection.In))
            {
                await server.WaitForConnectionAsync();

                using (var sr = new StreamReader(server))
                {
                    var msg = await sr.ReadToEndAsync();
                    Console.WriteLine("Message: {0}", msg);
                }
            }
        }
    }
}
