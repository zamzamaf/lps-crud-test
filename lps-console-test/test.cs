using System.Text;

namespace Latihan1;

class Test
{
    internal static void ReverseString(string str)
    {

        char[] charArray = str.ToCharArray();
        for (int i = 0, j = str.Length - 1; i < j; i++, j--)
        {
            charArray[i] = str[j];
            charArray[j] = str[i];
        }
        string reversedstring = new string(charArray);
        Console.WriteLine(reversedstring);
    }

    internal static void chkPalindrome(string str)
    {
        bool flag = false;
        for (int i = 0, j = str.Length - 1; i < str.Length / 2; i++, j--)
        {
            if (str[i] != str[j])
            {
                flag = false;
                break;
            }
            else
                flag = true;
        }
        if (flag)
        {
            Console.WriteLine("Palindrome");
        }
        else
            Console.WriteLine("Not Palindrome");
    }

    internal static void ReverseWordOrder(string str)
    {
        int i;
        StringBuilder reverseSentence = new StringBuilder();

        int Start = str.Length - 1;
        int End = str.Length - 1;

        while (Start > 0)
        {
            if (str[Start] == ' ')
            {
                i = Start + 1;
                while (i <= End)
                {
                    reverseSentence.Append(str[i]);
                    i++;
                }
                reverseSentence.Append(' ');
                End = Start - 1;
            }
            Start--;
        }

        for (i = 0; i <= End; i++)
        {
            reverseSentence.Append(str[i]);
        }
        Console.WriteLine(reverseSentence.ToString());
    }

    internal static void ReverseWords(string str)
    {
        StringBuilder output = new StringBuilder();
        List<char> charlist = new List<char>();

        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] == ' ' || i == str.Length - 1)
            {
                if (i == str.Length - 1)
                    charlist.Add(str[i]);
                for (int j = charlist.Count - 1; j >= 0; j--)
                    output.Append(charlist[j]);

                output.Append(' ');
                charlist = new List<char>();
            }
            else
                charlist.Add(str[i]);
        }
        Console.WriteLine(output.ToString());
    }

    internal static void Countcharacter(string str)
    {
        Dictionary<char, int> characterCount = new Dictionary<char, int>();

        foreach (var character in str)
        {
            if (character != ' ')
            {
                if (!characterCount.ContainsKey(character))
                {
                    characterCount.Add(character, 1);
                }
                else
                {
                    characterCount[character]++;
                }
            }

        }
        foreach (var character in characterCount)
        {
            Console.WriteLine("{0} - {1}", character.Key, character.Value);
        }
    }

    internal static void removeduplicate(string str)
    {
        string result = string.Empty;

        for (int i = 0; i < str.Length; i++)
        {
            if (!result.Contains(str[i]))
            {
                result += str[i];
            }
        }
        Console.WriteLine(result);
    }

    internal static void findallsubstring(string str)
    {
        for (int i = 0; i < str.Length; ++i)
        {
            StringBuilder subString = new StringBuilder(str.Length - i);
            for (int j = i; j < str.Length; ++j)
            {
                subString.Append(str[j]);
                Console.Write(subString + " ");
            }
        }
    }

    internal static void RotateLeft(int[] array)
    {
        int size = array.Length;
        int temp;
        for (int j = size - 1; j > 0; j--)
        {
            temp = array[size - 1];
            array[array.Length - 1] = array[j - 1];
            array[j - 1] = temp;
        }

        foreach (int num in array)
        {
            Console.Write(num + " ");
        }
    }

    internal static void RotateRight(int[] array)
    {
        int size = array.Length;
        int temp;
        for (int j = 0; j < size - 1; j++)
        {
            temp = array[0];
            array[0] = array[j + 1];
            array[j + 1] = temp;
        }
        foreach (int num in array)
        {
            Console.Write(num + " ");
        }
    }

    internal static bool FindPrime(int number)
    {
        if (number == 1) return false;
        if (number == 2) return true;
        if (number % 2 == 0) return false;

        var squareRoot = (int)Math.Floor(Math.Sqrt(number));

        for (int i = 3; i <= squareRoot; i += 2)
        {
            if (number % i == 0) return false;
        }

        return true;
    }
}