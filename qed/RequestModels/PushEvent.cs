namespace qed
{
    public class PushEvent
    {
        public string After { get; set; }
        public string Before { get; set; }
        public string Ref { get; set; }
        public Repository Repository { get; set; }
    }
}