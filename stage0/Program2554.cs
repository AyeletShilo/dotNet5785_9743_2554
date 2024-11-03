//Console.WriteLine("Hello, World!");

namespace Targil0

{
    partial class Program
    {
        private static void Main(string[] args)
        {
            welcome2554();
            welcome9743();
            Console.ReadKey();
        }

        static partial void welcome9743();
        private static void welcome2554()
        {
            Console.WriteLine("Enter your name: ");
            string name = Console.ReadLine();
            Console.WriteLine("{0}, welcome to my first console application", name);
        }
    }
}