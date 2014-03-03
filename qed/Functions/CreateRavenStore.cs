using Raven.Client;
using Raven.Client.Embedded;
using fn = qed.Functions;

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
                DataDirectory = GetConfiguration<string>(Constants.Configuration.RavenDataDirectoryKey)
            };
            
            ravenStore.Initialize();

            SetConfiguration(Constants.Configuration.RavenStoreKey, ravenStore);

            return ravenStore;
        }
    }
}
