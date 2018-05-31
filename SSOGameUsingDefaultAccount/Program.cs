using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using SSOClient;
using SSOClient.Responses;
using SSOClient.StandardTools;
namespace SSOGameUsingDefaultAccount
{
    class Program
    {
        private static UserAccount userAccount;
        private static Client client = new Client();

        public static void Main(string[] args)
        {
            client.Connect();
            Console.WriteLine("GAME WITH DEFAULT ACCOUNT");
            Login();


            Console.ReadKey();
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
}
