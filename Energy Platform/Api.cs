using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MaxRev.Servers.API;
using MaxRev.Servers.API.Extensions;
using MaxRev.Servers.Core.Route;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Energy_Platform
{
    [RouteBase("/api")]
    public class Api : CoreApi
    {
        private Database Context { get; set; }
        private DbInitializationHelper DbInitializationHelper { get; set; }
        protected override void OnInitialized()
        {
            Builder.ContentType("application/json");
            Context = Services.GetRequiredService<Database>();
            DbInitializationHelper = Services.GetRequiredService<DbInitializationHelper>(); 
        }
        [Route("/hospitals/{id}/data")]
        public object GetHospitalsById(int id)
        {
            var el = Context.Hospitals.FirstOrDefault(x => x.Id == id);
            if (el != default)
            {
                var pools = Context.Pools.Where(x => x.Name == DbInitializationHelper.DefaultPoolName);

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
            return Context.Devices.AsQueryable().ToArray();
        }

        [Route("types")]
        public object GetTypes()
        {
            return DbInitializationHelper.Map.ToResponseInfo(Builder);
        }
    }
}