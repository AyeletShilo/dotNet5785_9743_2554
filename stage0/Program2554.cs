//Console.WriteLine("Hello, World!");

namespace Stage0
{
    partial class Program
    {
        private static void Main(string[] args)
        {
            Welcome2554();
            Welcome9743();
            Console.ReadKey();
        }

        static partial void Welcome9743();
        private static void Welcome2554()
        {
            Console.WriteLine("Enter your name: ");
            string name = Console.ReadLine();
            Console.WriteLine("{0}, welcome to my first console application", name);
        }
    }
}

