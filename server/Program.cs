using System;

namespace Ion
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "Ion: Habbo Hotel server emulation environment";
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
            Console.Beep();

            IonEnvironment.Initialize();

            // Input loop
            while (true)
            {
                Console.ReadKey(true);
                IonEnvironment.Destroy();
            }
        }
    }
}
