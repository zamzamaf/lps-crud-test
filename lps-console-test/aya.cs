namespace Latihan1;

class Program
{
    // static void Main(string[] args)
    // {
        // Test.ReverseString("Hello World");
        // Test.chkPalindrome("madam");
        // Test.ReverseWordOrder("Hello World from C#");
        // Program program = new Program();
        // Console.WriteLine("Enter a string to check if it's a palindrome:");
        // string? input = Console.ReadLine();
        // if (program.IsPalindrome(input))
        // {
        //     Console.WriteLine($"'{input}' is a palindrome.");
        // }
        // else
        // {
        //     Console.WriteLine($"'{input}' is not a palindrome.");
        // }

        // Console.WriteLine("Enter two numbers to swap:");
        // Console.Write("First number: ");
        // int firstNumber = int.Parse(Console.ReadLine());
        // Console.Write("Second number: ");
        // int secondNumber = int.Parse(Console.ReadLine());
        // program.SwapNumbers(ref firstNumber, ref secondNumber);
        // Console.WriteLine($"After swapping: First number = {firstNumber}, Second number = {secondNumber}");

        // Console.WriteLine("Enter a list of numbers separated by commas to find duplicates:");
        // string? numbersInput = Console.ReadLine();
        // int[] numbers = numbersInput.Split(',').Select(int.Parse).ToArray();
        // Console.WriteLine("Duplicate numbers:");
        // program.FindDuplicates(numbers);

        // Console.WriteLine("Enter a string to count character occurrences:");
        // string? charInput = Console.ReadLine();
        // program.CountCharacterOccurrences(charInput);

        // Console.WriteLine("Enter a sentence to reverse the words:");
        // string? sentenceInput = Console.ReadLine();
        // Console.WriteLine($"Reversed sentence: {program.ReverseWords(sentenceInput)}");

        // Console.WriteLine("Enter a number to calculate its factorial:");
        // int factorialInput = int.Parse(Console.ReadLine());
        // Console.WriteLine($"Factorial of {factorialInput} is: {program.Factorial(factorialInput)}");

        // Console.WriteLine("Enter a number to calculate its Fibonacci:");
        // int fibonacciInput = int.Parse(Console.ReadLine());
        // Console.WriteLine($"Fibonacci of {fibonacciInput} is: {program.Fibonacci(fibonacciInput)}");
    // }

    bool IsPalindrome(string input)
    {
        string reversed = new string(input.Reverse().ToArray());
        return input.Equals(reversed, StringComparison.OrdinalIgnoreCase);
    }

    void SwapNumbers(ref int a, ref int b)
    {
        a = a + b;
        b = a - b;
        a = a - b;
    }

    void FindDuplicates(int[] arr)
    {
        var duplicates = arr.GroupBy(x => x)
        .Where(g => g.Count() > 1)
        .Select(g => g.Key);
        Console.WriteLine(string.Join(", ", duplicates));
    }

    void CountCharacterOccurrences(string input)
    {
        var characterCount = input.GroupBy(c => c)
                                .ToDictionary(g => g.Key, g => g.Count());
        foreach (var kvp in characterCount)
        {
            Console.WriteLine($"{kvp.Key}: {kvp.Value}");
        }
    }

    string ReverseWords(string sentence)
    {
        return string.Join(" ", sentence.Split(' ').Reverse());
    }

    int Factorial(int n)
    {
        return n == 0 ? 1 : n * Factorial(n - 1);
    }

    int Fibonacci(int n)
    {
        return n <= 1 ? n : Fibonacci(n - 1) + Fibonacci(n - 2);
    }
}
