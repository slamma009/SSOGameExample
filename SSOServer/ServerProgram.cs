using Newtonsoft.Json;
using SSOClient;
using SSOClient.Commands;
using SSOClient.Responses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace SSOServer
{
    class ServerProgram
    {
        static TcpListener listener;
        const int LIMIT = 5; //5 concurrent clients
        private static List<UserAccount> allUsers = new List<UserAccount>();
        static void Main(string[] args)
        {
            LoadUsers();
            listener = new TcpListener(2055);
            listener.Start();
            Console.WriteLine("Server mounted, listening to port 2055");

            for (int i = 0; i < LIMIT; i++)
            {
                Thread t = new Thread(new ThreadStart(Service));
                t.Start();
            }
            for (int i = 0; i < LIMIT; i++)
            {

                Thread t = new Thread(new ThreadStart(Service));
                t.Start();
            }
        }

        private static void LoadUsers()
        {
            string text = File.ReadAllText(@"UserAccounts.json");
            allUsers = JsonConvert.DeserializeObject<List<UserAccount>>(text);

        }
        private static void SaveAllUsers()
        {
            File.WriteAllText(@"UserAccounts.json", JsonConvert.SerializeObject(allUsers));
        }
        public static void Service()
        {
            while (true)
            {
                Socket soc = listener.AcceptSocket();
                //soc.SetSocketOption(SocketOptionLevel.Socket,

                Console.WriteLine("Connected: {0}", soc.RemoteEndPoint);

                try
                {
                    Stream s = new NetworkStream(soc);
                    StreamReader sr = new StreamReader(s);
                    StreamWriter sw = new StreamWriter(s);
                    sw.AutoFlush = true; // enable automatic flushing
                    sw.WriteLine("Connected");
                    bool run = true;

                    while (run)
                    {
                        string json = sr.ReadLine();
                        string response = JsonConvert.SerializeObject(new BaseResponse() { Status = 500 });
                        try
                        {
                            var command = JsonConvert.DeserializeObject<BaseCommand>(json);
                            switch (command.command)
                            {
                                case SSOCommandsEnum.Logout:
                                    response = JsonConvert.SerializeObject(new BaseResponse() { Status = 200 });
                                    run = false;
                                    break;

                                case SSOCommandsEnum.Login:
                                    response = JsonConvert.SerializeObject(Login(json));
                                    break;

                                case SSOCommandsEnum.Create:
                                    response = JsonConvert.SerializeObject(Create(json));
                                    break;

                                default:
                                    response = JsonConvert.SerializeObject(new BaseResponse() { Status = 404 });
                                    break;
                            }
                        }
                        finally
                        {
                            sw.WriteLine(response);
                        }
                    }
                    s.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                Console.WriteLine("Disconnected: {0}", soc.RemoteEndPoint);
                soc.Close();
            }
        }

        public static LoginResponse Login(string rawJson)
        {
            LoginCommand command = JsonConvert.DeserializeObject<LoginCommand>(rawJson);

            UserAccount account = allUsers.Where(x => x.Username == command.Username).FirstOrDefault();
            if (account == null)
            {
                return new LoginResponse() { Status = 404, Text = "User not found." };
            } else {
                return new LoginResponse()
                {
                    Status = 200,
                    AccountInformation = account
                };
            }

        }

        public static BaseResponse Create(string rawJson)
        {
            CreateCommand command = JsonConvert.DeserializeObject<CreateCommand>(rawJson);

            if (allUsers.Any(x => x.Username == command.newAccount.Username))
            {
                return new BaseResponse()
                {
                    Status = 500,
                    Text = "User already exists."
                };
            }
            else
            {
                allUsers.Add(command.newAccount);
                SaveAllUsers();

                return new BaseResponse() { Status = 200 };
            }
        }
    }
}
