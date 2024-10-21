using Plat4Me.DialAgentApi.Application.Enums;

namespace Plat4Me.DialAgentApi.Persistent.Configurations;

public static class SettingConstants
{
    public static class ProductiveDialer
    {
        private const string Key = nameof(SettingTypes.ProductiveDialer);
        public const string EndCallButtonAfter = $"{Key}.endCallButtonAfterThisAmountOfSeconds";
        public const string MaxCallDuration = $"{Key}.maxCallDuration";
    }
    public static class VoiceMail
    {
        private const string Key = nameof(SettingTypes.VoiceMail);
        public const string ShowVoicemailButton = $"{Key}.showVoicemailButtonAfterThisAmountOfSecondsOfCall";
        public const string HideVoicemailButton = $"{Key}.hideVoicemailButtonAfterThisAmountOfSecondsOfCall";
    }

    public static class Feedback
    {
        private const string Key = nameof(SettingTypes.Feedback);
        public const string PageTimeout = $"{Key}.pageTimeout";
        public const string RedialsLimit = $"{Key}.redialsLimit";
    }
}
