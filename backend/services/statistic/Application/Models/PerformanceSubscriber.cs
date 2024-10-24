﻿using KL.Statistics.Application.Common.Enums;

namespace KL.Statistics.Application.Models;

public class PerformanceSubscriber
{
    public long ClientId { get; set; }
    public PlotSubscription? PlotSubscription { get; set; }
    public StatisticSubscription? StatisticSubscription { get; set; }
}

public record PlotSubscription(DateTimeOffset From, int Step, PerformanceTypes Type);
public record StatisticSubscription(DateTimeOffset From, PerformanceTypes[] Types);