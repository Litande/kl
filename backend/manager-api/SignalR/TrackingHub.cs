﻿using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Plat4Me.DialClientApi.Application.Common;
using Plat4Me.DialClientApi.Application.Models;
using Plat4Me.DialClientApi.Application.Models.Requests.Dashboard;

namespace Plat4Me.DialClientApi.SignalR;

[Authorize(Roles = "Manager")]
public class TrackingHub : Hub
{
    private const string GroupNameFormat = "client_id_{0}_group";
    public static readonly ConcurrentDictionary<string, PerformanceSubscriber> PerformanceSubscribers = new();

    public static string GetGroupName(long clientId) => string.Format(GroupNameFormat, clientId);

    public override async Task OnConnectedAsync()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, GetGroupName(CurrentClientId));

        await base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var contains = PerformanceSubscribers.ContainsKey(Context.ConnectionId);

        if (contains)
            PerformanceSubscribers.Remove(Context.ConnectionId, out _);

        return base.OnDisconnectedAsync(exception);
    }

    public async Task SubscribeAgentPerformancePlot(AgentPerformancePlotSubscription subscription)
    {
        PerformanceSubscribers.TryGetValue(Context.ConnectionId, out var subscriber);
        var newSubscription = new PlotSubscription(subscription.From, subscription.Step, subscription.Type);
        if (subscriber is not null)
        {
            subscriber.PlotSubscription = newSubscription;
        }
        else
        {
            PerformanceSubscribers.TryAdd(Context.ConnectionId, new PerformanceSubscriber
            {
                ClientId = CurrentClientId,
                PlotSubscription = newSubscription
            });
        }
    }

    public async Task SubscribeAgentPerformanceStatistic(AgentPerformanceStatisticSubscription subscription)
    {
        if (subscription.Types is null)
            return;

        PerformanceSubscribers.TryGetValue(Context.ConnectionId, out var subscriber);
        var newSubscription = new StatisticSubscription(subscription.From, subscription.Types);
        if (subscriber is not null)
        {
            subscriber.StatisticSubscription = newSubscription;
        }
        else
        {
            PerformanceSubscribers.TryAdd(Context.ConnectionId, new PerformanceSubscriber
            {
                ClientId = CurrentClientId,
                StatisticSubscription = newSubscription
            });
        }
    }

    protected long CurrentClientId
    {
        get
        {
            var clientId = Context.User?.Claims
                .FirstOrDefault(c => c.Type == CustomClaimTypes.ClientId);

            if (clientId is null)
                throw new KeyNotFoundException("UserId not in claims");

            return long.Parse(clientId.Value);
        }
    }
}