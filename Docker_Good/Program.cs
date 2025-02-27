namespace Docker_Good
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await Shared.Test.Run();
        }
    }
}
