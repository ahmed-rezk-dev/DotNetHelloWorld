namespace HelloWorld
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            Task firstTask = new Task(() =>
            {
                Thread.Sleep(100);
                Console.WriteLine("Task 1");
            });
            firstTask.Start();
            Console.WriteLine("After the task was created");
            await firstTask;
        }
    }
}
