// See https://aka.ms/new-console-template for more information

using System.Globalization;
using System.Text.Json;
using HelloWorld.Data;
using HelloWorld.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace HelloWorld
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            DataContxtDapper dapper = new DataContxtDapper(config);

            Computer myComputer = new Computer()
            {
                ComputerId = 0,
                Motherboard = "Z690",
                HasWifi = true,
                HasLTE = false,
                ReleaseDate = DateTime.Now,
                Price = 943.87m,
                VideoCard = "RTX 2060"
            };

            string computersJosn = File.ReadAllText("Computers.json");

            JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            IEnumerable<Computer>? computersFromSystemJson =
                System.Text.Json.JsonSerializer.Deserialize<IEnumerable<Computer>>(
                    computersJosn,
                    jsonSerializerOptions
                );

            if (computersFromSystemJson != null)
                foreach (Computer computer in computersFromSystemJson)
                {
                    string sql =
                        @"INSERT INTO TutorialAppSchema.Computer (
                      Motherboard,
                      HasWifi,
                      HasLTE,
                      ReleaseDate,
                      Price,
                      VideoCard
                  ) VALUES ('"
                        + EscapeSingleQuote(computer.Motherboard)
                        + "','"
                        + computer.HasWifi
                        + "','"
                        + computer.HasLTE
                        + "','"
                        + computer.ReleaseDate?.ToString("yyyy-MM-dd")
                        + "','"
                        + computer.Price.ToString("0.00", CultureInfo.InvariantCulture)
                        + "','"
                        + EscapeSingleQuote(computer.VideoCard)
                        + "')";
                    dapper.exucuteSql(sql);
                }

            IEnumerable<Computer>? computersFromNewTonSoft = JsonConvert.DeserializeObject<
                IEnumerable<Computer>
            >(computersJosn);

            // if (computersFromNewTonSoft != null)
            //     foreach (Computer computer in computersFromNewTonSoft)
            //     {
            //         Console.WriteLine(computer.Motherboard);
            //     }
        }

        static string EscapeSingleQuote(string input)
        {
            string output = input.Replace("'", "''");

            return output;
        }
    }
}
