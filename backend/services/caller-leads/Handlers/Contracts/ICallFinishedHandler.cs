﻿using KL.Caller.Leads.Models.Messages;

namespace KL.Caller.Leads.Handlers.Contracts;

public interface ICallFinishedHandler : ISubHandler<CallFinishedMessage>
{

}