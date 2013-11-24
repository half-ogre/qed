using System;

namespace qed
{
    public class Build
    {
        public string Command { get; set; }
        public string CommandArguments { get; set; }
        public string Event { get; set; }
        public string EventType { get; set; }
        public DateTimeOffset? Finished { get; set; }
        public Guid Id { get; set; }
        public string Ouput { get; set; }
        public DateTimeOffset? Queued { get; set; }
        public string Ref { get; set; }
        public string RepositoryName { get; set; }
        public string RepositoryOwner { get; set; }
        public string RepositoryUrl { get; set; }
        public string Revision { get; set; }    
        public DateTimeOffset? Started { get; set; }
        public bool? Succeeded { get; set; }
    }
}