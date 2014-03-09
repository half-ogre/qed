using Raven.Client;
using Raven.Client.Document;
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

            if (string.IsNullOrEmpty(GetConfiguration<string>(Constants.Configuration.RavenConnectionStringKey)))
            {
                ravenStore = new EmbeddableDocumentStore
                {
                    DataDirectory = GetConfiguration<string>(Constants.Configuration.RavenDataDirectoryKey)
                };
            }
            else
            { 
                ravenStore = new DocumentStore
                {
                    Url = GetConfiguration<string>(Constants.Configuration.RavenConnectionStringKey)
                };
            }
            
            ravenStore.Initialize();

            SetConfiguration(Constants.Configuration.RavenStoreKey, ravenStore);

            return ravenStore;
        }
    }
}