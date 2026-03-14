
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Security.Claims;

namespace SurveyBasket.Persistence;
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options , IHttpContextAccessor httpContextAccessor) :
    IdentityDbContext<ApplicationUser>(options)
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public DbSet<Poll> Polls { get; set;  }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var etnries = ChangeTracker.Entries<AuditableEntity>();
        foreach (var entityEntry in etnries)
        {
            var currentUserId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (entityEntry.State == EntityState.Added)
            {
                entityEntry.Property(e => e.CreatedById).CurrentValue = currentUserId!;
                entityEntry.Property(e => e.CreatedOn).CurrentValue = DateTime.UtcNow;
            }
            else if(entityEntry.State == EntityState.Modified)
            {
                entityEntry.Property(e => e.UpdatedById).CurrentValue = currentUserId;
                entityEntry.Property(e => e.UpdatedOn).CurrentValue = DateTime.UtcNow;
            }
        }


        return base.SaveChangesAsync(cancellationToken);
    }

}


/// public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
///  Old Version without IdentityDbContext
