using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ConventionalOptions.DependencyInjection
{
public static class OptionsServiceCollectionExtensions
    {
        public static void RegisterOptionsFromAssemblies(this IServiceCollection services, IConfiguration configuration, params Assembly[] assemblies)
        {
            var types = assemblies.SelectMany(a =>
                a.GetTypes().Where(t => !t.IsAbstract && !t.IsInterface && t.Name.EndsWith("Options"))).ToList();

            foreach (var t in types) services.RegisterOptions(configuration, t);
        }

        public static void RegisterOptions<TOptions>(this IServiceCollection services, IConfiguration configuration)
        {
            services.RegisterOptions(configuration, typeof(TOptions));
        }

        public static void RegisterOptions(this IServiceCollection services, IConfiguration configuration, Type optionsType)
        {
            RegisterChangeTokenSource(services, optionsType, configuration);
            RegisterConfigureOptions(services, optionsType, configuration);
            RegisterOptions(services, optionsType);
        }

        static void RegisterOptions(IServiceCollection services, Type optionsType)
        {
            services.Add(new ServiceDescriptor(optionsType, serviceProvider =>
            {
                var genericType = typeof(IOptions<>).MakeGenericType(optionsType);
                var options = serviceProvider.GetService(genericType);
                return genericType.GetProperty("Value").GetValue(options);
            }, ServiceLifetime.Singleton));
        }

        static void RegisterConfigureOptions(IServiceCollection services, Type optionsType, IConfiguration configurationSection)
        {
            var configurationOptionsInterfaceType = typeof(IConfigureOptions<>).MakeGenericType(optionsType);

            services.Add(new ServiceDescriptor(configurationOptionsInterfaceType, serviceProvider =>
            {
                var configurationOptionsType = typeof(ConfigureFromConfigurationSectionOptions<>).MakeGenericType(optionsType);
                var configurationOptionsTypeInstance = Activator.CreateInstance(configurationOptionsType, configurationSection);

                return configurationOptionsTypeInstance;
            }, ServiceLifetime.Singleton));
        }

        static void RegisterChangeTokenSource(IServiceCollection services, Type optionsType, IConfiguration configurationSection)
        {
            var optionsChangeTokenSourceInterfaceType = typeof(IOptionsChangeTokenSource<>).MakeGenericType(optionsType);

            services.Add(new ServiceDescriptor(optionsChangeTokenSourceInterfaceType, serviceProvider =>
            {
                var changeTokenSourceType = typeof(ConfigurationChangeTokenSource<>).MakeGenericType(optionsType);
                var configurationChangeTokenSourceInstance = Activator.CreateInstance(changeTokenSourceType, Options.DefaultName, configurationSection);
                return configurationChangeTokenSourceInstance;
            }, ServiceLifetime.Singleton));
        }
    }
}