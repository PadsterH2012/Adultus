﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class mysqldbEntities1 : DbContext
    {
        public mysqldbEntities1()
            : base("name=mysqldbEntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C__migrationhistory> C__migrationhistory { get; set; }
        public virtual DbSet<aspnetrole> aspnetroles { get; set; }
        public virtual DbSet<aspnetuserclaim> aspnetuserclaims { get; set; }
        public virtual DbSet<aspnetuserlogin> aspnetuserlogins { get; set; }
        public virtual DbSet<aspnetuser> aspnetusers { get; set; }
        public virtual DbSet<userinfo> userinfoes { get; set; }
        public virtual DbSet<userlist> userlists { get; set; }
        public virtual DbSet<role> roles { get; set; }
    }
}
