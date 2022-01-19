using System.Data;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Travel.Application.Common.Interfaces;
using Travel.Application.TourLists.Queries.GetTours;
using Travel.Domain.Entities;

namespace Travel.Data.Contexts;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly IDateTime _dateTime;
    private IDbContextTransaction _currentTransaction;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDateTime dateTime) : base(options)
    {
        _dateTime = dateTime;
    }

    public DbSet<TourList> TourLists { get; set; }
    public DbSet<TourPackage> TourPackages { get; set; }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        return base.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync()
    {
        if (_currentTransaction != null)
            return;
        _currentTransaction =
            await base.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted).ConfigureAwait(false);
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await SaveChangesAsync().ConfigureAwait(false);
            _currentTransaction?.Commit();
        }
        catch
        {
            RollbackTransaction();
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    private void RollbackTransaction()
    {
        try
        {
            _currentTransaction?.Rollback();
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}