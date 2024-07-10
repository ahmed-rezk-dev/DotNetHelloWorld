// See https://aka.ms/new-console-template for more information

using System.Globalization;
using HelloWorld.Data;
using HelloWorld.Models;
using Microsoft.Extensions.Configuration;

namespace HelloWorld
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

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

            string dbInsertComputerString =
                @"INSERT INTO TutorialAppSchema.Computer (
                Motherboard,
                HasWifi,
                HasLTE,
                ReleaseDate,
                Price,
                VideoCard
            ) VALUES ('"
                + myComputer.Motherboard
                + "','"
                + myComputer.HasWifi
                + "','"
                + myComputer.HasLTE
                + "','"
                + myComputer.ReleaseDate.ToString("yyyy-MM-dd")
                + "','"
                + myComputer.Price.ToString("0.00", CultureInfo.InvariantCulture)
                + "','"
                + myComputer.VideoCard
                + "')";

            DataContxtDapper dapper = new DataContxtDapper(config);
            DataContextEF entityFramework = new DataContextEF(config);

            int result = dapper.exucuteSqlWithRowsCount(dbInsertComputerString);

            Console.WriteLine(result);

            // string myComputerSqlString = @"SELECT * FROM TutorialAppSchema.Computer";
            // IEnumerable<Computer> computers = dapper.LoadData<Computer>(myComputerSqlString);
            IEnumerable<Computer>? computers = entityFramework.Computer?.ToList<Computer>();

            if (computers != null)
                foreach (Computer computer in computers)
                {
                    Console.WriteLine(computer.ComputerId);
                }
        }
    }
}
