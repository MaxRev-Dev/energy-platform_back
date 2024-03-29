﻿using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Energy_Platform
{
    public class Database : DbContext
    {
        public Database(DbContextOptions<Database> options)
            : base(options)
        { }
        public DbSet<HospitalInfo> Hospitals { get; set; }
        public DbSet<Record> Records { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Pool> Pools { get; set; }

    }
 
    public class Record
    {
        [Key]
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public int DeviceId { get; set; }
        public string NameType { get; set; }
        public double Value { get; set; }
    }



    public class Device
    {
        [Key]
        public int Id { get; set; }
        public string Key { get; set; }
        public string Type { get; set; }
        public int PoolId { get; set; }
    }

    public class Pool
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }


    public class Hospital
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
    }

    public class HospitalInfo
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}