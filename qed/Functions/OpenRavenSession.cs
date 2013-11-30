using Raven.Client;

namespace qed
{
    public static partial class Functions
    {
        public static IDocumentSession OpenRavenSession()
        {
            return GetRavenStore().OpenSession();
        }
    }
}
