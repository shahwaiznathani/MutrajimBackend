using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MutrajimAPI.Models
{
    public class AuthenticationContext : IdentityDbContext
    {
        public AuthenticationContext(DbContextOptions options):base(options)
        {

        }
        public DbSet<LocaleSetting> LocaleSettings { get; set; }
        public DbSet<TranslationModel> Translations { get; set; }
        public DbSet<FileSetting> FileSettings { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
    }
}
