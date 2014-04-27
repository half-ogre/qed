using System;
using Owin;

namespace qed
{
    public static partial class Functions
    {
        public static void UseQed(this IAppBuilder appBuilder)
        {
            ConfigureBuilder(appBuilder);
        }
    }
}
