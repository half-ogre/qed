using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace qed
{
    public static partial class Functions
    {
        public static IEnumerable<BuildConfiguration> GetBuildConfigurations()
        {
            var buildConfigurations = GetConfiguration<IEnumerable<BuildConfiguration>>(Constants.Configuration.BuildConfigurationsKey);
            if (buildConfigurations != null)
                return buildConfigurations;

            var baseDirectory = GetBaseDirectory();
            var buildConfigurationsPath = Path.Combine(baseDirectory, "build.config");
            if (!File.Exists(buildConfigurationsPath))
            {
                var rootBuildConfigurationsPath = Path.Combine(baseDirectory, @"..\..\..\", "Build.config");
                
                if (File.Exists(rootBuildConfigurationsPath))
                    buildConfigurationsPath = rootBuildConfigurationsPath;
                else
                    return new BuildConfiguration[0];
            }

            var json = File.ReadAllText(buildConfigurationsPath);
            
            buildConfigurations = JsonConvert.DeserializeObject<IEnumerable<BuildConfiguration>>(json);
            
            SetConfiguration(Constants.Configuration.BuildConfigurationsKey, buildConfigurations);

            return buildConfigurations;
        }
    }

}