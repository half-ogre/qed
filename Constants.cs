namespace qed
{
    public static class Constants
    {
        public static class Configuration
        {
            public const string BaseDirectoryKey = "BaseDirectory";
            public const string BuildConfigurationsKey = "BuildConfigurations";
            public const string HostKey = "Host";
            public const string RavenStoreKey = "RavenStore";
        }

        public static class Owin
        {
            public const string CallCancelledKey = "owin.CallCancelled";
            public const string RequestHeadersKey = "owin.RequestHeaders";
            public const string RequestMethodKey = "owin.RequestMethod";
            public const string RequestPathKey = "owin.RequestPath";
            public const string ResponseBodyKey = "owin.ResponseBody";
            public const string ResponseHeadersKey = "owin.ResponseHeaders";
            public const string ResponseStatusCodeKey = "owin.ResponseStatusCode";
        }

        public static class OwinExtensions
        {
            public const string RequestFormKey = "owinextensions.RequestForm";
        }
    }
}
