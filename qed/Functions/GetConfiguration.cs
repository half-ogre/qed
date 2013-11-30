namespace qed
{
    public static partial class Functions
    {
        public static T GetConfiguration<T>(string key)
        {
            object value;
            return _configuration.TryGetValue(key, out value) ? (T)value : default(T);
        }
    }
}
