using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SocialBrothersCase;

namespace SocialBrothersCase.Data
{
    public class SocialBrothersCaseContext : DbContext
    {
        public SocialBrothersCaseContext (DbContextOptions<SocialBrothersCaseContext> options)
            : base(options)
        {
        }

        public DbSet<SocialBrothersCase.Address> Address { get; set; } = default!;
    }
}
