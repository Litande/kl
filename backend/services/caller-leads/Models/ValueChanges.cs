namespace KL.Caller.Leads.Models;

public record ValueChanges<T>(string Name, T From, T To);
