﻿using KL.Agent.API.Application.Enums;

namespace KL.Agent.API.Persistent.Entities;

public class SettingsEntity
{
    public long Id { get; set; }
    public string Value { get; set; } = null!;
    public long ClientId { get; set; }
    public SettingTypes Type { get; set; }
}