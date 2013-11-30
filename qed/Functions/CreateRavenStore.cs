using Raven.Client;
using Raven.Client.Embedded;

namespace qed
{
    public static partial class Functions
    {
        public static IDocumentStore GetRavenStore()
        {
            var ravenStore = GetConfiguration<IDocumentStore>(Constants.Configuration.RavenStoreKey);
            if (ravenStore != null)
                return ravenStore;

            ravenStore = new EmbeddableDocumentStore
            {
                DataDirectory = "~\\.ravendb"
            };
            
            ravenStore.Initialize();

            SetConfiguration(Constants.Configuration.RavenStoreKey, ravenStore);

            return ravenStore;
        }
    }
}
