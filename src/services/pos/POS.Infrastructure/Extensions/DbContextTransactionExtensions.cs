#nullable enable

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace POS.Infrastructure.Extensions;

public static class DbContextTransactionExtensions
{
    public static bool HasActiveTransaction(this POSContext context)
    {
        return context.Database.CurrentTransaction != null;
    }

    public static async Task<IDbContextTransaction> BeginTransactionAsync(this POSContext context)
    {
        return await context.Database.BeginTransactionAsync();
    }

    public static async Task CommitTransactionAsync(this POSContext context, IDbContextTransaction transaction)
    {
        await transaction.CommitAsync();
    }

    public static IDbContextTransaction? GetCurrentTransaction(this POSContext context)
    {
        return context.Database.CurrentTransaction;
    }
} 
