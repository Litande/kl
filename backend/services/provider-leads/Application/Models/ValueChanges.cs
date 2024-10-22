namespace KL.Provider.Leads.Application.Models;

public record ValueChanges<T>(string Name, T From, T To);