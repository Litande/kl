﻿namespace KL.Manager.API.Persistent.Entities.Projections;

public class AssignedAgentProjection
{
    public long AgentId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}