using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MutrajimAPI.Models
{
    public class MutrajimDbContext:DbContext
    {
        public MutrajimDbContext(DbContextOptions<MutrajimDbContext> options) : base(options)
        {

        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<TranslationModel> Translations { get; set; }
    }
}
