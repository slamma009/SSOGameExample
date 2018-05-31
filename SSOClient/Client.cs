using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SSOClient.Commands;
using SSOClient.Responses;

namespace SSOClient
{
    public class Client
    {
        TcpClient client;
        StreamReader streamReader;
        Stream stream;

        public void Connect() {
            client = new TcpClient("127.0.0.1", 2055);

            stream = client.GetStream();
            streamReader= new StreamReader(stream);
            Console.WriteLine(streamReader.ReadLine());

        }
        public void Stop()
        {
            stream.Close();
            client.Close();
        }

        public LoginResponse Login(string name)
        {
            StreamWriter sw = new StreamWriter(stream);
            sw.AutoFlush = true;
            LoginCommand command = new LoginCommand();
            command.command = SSOCommandsEnum.Login;
            command.Username = name;
            sw.WriteLine(JsonConvert.SerializeObject(command));

            string rawResponse = streamReader.ReadLine();
            LoginResponse response = new LoginResponse() { Status = 500, AccountInformation = null };
            try
            {
                BaseResponse baseResponse = JsonConvert.DeserializeObject<BaseResponse>(rawResponse);
                if (baseResponse.Status != 200)
                {
                    response.Status = baseResponse.Status;
                }
                else
                {
                    response = JsonConvert.DeserializeObject<LoginResponse>(rawResponse);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);

            }

            return response;
        }

        public BaseResponse Create(UserAccount account)
        {
            StreamWriter sw = new StreamWriter(stream);
            sw.AutoFlush = true;
            CreateCommand command = new CreateCommand();
            command.command = SSOCommandsEnum.Create;
            command.newAccount = account;
            sw.WriteLine(JsonConvert.SerializeObject(command));
            return JsonConvert.DeserializeObject<BaseResponse>(streamReader.ReadLine());
        }
    }
}
