// See https://aka.ms/new-console-template for more information

using System.Globalization;
using System.Text.Json;
using AutoMapper;
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

            // IEnumerable<Computer>? computersFromSystemJson =
            //     System.Text.Json.JsonSerializer.Deserialize<IEnumerable<Computer>>(
            //         computersJosn,
            //         jsonSerializerOptions
            //     );
            //
            // if (computersFromSystemJson != null)
            //     foreach (Computer computer in computersFromSystemJson)
            //     {
            //         string sql =
            //             @"INSERT INTO TutorialAppSchema.Computer (
            //           Motherboard,
            //           HasWifi,
            //           HasLTE,
            //           ReleaseDate,
            //           Price,
            //           VideoCard
            //       ) VALUES ('"
            //             + EscapeSingleQuote(computer.Motherboard)
            //             + "','"
            //             + computer.HasWifi
            //             + "','"
            //             + computer.HasLTE
            //             + "','"
            //             + computer.ReleaseDate?.ToString("yyyy-MM-dd")
            //             + "','"
            //             + computer.Price.ToString("0.00", CultureInfo.InvariantCulture)
            //             + "','"
            //             + EscapeSingleQuote(computer.VideoCard)
            //             + "')";
            //         dapper.exucuteSql(sql);
            //     }
            //
            // IEnumerable<Computer>? computersFromNewTonSoft = JsonConvert.DeserializeObject<
            //     IEnumerable<Computer>
            // >(computersJosn);

            // Mapper
            string computersJson = File.ReadAllText("ComputersSnake.json");

            Mapper mapper = new Mapper(
                new MapperConfiguration(
                    (cfg) =>
                    {
                        cfg.CreateMap<ComputerSnake, Computer>()
                            .ForMember(
                                destination => destination.ComputerId,
                                options => options.MapFrom(source => source.computer_id)
                            )
                            .ForMember(
                                destination => destination.CPUCores,
                                options => options.MapFrom(source => source.cpu_cores)
                            )
                            .ForMember(
                                destination => destination.HasLTE,
                                options => options.MapFrom(source => source.has_lte)
                            )
                            .ForMember(
                                destination => destination.HasWifi,
                                options => options.MapFrom(source => source.has_wifi)
                            )
                            .ForMember(
                                destination => destination.Motherboard,
                                options => options.MapFrom(source => source.motherboard)
                            )
                            .ForMember(
                                destination => destination.VideoCard,
                                options => options.MapFrom(source => source.video_card)
                            )
                            .ForMember(
                                destination => destination.ReleaseDate,
                                options => options.MapFrom(source => source.release_date)
                            )
                            .ForMember(
                                destination => destination.Price,
                                options => options.MapFrom(source => source.price)
                            );
                    }
                )
            );

            IEnumerable<ComputerSnake>? computersSystem =
                System.Text.Json.JsonSerializer.Deserialize<IEnumerable<ComputerSnake>>(
                    computersJson
                );

            if (computersSystem != null)
            {
                IEnumerable<Computer> computerResult = mapper.Map<IEnumerable<Computer>>(
                    computersSystem
                );
                Console.WriteLine("Automapper Count: " + computerResult.Count());
            }
        }

        static string EscapeSingleQuote(string input)
        {
            string output = input.Replace("'", "''");

            return output;
        }
    }
}
