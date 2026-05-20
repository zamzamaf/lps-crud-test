class Program
{
    static void Main()
    {
        while (true)
        {
            Console.WriteLine("Pilih metode:");
            Console.WriteLine("1. Hello World");
            Console.WriteLine("2. Foreach One To N");
            Console.WriteLine("3. Reverse Word Order");
            Console.WriteLine("4. Reverse Words");
            Console.WriteLine("5. Check Palindrome");
            Console.WriteLine("0. Exit");

            int choice = int.Parse(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    Test.HelloWorld();
                    break;
                case 2:
                    Test.ForeachOneToN();
                    break;
                case 0:
                    return;
                default:
                    Console.WriteLine("Pilihan tidak valid.");
                    break;
            }

            Console.WriteLine();
        }
    }  
}

public class Test
{
    public static void HelloWorld()
    {
        string input = "hello world";

        // hapus spasi manual
        char[] chars = new char[input.Length];
        int length = 0;

        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] != ' ')
            {
                chars[length] = input[i];
                length++;
            }
        }

        char[] unique = new char[length];
        int[] count = new int[length];
        int uniqueCount = 0;

        // hitung karakter
        for (int i = 0; i < length; i++)
        {
            char current = chars[i];
            bool found = false;

            for (int j = 0; j < uniqueCount; j++)
            {
                if (unique[j] == current)
                {
                    count[j]++;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                unique[uniqueCount] = current;
                count[uniqueCount] = 1;
                uniqueCount++;
            }
        }

        // output
        for (int i = 0; i < uniqueCount; i++)
        {
            Console.WriteLine(unique[i] + " - " + count[i]);
        }
    }

    public static void FizzBuzz()
    {
        Console.Write("Masukkan nilai N: ");
        int N = int.Parse(Console.ReadLine());

        for (int i = 1; i <= N; i++)
        {
            if (i % 5 == 0 && i % 6 != 0)
            {
                Console.Write("IDIC ");
            }
            else if (i % 6 == 0 && i != 6)
            {
                Console.Write("LPS ");
            }
            else
            {
                Console.Write(i + " ");
            }
        }
    }
}