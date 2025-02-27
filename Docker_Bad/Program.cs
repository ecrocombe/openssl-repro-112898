using System.Threading.Tasks;

namespace Docker_Bad
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await Shared.Test.Run();
        }
    }
}
