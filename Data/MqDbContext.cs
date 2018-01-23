using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Echeers.Mq.Models;

namespace Echeers.Mq.Data
{
    public class MqDbContext : IdentityDbContext<MqUser>
    {
        public MqDbContext(DbContextOptions<MqDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<App> Apps { get; set; }
    }
}
