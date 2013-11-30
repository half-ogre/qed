using System;

namespace qed
{
    public static partial class Functions
    {
        public static string GetBaseDirectory()
        {
            var baseDirectory = GetConfiguration<string>(Constants.Configuration.BaseDirectoryKey);
            if (baseDirectory != null)
                return baseDirectory;

            baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            SetConfiguration(Constants.Configuration.BaseDirectoryKey, baseDirectory);

            return baseDirectory;
        }
    }
}
