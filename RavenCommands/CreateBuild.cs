namespace qed
{
    public static partial class Functions
    {
        public static Build CreateBuild(
            string command,
            string commandArguments,
            string repositoryName,
            string repositoryOwner,
            string repositoryUrl,
            string @ref,
            string revision,
            string eventType,
            string @event)
        {
            // TODO: validate args

            using (var ravenSession = OpenRavenSession())
            {
                var build = new Build
                {
                    Command = command,
                    CommandArguments = commandArguments,
                    RepositoryName = repositoryName,
                    RepositoryOwner = repositoryOwner,
                    RepositoryUrl = repositoryUrl,
                    Ref = @ref,
                    Revision = revision,
                    EventType = eventType,
                    Event = @event
                };

                ravenSession.Store(build);
                ravenSession.SaveChanges();

                return build;
            }
        }
    }
}
