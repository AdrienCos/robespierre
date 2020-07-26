using System;
using System.Collections.Generic;
using System.IO;

namespace robespierre
{
    class Program
    {

        static Dictionary<int, List<string>> LoadWords(string path, int minLen, int maxLen)
        {
            // Loads the given word list and remove all the ones shorter than minLen
            Dictionary<int, List<string>> wordsClean = new Dictionary<int, List<string>>();
            string[] words = File.ReadAllLines(path);
            int nbWords = words.Length;
            for (int i = 0; i < nbWords; i++)
            {
                if (words[i].Length >= minLen && words[i].Length <= maxLen)
                {
                    int wordlen = words[i].Length;
                    if (!wordsClean.ContainsKey(wordlen))
                    {
                        wordsClean.Add(wordlen, new List<string>());
                    }
                    wordsClean[wordlen].Add(words[i]);
                }
            }
            return wordsClean;
        }

        static int MaxKey(Dictionary<int, List<string>> dict)
        {
            int max = int.MinValue;
            foreach (int key in dict.Keys)
            {
                if (key > max)
                {
                    max = key;
                }
            }
            return max;
        }

        static int MaxValueLen(Dictionary<int, List<string>> dict)
        {
            // Returns the key of the longest value
            int indexMax = -1;
            int max = int.MinValue;
            foreach (int key in dict.Keys)
            {
                if (dict[key].Count > max)
                {
                    max = dict[key].Count;
                    indexMax = key;
                }
            }
            return indexMax;
        }

        static char GetNewLetter(List<char> usedLetters)
        {
            bool isValidLetter = false;
            char letter = '0';
            do
            {
                Console.Write("Choose a letter: ");
                string input = Console.ReadLine().ToLower();
                if (input.Length == 1)
                {
                    letter = input[0];
                    // Check if it is a new letter
                    if (!char.IsLetter(letter))
                    {
                        Console.WriteLine("Please use a letter");
                    }
                    else if (!usedLetters.Contains(letter))
                    {
                        isValidLetter = true;
                    }
                    else
                    {
                        Console.WriteLine("This letter has already been used");
                    }
                }
                else
                {
                    Console.WriteLine("Please use a single character");

                }
            } while (!isValidLetter);
            return letter;
        }

        static List<string> GetValidWords(List<string> words, char letter)
        {
            // Returns the list of words that do not contain the given letter
            List<string> validWords = new List<string>();
            foreach (string word in words)
            {
                if (!word.Contains(letter))
                {
                    validWords.Add(word);
                }
            }
            return validWords;
        }

        static List<string> GetBestWords(List<string> words, char letter, out int longestIndex)
        {
            // Build a dict of index bitmap -> wordList
            Dictionary<int, List<string>> wordLists = new Dictionary<int, List<string>>();
            foreach (string word in words)
            {
                // Compute the bitmask
                int bitmap = 0;
                for (int i = 0; i < word.Length; i++)
                {
                    if (word[i] == letter)
                    {
                        bitmap ^= 1 << (word.Length - i - 1);
                    }
                }
                // Check if wordLists has an entry for it
                List<string> wordList;
                if (!wordLists.ContainsKey(bitmap))
                {
                    // Create the entry if needed
                    wordList = new List<string>();
                    wordLists[bitmap] = wordList;
                }
                // Insert the word into the entry
                wordList = wordLists[bitmap];
                wordList.Add(word);
            }
            // Return the longest list
            longestIndex = MaxValueLen(wordLists);
            return wordLists[longestIndex];
        }

        static void Main(string[] args)
        {
            // Constants
            const int minLen = 5;
            const int maxLen = 15;
            const int startLives = 10;
            const string filepath = "data/dict_en.txt";
            // Load the words
            Dictionary<int, List<string>> words = LoadWords(filepath, minLen, maxLen);
            // Pick a word length
            Random rd = new Random();
            int wordLen = rd.Next(minLen, maxLen + 1);
            // Build the list of valid words
            List<string> validWords = words[wordLen];
            // While the game is not over
            int playerLives = startLives;
            int guessedLetters = 0;
            bool isOver = false;
            List<char> usedLetters = new List<char>();
            char[] currentGuess = new char[wordLen];
            for (int i = 0; i < wordLen; i++)
            {
                currentGuess[i] = '_';
            }
            do
            {
                // Print the blank word
                Console.WriteLine(new String('#', 15));
                Console.WriteLine($"Lives remaining: {playerLives}");
                Console.WriteLine($"{wordLen} letters: {string.Join(String.Empty, currentGuess)}");
                Console.WriteLine($"Used Letters: {string.Join(' ', usedLetters)}");
                // Ask the player for a letter
                char letter = GetNewLetter(usedLetters);
                usedLetters.Add(letter);
                usedLetters.Sort();
                // See if there is a set of words that do not contain that letter
                List<string> newValidWords = GetValidWords(validWords, letter);
                if (newValidWords.Count == 0)
                {
                    // There is no word without this letter
                    // Find the index for which there is the most letters
                    int bitmap;
                    validWords = GetBestWords(validWords, letter, out bitmap);
                    // Reveal the letter
                    for (int i = 0; i < wordLen; i++)
                    {
                        if ((bitmap & (1 << (wordLen - i - 1))) != 0)
                        {
                            currentGuess[i] = letter;
                            guessedLetters++;
                        }
                    }
                    // Check if the word has been guessed
                    if (guessedLetters == wordLen)
                    {
                        Console.WriteLine("Congratulations, you win!");
                        Console.WriteLine($"Solution: {string.Join(String.Empty, currentGuess)}");
                        isOver = true;
                    }
                }
                else
                {
                    // There are some words without this letter
                    validWords = newValidWords;
                    playerLives--;
                    // Check if the player lost
                    if (playerLives == 0)
                    {
                        Console.WriteLine("Sorry, you lost!");
                        Console.WriteLine($"Solution: {validWords[rd.Next(validWords.Count)]}");
                        isOver = true;
                    }
                }
            } while (!isOver);
        }
    }
}
