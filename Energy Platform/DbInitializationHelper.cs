using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Energy_Platform
{
    public class DbInitializationHelper
    {
        public Dictionary<string, Dictionary<string, string>> Map { get; }
        private readonly Database Context;

        public DbInitializationHelper(Database context)
        {
            Context = context;
            Map = ApiHelpers.GetStaticTypes();
        }

        public string DefaultPoolName = "Вся будівля";


        public async Task SetupDatabaseAsync()
        {
            int s = 1;
            while (true)
            {
                try
                {
                    Context.Database.EnsureCreated();
                }
                catch (SqlException)
                {
                    Console.WriteLine($"Trying to reconnect to DB in {s} sec");
                    await Task.Delay(s * 1000);
                    s *= 5;
                    if (s > 125)
                        throw new ApplicationException("Failed to connect to DB");
                    continue;
                }

                break;
            }

            await PopulateDatabaseAsync().ConfigureAwait(false);

            await CreateDefaultHospitalAsync().ConfigureAwait(false);

            await PopulateRecordsAsync().ConfigureAwait(false);

            Debug.WriteLine("Database is ready");
        }

        private async Task PopulateRecordsAsync()
        {
            if (Context.Records == null || !Context.Records.Any())
            {
                var f = new CsvParser();

                var records = f.FromFile("test.csv");

                await Context.AddRangeAsync(records).ConfigureAwait(false);
                await Context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        private async Task CreateDefaultHospitalAsync()
        {
            if (Context.Pools == null || !Context.Pools.Any())
            {
                var pp = Context.Add(new Pool { Name = DefaultPoolName });

                foreach (var v in new[]
                {
                    "water_mc",
                    "elec_kv",
                    "gas_mc",
                    "heat_gc"
                })
                {
                    Context.Add(new Device
                    {
                        PoolId = pp.Entity.Id,
                        Type = Map[v]["short"],
                        Key = "Загальний"
                    });
                }

                await Context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        private async Task PopulateDatabaseAsync()
        {
            if (Context.Hospitals == null || !Context.Hospitals.Any())
            {
                var f2 = new HospitalsProvider();
                var coll = f2.GetFromFile("clinics_score.geojson");

                var entities = coll.Features.Select(x => new HospitalInfo
                {
                    Name = x.Properties["name_service"],
                    Score = (int)x.Properties["score"],
                    Latitude = x.Properties["latitude"],
                    Longitude = x.Properties["longitude"]
                });

                await Context.Database.EnsureCreatedAsync().ConfigureAwait(false);

                await Context.AddRangeAsync(entities).ConfigureAwait(false);
                await Context.SaveChangesAsync().ConfigureAwait(false);

                Debug.WriteLine("Log populated => OK");
            }


        }
    }
}