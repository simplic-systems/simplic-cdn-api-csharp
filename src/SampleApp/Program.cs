using Simplic.CDN.CSharp;
using System;
using System.IO;

namespace SampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Create a new connection
                using (var cdn = new Cdn("http://localhost:50121/api/v1-0/"))
                {
                    // Ping the server
                    Console.WriteLine("Ping: " + cdn.Ping());

                    // Try to connect
                    if (cdn.Connect("UnitTestUser", "UnitTestPassword"))
                    {
                        Console.WriteLine($"Connected with cdn service: {cdn.Url}");

                        using (var stream = new MemoryStream(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }))
                        {
                            // Write data
                            cdn.WriteData("sample.data", stream);
                        }

                        // Read data
                        var data = cdn.ReadData("sample.data");
                    }
                    else
                    {
                        Console.WriteLine($"Could not connect to: {cdn.Url}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something went wrong... ");

                Console.WriteLine(ex.Message);

                Exception _ex = ex.InnerException;

                // Read list of inner exception until no exception exists
                while (_ex != null)
                {
                    Console.WriteLine(_ex.Message);
                    _ex = _ex.InnerException;
                }
            }

            Console.ReadLine();
        }
    }
}