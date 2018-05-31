using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using Newtonsoft.Json;
using SSOClient;
using SSOClient.Responses;
using SSOClient.StandardTools;
using SSOGameExample;

class GameScaled
{
    private static GameSpecificAccount account;
    private static Client client = new Client();
    private static List<GameSpecificAccount> allAccounts = new List<GameSpecificAccount>();

    public static void Main(string[] args)
    {
        LoadUsers();
        client.Connect();
        Login();


        account.WriteStats();

        Console.ReadKey();

    }

    private static void LoadUsers()
    {
        string text = File.ReadAllText(@"UserAccounts.json");
        allAccounts = JsonConvert.DeserializeObject<List<GameSpecificAccount>>(text);

    }
    private static void SaveAllUsers()
    {
        File.WriteAllText(@"UserAccounts.json", JsonConvert.SerializeObject(allAccounts));
    }

    private static void Login()
    {
        bool loggedIn = false;

        while (!loggedIn)
        {
            Console.WriteLine("Please Provide a username: ");
            LoginResponse response = client.Login(Console.ReadLine());
            if (response.Status == 200)
            {
                Console.Clear();
                loggedIn = true;
                var userAccount = response.AccountInformation;

                account = allAccounts.Where(x => x.Username == response.AccountInformation.Username).FirstOrDefault();
                if(account != null)
                {
                    account.SetBaseAccount(response.AccountInformation);
                    Console.WriteLine("Successfully logged in.\n\n");
                }
                else
                {
                    int choice = 0;
                    string Error = "";
                    while (choice == 0)
                    {
                        Console.Clear();
                        Console.WriteLine("First time log in. \n\n This game doesn't use the Wisdom modifier. Do you wish to start with fresh stats, or modify your existing ones?");
                        
                        Console.WriteLine("\n1: Fresh Stats\n\n2: Modify Existing");
                        Console.WriteLine();
                        if (string.IsNullOrEmpty(Error))
                        {
                            Console.WriteLine();
                        } else
                        {
                            Console.WriteLine(Error);
                            Error = "";
                        }
                        Console.WriteLine();

                        string option = Console.ReadLine().ToString();
                        int.TryParse(option, out choice);

                        if (choice > 2 || choice < 0)
                            choice = 0;
                        if(choice == 0)
                        {
                            Error = "Please choose a valid option.";
                        }

                    }
                    GameSpecificAccount newAccount;
                    newAccount = new GameSpecificAccount(userAccount);
                    if(choice == 1)
                    {
                        newAccount.StrMod = 10 - newAccount.Strength;
                        newAccount.DexMod = 10 - newAccount.Dexterity;
                        newAccount.ConMod = 10 - newAccount.Constitution;
                        newAccount.ChaMod = 10 - newAccount.Charisma;
                        newAccount.IntMod = 10 - newAccount.Inteligence;
                    }
                    CreateAccount(newAccount);
                }
            } else
            {
                Console.WriteLine(response.Text);
            }
        }


    }
    private static void CreateAccount(GameSpecificAccount newAccount)
    {
        int option = -1;
        bool creating = true;
        string Error = "";
        Dictionary<int, string> optionsToStat = new Dictionary<int, string>()
        {
            {2, "Strength" },
            {3, "Dexterity" },
            {4, "Constitution" },
            {5, "Charisma" },
            {6, "Inteligence" },
        };
        while (creating)
        {

            Console.Clear();
            Console.WriteLine("~~~CHARACTER CREATION~~~");
            Console.WriteLine();
            Console.WriteLine("Points Left: " + (newAccount.PointBuy - CalculateTotalPoitnsForAccount(newAccount)));
            Console.WriteLine();
            Console.WriteLine("2: STR: " + (option == 2 ? "" : newAccount.Strength.ToString()));
            Console.WriteLine("3: Dexterity: " + (option == 3 ? "" : newAccount.Dexterity.ToString()));
            Console.WriteLine("4: Constitution: " + (option == 4 ? "" : newAccount.Constitution.ToString()));
            Console.WriteLine("5: Charisma: " + (option == 5 ? "" : newAccount.Charisma.ToString()));
            Console.WriteLine("6: Inteligence: " + (option == 6 ? "" : newAccount.Inteligence.ToString()));
            Console.WriteLine();
            Console.WriteLine("0: Save Account");
            Console.WriteLine();
            if (string.IsNullOrEmpty(Error))
                Console.WriteLine();
            else
                Console.WriteLine("Error: " + Error);
            Console.WriteLine();

            Error = "";
            if (option == -1)
            {
                Console.Write("Choose an option: ");
                string rawOption = Console.ReadLine();
                try
                {
                    option = int.Parse(rawOption);
                }
                catch (Exception ex)
                {
                    Error = "Choose a valid Option";
                }

                if (option > 6 || option < 0 || option == 1)
                {
                    option = -1;
                    Error = "Choose a valid Option";
                }
            }
            else if (option > 1)
            {
                Console.Write("Choose a " + optionsToStat[option] + " score: ");
                int score = 0;
                int.TryParse(Console.ReadLine(), out score);
                if (PointSystem.CalculatePointCost(score) > 100)
                    Error = "Choose a score between 7 and 18";
                else
                {
                    switch (option)
                    {
                        case 2:
                            newAccount.StrMod = 0;
                            newAccount.StrMod = score - newAccount.Strength;
                            break;
                        case 3:
                            newAccount.DexMod = 0;
                            newAccount.DexMod = score - newAccount.Dexterity;
                            break;
                        case 4:
                            newAccount.ConMod = 0;
                            newAccount.ConMod = score - newAccount.Constitution;
                            break;
                        case 5:
                            newAccount.ChaMod = 0;
                            newAccount.ChaMod = score - newAccount.Charisma;
                            break;
                        case 6:
                            newAccount.IntMod = 0;
                            newAccount.IntMod = score - newAccount.Inteligence;
                            break;
                    }
                    option = -1;
                }
            }
            else if (option == 0)
            {
                if (CalculateTotalPoitnsForAccount(newAccount) != newAccount.PointBuy)
                {
                    Error = "Account is not valid.";
                    option = -1;
                }
                else
                {
                    allAccounts.Add(newAccount);
                    SaveAllUsers();
                    Console.Clear();
                    Console.WriteLine("Account made successfully!.\n\n");
                    account = newAccount;
                    creating = false;
                }
            }
        }

    }
    public static int CalculateTotalPoitnsForAccount(GameSpecificAccount account)
    {
        var points = 0;
        points += PointSystem.CalculatePointCost(account.Strength);
        points += PointSystem.CalculatePointCost(account.Dexterity);
        points += PointSystem.CalculatePointCost(account.Constitution);
        points += PointSystem.CalculatePointCost(account.Charisma);
        points += PointSystem.CalculatePointCost(account.Inteligence);

        return points;
    }
}