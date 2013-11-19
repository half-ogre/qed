using System;
using System.Linq;

namespace qed
{
    public static partial class Functions
    {
        public static BuildConfiguration GetBuildConfiguration(string owner, string name)
        {
            var buildConfigurations = GetBuildConfigurations();

            return buildConfigurations.FirstOrDefault(bc =>
                bc.Owner.Equals(owner, StringComparison.OrdinalIgnoreCase) &&
                bc.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}