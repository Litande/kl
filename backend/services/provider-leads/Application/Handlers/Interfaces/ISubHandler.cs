﻿namespace KL.Provider.Leads.Application.Handlers.Interfaces;

public interface ISubHandler<in TMessage>
{
    Task Process(TMessage message, CancellationToken ct = default);
}