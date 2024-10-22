using KL.Caller.Leads.Models.Entities;

namespace KL.Caller.Leads.Seed;

public static class SeedClient
{
    public static long Seed(KlDbContext context)
    {
        var clientId = AddClientIfNeed(context);
        return clientId;
    }

    private static long AddClientIfNeed(KlDbContext context)
    {
        const long defaultClientId = 1;
        var client = context.Clients.Where(r => r.Id == defaultClientId).FirstOrDefault();
        if (client is not null) return client.Id;

        client = new Client { Id = defaultClientId, };
        context.Clients.Add(client);
        context.SaveChanges();

        return client.Id;
    }
}