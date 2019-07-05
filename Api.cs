﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MaxRev.Servers.API;
using MaxRev.Servers.API.Extensions;
using MaxRev.Servers.Core.Route;
using MaxRev.Servers.Interfaces;
using MaxRev.Utils.Http;
using MaxRev.Utils.Methods;
using Microsoft.Extensions.DependencyInjection;

namespace Energy_Platform
{
    [RouteBase("/api")]
    public class Api : CoreApi
    {
        Dictionary<string, Dictionary<string, string>> _map { get; set; }

        #region Contexts
        Database Context => Services.GetRequiredService<Database>();
        //DeviceContext DeviceContext => Services.GetRequiredService<DeviceContext>();
        //RecordContext RecordContext => Services.GetRequiredService<RecordContext>();
        //PoolContext PoolContext => Services.GetRequiredService<PoolContext>();

        #endregion

        protected override void OnInitialized()
        {
            _map = ApiHelpers.GetStaticTypes();

            Builder.ContentType("application/json");

            Context.Database.EnsureCreated();

            PopulateDatabase().Wait();

            CreateDefaultHospital();

            PopulateRecords();
        }

        private void PopulateRecords()
        {
            if (Context.Records == null || !Context.Records.Any())
            {
                var f = new CsvParser();

                var records = f.FromFile("test.csv");

                Context.AddRange(records);
                Context.SaveChanges();
            }
        }

        private string DefaultPoolName = "Вся будівля";
        private void CreateDefaultHospital()
        {
            if (Context.Pools == null || !Context.Pools.Any())
            {
                var pp = Context.Add(new Pool {Name = DefaultPoolName});
                Context.SaveChanges();

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
                        Type = _map[v]["short"],
                        Key = "Загальний"
                    });
                }

                Context.SaveChanges();
            }
        }

        private async Task PopulateDatabase()
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

                var created = await Context.Database.EnsureCreatedAsync().ConfigureAwait(false);

                await Context.AddRangeAsync(entities).ConfigureAwait(false);
                await Context.SaveChangesAsync().ConfigureAwait(false);

                Console.WriteLine("Log populated => OK");
            }


        }

        [Route("/hospitals/{id}/data")]
        public object GetHospitalsById(int id)
        {
            var el = Context.Hospitals.FirstOrDefault(x => x.Id == id);
            if (el != default)
            {
                var pools = Context.Pools.Where(x => x.Name == DefaultPoolName);

                var devices = Context.Devices.Join(pools, x => x.PoolId, x => x.Id, (x, y) => x);

                return Context.Records.Join(devices, x => x.DeviceId, x => x.Id, (x, y) => x);
            }

            return default;
        }
        [Route("/hospitals/names-dataset")]
        public object GetHospitals()
        {
            return Context.Hospitals.AsQueryable().ToArray();
        }
        [Route("devices")]
        public object GetDevices()
        {
            var query = Get("pool");
            return Context.Devices.AsQueryable().ToArray();
        }

        [Route("types")]
        public object GetTypes()
        {
            return _map.ToResponseInfo(Builder);
        }

        #region Helpers

        /// <summary>
        /// Ensures query contains all keys
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        protected bool HasKeys(params string[] keys)
        {
            return keys.All(t => Info.Query.HasKey(t));
        }

        /// <summary>
        /// Query parameter value by name or default specified value
        /// </summary>
        /// <param name="paramName">parameter name</param>
        /// <param name="defaultValue">default value to return</param>
        /// <returns>parameter value</returns>
        protected string Get(string paramName, string defaultValue = default)
        {
            return HasKeys(paramName) ? Info.Query[paramName]?.Trim() ?? defaultValue : defaultValue;
        }

        /// <exception cref="T:System.InvalidOperationException">Failed to get value</exception>
        protected T Get<T>(string paramName, string defaultValue = default)
        {
            var u = Get(paramName, defaultValue);
            if (u == default) return default;
            try
            {
                return (T)Convert.ChangeType(u, typeof(T));
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to get value", ex);
            }
        }

        #endregion

    }
}