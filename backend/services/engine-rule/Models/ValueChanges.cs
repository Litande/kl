namespace KL.Engine.Rule.Models;

public record ValueChanges<T>(string Name, T From, T To); 
