using Microsoft.EntityFrameworkCore;

namespace Octopus.Data;

internal static class Dumper
{
    public static async ValueTask Dump<T>(T[] data, string connection)
    {
        using OctopusDbContext db = new(connection);
        await db.AddRangeAsync(data);
        await db.SaveChangesAsync();
    }

    public static async ValueTask MigrateDb(string connection)
    {
        OctopusDbContext db = new(connection);
        await db.Database.EnsureCreatedAsync();
        await db.Database.MigrateAsync();
    }
}