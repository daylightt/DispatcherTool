﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication1.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DispatcherDBEntities : DbContext
    {
        public DispatcherDBEntities()
            : base("name=DispatcherDBEntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Customer> Customers { get; set; }
        public DbSet<DailyHour> DailyHours { get; set; }
        public DbSet<Engineer> Engineers { get; set; }
        public DbSet<EngineerSkill> EngineerSkills { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<TaskCategory> TaskCategories { get; set; }
        public DbSet<WeeklyHour> WeeklyHours { get; set; }
    }
}
