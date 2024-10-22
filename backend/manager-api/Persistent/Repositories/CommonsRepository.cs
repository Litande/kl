using System.Globalization;
using KL.Manager.API.Application.Enums;
using KL.Manager.API.Application.Extensions;
using KL.Manager.API.Application.Models.Responses.Common;
using KL.Manager.API.Persistent.Repositories.Interfaces;

namespace KL.Manager.API.Persistent.Repositories;

public class CommonsRepository : ICommonsRepository
{
    //private readonly DialDbContext _context;

    public IReadOnlyCollection<LabelValue> Countries { get; protected set; }
    public IReadOnlyCollection<LabelValue> LeadStatuses { get; protected set; }

    public CommonsRepository(
        //DialDbContext context
    )
    {
        //_context = context;
        Countries = PrepareCountries();
        LeadStatuses = PrepareLeadStatuses();
    }

    private IReadOnlyCollection<LabelValue> PrepareCountries()
    {
        var cultureList = new List<LabelValue>();
        var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures);
        foreach (var culture in cultures)
        {
            var region = new RegionInfo(culture.Name);
            if (cultureList.Any(r => r.Value == region.TwoLetterISORegionName)) continue;

            cultureList.Add(new LabelValue(region.EnglishName, region.TwoLetterISORegionName));
        }

        return cultureList
            .OrderBy(x => x.Label).ToArray();
    }

    private IReadOnlyCollection<LabelValue> PrepareLeadStatuses()
    {
        var items = EnumExtensions.EnumToList<LeadStatusTypes>()
            .Select(r => new LabelValue(
                r.Key.ToDescription(),
                r.Key.ToString()))
            .ToArray();

        return items;
    }
}
