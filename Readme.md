# Robespierre

This is Robespierre, an adversary for the game Hangman than cheats and lies in order to guarantee its victory.

## Running Robespierre

* Install `.NET` on your machine.
* Clone and `cd` into this repo
* Run `dotnet run ` to start the game

## Configuring the difficulty

At this time, the ways to influence the difficulty are:

* Change the number of guesses in the `Main` function (defaults to `int startLives = 10`)
* Change the minimum and maximum length of the word the computer can choose in the `Main` function (defaults repectively to `int minLen = 5` and `int maxLen = 15`)
* Change the set of words used as the selection pool, either by replacing the `data/dict_en.txt` file, or changing the path in the main function (defaults to `string filepath = "data/dict_en.txt"`)


## How it works

When the game starts, the program first picks a random word length and creates the list of all fitting words from the given list. Whenever the player inputs a letter, the Robespierre checks if there is a subset of these words without this letter.

If there is such a subset, the letter is considered to be a miss, and the list of valid words is reduced to this set. Otherwise, it computes the largest set of words with the new letter at the same position, and reveals only these letters. This lets the program to constantly keep a list of words as large as possible, allowing it to evade as many guesses as possible.