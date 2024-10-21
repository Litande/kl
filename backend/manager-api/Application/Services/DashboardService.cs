using Microsoft.Extensions.Logging;
using Plat4Me.DialClientApi.Application.Models.Responses.Dashboard;
using System.Globalization;
using Plat4Me.DialClientApi.Application.Enums;
using Plat4Me.DialClientApi.Application.Services.Interfaces;
using Plat4Me.DialClientApi.Persistent.Repositories.Interfaces;

namespace Plat4Me.DialClientApi.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IStatisticPeriodService _periodService;
    private readonly ICDRRepository _cdrRepository;
    private readonly IEnumerable<RegionInfo> _regionInfoList;
    private readonly ILogger<DashboardService> _logger;

    public DashboardService(
        IStatisticPeriodService periodService,
        ICDRRepository cdrRepository,
        ILogger<DashboardService> logger)
    {
        _periodService = periodService;
        _cdrRepository = cdrRepository;
        _logger = logger;
        _regionInfoList = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
            .Select(culture => new RegionInfo(culture.Name))
            .ToList();
    }

    public async Task<CallAnalysisResponse> CalculateCallAnalysis(
        long clientId,
        PeriodTypes periodType,
        CancellationToken ct = default)
    {
        var currentPeriod = _periodService.GetCurrent(periodType);
        var calls = await _cdrRepository.GetCallsByPeriod(clientId, currentPeriod.From, currentPeriod.To, ct);

        var callAnalysis = calls
            .Where(c => c.LeadCountry is not null && (c.LeadAnswerAt is not null || c.UserAnswerAt is not null))
            .GroupBy(c => c.LeadCountry)
            .Select(c =>
            {
                var regionInfo = _regionInfoList.FirstOrDefault(r => r.TwoLetterISORegionName.Contains(c.Key!));
                var code = regionInfo is null ? c.Key! : regionInfo.ThreeLetterISORegionName;
                var countryEnglishName = regionInfo is null ? c.Key! : regionInfo.EnglishName;
                var allCalls = c.Count();
                var answeredCalls = c.Count(c => c.LeadAnswerAt.HasValue && c.UserAnswerAt.HasValue);
                var avgTime = c.Average(y => (y.CallHangupAt - new DateTimeOffset(
                    Math.Max((y.LeadAnswerAt?.Ticks ?? 0), (y.UserAnswerAt?.Ticks ?? 0)),
                    TimeSpan.Zero))?.TotalSeconds);
                return new CallAnalysisItem(countryEnglishName!, code, allCalls,
                    Calculate(answeredCalls, allCalls).ToString(),
                    TimeSpan.FromSeconds(Math.Round(avgTime ?? 0)).ToString());
            }).ToList();

        return new CallAnalysisResponse(periodType, callAnalysis);
    }

    private static double Calculate(double source, double target, bool acceptNegative = false)
    {
        if (acceptNegative && source > target)
        {
            return 100;
        }

        return target > 0 ? Math.Round(source / target * 100) : 0;
    }
}