using System;

namespace Audit;

public static class GenerateData
{
    private static readonly Random Rand = new(DateTime.Now.Second);
    private static string GenerateFullName() { return GenerateName() + " " + GenerateName() + " " + GenerateName(); }
    
    private static string GenerateName()
    {
        var len = Rand.Next(5) + 5;
        string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "z", "t", "v", "w", "x" };
        string[] vowels = { "a", "e", "i", "o", "u", "ae", "y" };
        var name = "";
        name += consonants[Rand.Next(consonants.Length)].ToUpper();
        name += vowels[Rand.Next(vowels.Length)];
        var b = 2;
        while (b < len)
        {
            name += consonants[Rand.Next(consonants.Length)];
            b++;
            name += vowels[Rand.Next(vowels.Length)];
            b++;
        }

        return name;
    }
}