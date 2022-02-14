using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using StructureMap;

namespace ConventionalOptions.StructureMap
{
    public static class StructureMapOptionsServiceCollectionExtensions
    {
        /// <summary>
        /// Registers base option types with the StructureMap Container.
        ///
        /// The intent of this method is to provide registration of base option types
        /// when used outside the context of an Asp.Net Core Web application as
        /// use of StructureMap with web apps would typically use the AddOptions() extension
        /// method and rely upon StructureMap's Populate() method to register services from
        /// the ServiceCollection.
        /// </summary>
        /// <param name="configurationExpression"></param>
        /// <returns></returns>
        public static ConfigurationExpression AddOptions(this ConfigurationExpression configurationExpression)
        {
            if (configurationExpression == null)
            {
                throw new ArgumentNullException(nameof(configurationExpression));
            }

            configurationExpression.For(typeof(IOptions<>)).Use(typeof(OptionsManager<>)).Singleton();
            configurationExpression.For(typeof(IOptionsSnapshot<>)).Use(typeof(OptionsManager<>)).ContainerScoped();
            configurationExpression.For(typeof(IOptionsMonitor<>)).Use(typeof(OptionsMonitor<>)).Singleton();
            configurationExpression.For(typeof(IOptionsFactory<>)).Use(typeof(OptionsFactory<>)).Transient();
            configurationExpression.For(typeof(IOptionsMonitorCache<>)).Use(typeof(OptionsCache<>)).Singleton();

            return configurationExpression;
        }

        public static ConfigurationExpression RegisterOptionsFromAssemblies(this ConfigurationExpression configurationExpression, IConfiguration configuration, params Assembly[] assemblies)
        {
            var types = assemblies.SelectMany(a => a.GetTypes().Where(t => !t.IsAbstract && !t.IsInterface && t.Name.EndsWith("Options"))).ToList();

            foreach (var t in types) configurationExpression.RegisterOptions(configuration, t);

            return configurationExpression;
        }

        public static ConfigurationExpression RegisterOptions<TOptions>(this ConfigurationExpression configurationExpression, IConfiguration configuration)
        {
            configurationExpression.RegisterOptions(configuration, typeof(TOptions));

            return configurationExpression;
        }

        public static ConfigurationExpression RegisterOptions(this ConfigurationExpression configurationExpression, IConfiguration configuration, Type optionsType)
        {
            RegisterChangeTokenSource(configurationExpression, optionsType, configuration);
            RegisterConfigureOptions(configurationExpression, optionsType, configuration);
            RegisterOptions(configurationExpression, optionsType);

            return configurationExpression;
        }

        static void RegisterOptions(ConfigurationExpression configurationExpression, Type optionsType)
        {
            configurationExpression.For(optionsType).Use($"Build ${optionsType}", context =>
                {
                    var genericType = typeof(IOptions<>).MakeGenericType(optionsType);
                    var options = context.GetInstance(genericType);
                    return genericType.GetProperty("Value").GetValue(options);
                }).Singleton();
        }

        static void RegisterConfigureOptions(ConfigurationExpression configurationExpression, Type optionsType, IConfiguration configuration)
        {
            var configurationOptionsInterfaceType = typeof(IConfigureOptions<>).MakeGenericType(optionsType);

            configurationExpression.For(configurationOptionsInterfaceType).Use($"Build ${configurationOptionsInterfaceType}", context =>
            {
                var configurationOptionsType = typeof(ConfigureFromConfigurationSectionOptions<>).MakeGenericType(optionsType);
                var configurationOptionsTypeInstance = Activator.CreateInstance(configurationOptionsType, configuration);

                return configurationOptionsTypeInstance;
            }).Singleton();
        }

        static void RegisterChangeTokenSource(ConfigurationExpression registry, Type optionsType, IConfiguration configuration)
        {
            var optionsChangeTokenSourceInterfaceType = typeof(IOptionsChangeTokenSource<>).MakeGenericType(optionsType);

            registry.For(optionsChangeTokenSourceInterfaceType).Use($"Build ${optionsChangeTokenSourceInterfaceType}",
                context =>
                {
                    var changeTokenSourceType = typeof(ConfigurationChangeTokenSource<>).MakeGenericType(optionsType);
                    var configurationChangeTokenSourceInstance = Activator.CreateInstance(changeTokenSourceType, Options.DefaultName, configuration);
                    return configurationChangeTokenSourceInstance;
                }).Singleton();
        }
    }
}