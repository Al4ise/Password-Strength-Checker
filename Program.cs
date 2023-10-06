using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace PasswordChecker{
    class Program{
        static void Main(string[] args){
            Console.Write("Enter Password: ");
            string password = Console.ReadLine();
            if (password == ""){
                Console.WriteLine("No password entered");
                Environment.Exit(0);
            }
            // specify wordlist
            Console.Write("Directory of Wordlist: ");
            string wldir = Console.ReadLine();
            int score = 0;
            bool upper_exists = false;
            bool lower_exists = false;
            bool number_exists = false;
            char[] numbers = {'1', '2', '3', '4', '5', '6', '7', '8', '9', '0'};
            char[] letters = {'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i',
                            'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r',
                            's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

            List<string> suggestions = new List<string>();

            // checking wordlist with one password per line
            if (File.Exists(wldir)){
                Console.WriteLine("Checking if password is in wordlist...");
                string[] wordlist = File.ReadAllLines(wldir);
                foreach (string word in wordlist){
                    if (password == word){
                        printScore(0);
                        Console.WriteLine("Critical: Password is in wordlist");
                        Environment.Exit(0);
                    }
                }
            }
            else{
                Console.WriteLine("Wordlist not found, skipping...");
            }

            // if contains lowercase, upercase and numbers, add point for all the characters except the specials
            if (password.Any(char.IsNumber) && password.Any(char.IsLower) && password.Any(char.IsUpper)){
                
                int add = 1;

                // punish consecutive or repeatingnumbers
                for (int n=0; n<password.Length; n++){
                    if (char.IsNumber(password[n])){
                        if (anyConsecutive(numbers, password, n) || checkEqual(password, n)){
                            add -= 1;
                        }
                            score+=add;
                            number_exists = true;
                    }
                }

                add = 1;

                // punish consecutive or repeating letters
                for (int n=0; n<password.Length; n++){
                    if (anyConsecutive(letters, password, n) || checkEqual(password, n)){
                        add -= 1;
                    }

                    if (char.IsLower(password[n])){
                        score+=add;
                        lower_exists = true;
                    }
                }

                add = 1;

                // punish consecutive or repeating letters
                for (int n=0; n<password.Length; n++){
                    if (anyConsecutive(letters, password, n) || checkEqual(password, n)){
                        add -= 1;
                    }

                    if (char.IsUpper(password[n])){
                        score+=add;
                        upper_exists = true;
                    }
                }
            }

            // adding suggestions
            if (!upper_exists){
                suggestions.Add("Critical: Add an Uppercase Letter");
            }
            if (!lower_exists){
                suggestions.Add("Critical: Add a Lowercase Letter");
            }
            if (!number_exists){
                suggestions.Add("Critical: Add a Number");
            }


            // check if contains special character
            char[] special = {'!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '_', '+', '=', '{', '}', '[', ']', '|', '\\', ':', ';', '"', '\'', '<', '>', ',', '.', '?', '/'};
            if (password.IndexOfAny(special) != -1){
                score+=3;
            }
            else{
                suggestions.Add("Suggestion: Add a Special Character");
            }

            // check if contains dictionary word
            if (password.Length <= 8){
                score=0;
                suggestions.Add("Password is less than 8 characters long");
            }

            // add one point for every character over 8 OR remove for every character under 8
            score+=password.Length-8;

            printScore(score);

            // Print suggestions
            foreach (string suggestion in suggestions){
                Console.WriteLine(suggestion);
            }
        }

        static bool anyConsecutive (char[] letters, string password, int n){  
            if(checkConsecutive(letters, password, n) || checkReverseConsecutive(letters, password, n)){
                return true;
            }
            else{
                return false;
            } 
        }
        static bool checkConsecutive (char[] letters, string password, int n){  
            if (n==password.Length-1){
                return false;
            }
            char p_low = Char.ToLower(password[n]);  
            int index = Array.IndexOf(letters, p_low);

            if (index+1 > letters.Length -1){
                return false;
            }
            
            char nextLetter = letters[index+1];

            if (Char.ToLower(password[n+1]).Equals(nextLetter)){
                return true;
            }
            else{
                return false;
            }
        }

        static bool checkReverseConsecutive (char[] letters, string password, int n){  
            if (n==password.Length-1){
                return false;
            }

            char p_low = Char.ToLower(password[n]);  
            int index = Array.IndexOf(letters, p_low);

            if (index-1 < 0){
                return false;
            }

            char prevLetter = letters[index-1];

            if (Char.ToLower(password[n+1]).Equals(prevLetter)){
                return true;
            }
            else{
                return false;
            }
        }

        static bool checkEqual(string password, int n){
            if (n==password.Length-1){
                return false;
            }
            if(password[n]==password[n+1]){
                return true;
            }
            else{
                return false;
            }
        }

        static void printScore(int score){
            if (score <= 0){
                Console.WriteLine("Password is weak");
            }
            else if (score <= 8){
                Console.WriteLine("Password is decent");
            }
            else if (score <= 12){
                Console.WriteLine("Password is strong");
            }
            else if (score <= 16){
                Console.WriteLine("Password is very strong");
            }
            else{
                Console.WriteLine("Password is extremely strong");
            }

            Console.WriteLine("Score: " + score);
        }
    }
}