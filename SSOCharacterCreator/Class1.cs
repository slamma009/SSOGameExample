using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using SSOClient;
using SSOClient.Responses;
using SSOClient.StandardTools;

class EmployeeTCPClient
{

    private static UserAccount userAccount;
    private static Client client = new Client();

    public static void Main(string[] args)
    {
        client.Connect();
        Login();


        Console.ReadKey();
    }
    private static void CreateAccount()
    {
        UserAccount account = new UserAccount();
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
            {7, "Wisdom" }
        };
        while (creating)
        {

            Console.Clear();
            Console.WriteLine("~~~CHARACTER CREATION~~~");
            Console.WriteLine();
            Console.WriteLine("Points Left: " + (account.PointBuy - PointSystem.CalculateTotalPoitnsForAccount(account)));
            Console.WriteLine();
            Console.WriteLine("1: Username: " + (option == 1 ? "" : account.Username));
            Console.WriteLine();
            Console.WriteLine("2: STR: " + (option == 2 ? "" : account.Strength.ToString()));
            Console.WriteLine("3: Dexterity: " + (option == 3 ? "" : account.Dexterity.ToString()));
            Console.WriteLine("4: Constitution: " + (option == 4 ? "" : account.Constitution.ToString()));
            Console.WriteLine("5: Charisma: " + (option == 5 ? "" : account.Charisma.ToString()));
            Console.WriteLine("6: Inteligence: " + (option == 6 ? "" : account.Inteligence.ToString()));
            Console.WriteLine("7: Wisdom: " + (option == 7 ? "" : account.Wisdom.ToString()));
            Console.WriteLine();
            Console.WriteLine("0: Submit to SSO Server.");
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

                if (option > 7 || option < 0)
                {
                    option = -1;
                    Error = "Choose a valid Option";
                }
            }
            else if (option == 1)
            {
                Console.Write("Choose a username: ");
                account.Username = Console.ReadLine();
                option = -1;
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
                            account.Strength = score;
                            break;
                        case 3:
                            account.Dexterity = score;
                            break;
                        case 4:
                            account.Constitution = score;
                            break;
                        case 5:
                            account.Charisma = score;
                            break;
                        case 6:
                            account.Inteligence = score;
                            break;
                        case 7:
                            account.Wisdom = score;
                            break;
                    }
                    option = -1;
                }
            }
            else if (option == 0)
            {
                if (!PointSystem.IsValidAccount(account))
                {
                    Error = "Account is not valid.";
                    option = -1;
                }
                else
                {
                    BaseResponse response = client.Create(account);
                    if (response.Status == 200)
                        creating = false;
                    else
                    {
                        Error = response.Text;
                        option = -1;
                    }

                }
            }
        }

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
                Console.WriteLine("Logged in: ");
                loggedIn = true;
                userAccount = response.AccountInformation;
                userAccount.WriteStats();
            }
            else
            {
                Console.WriteLine(response.Text);
            }
        }
    }
}