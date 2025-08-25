using System;
using System.Threading.Tasks;

namespace CRM.Integration.Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                await CRMIntegrationTest.RunTests();
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }
    }
}
