using System.ComponentModel;

namespace KL.Manager.API.Application.Enums;

public enum PerformanceTypes
{
    [Description("avg_call")] AvgCall = 1,
    [Description("avg_new_client")] AvgNewClient = 2,
    [Description("success_call")] SuccessCall = 3,
}