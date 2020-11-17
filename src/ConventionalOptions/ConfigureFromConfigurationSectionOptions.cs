using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ConventionalOptions
{
    public class ConfigureFromConfigurationSectionOptions<TOptions> : ConfigureOptions<TOptions> where TOptions : class
    {
        public ConfigureFromConfigurationSectionOptions(IConfiguration config)
            : base(options =>
            {
                var name = typeof(TOptions).Name;
                var sectionName = name.Substring(0, name.Length - 7);
                var section = config.GetSection(sectionName);

                if (section == null)
                    throw new Exception($"Could not find section \"{sectionName}\" in configuration.");

                section.Bind(options);
            })
        {
            if (config == null)
                throw new ArgumentNullException("config");
        }
    }
}