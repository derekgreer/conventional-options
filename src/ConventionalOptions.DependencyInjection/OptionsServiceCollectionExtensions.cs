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
        public static void RegisterOptionsFromAssemblies(this IServiceCollection services, params Assembly[] assemblies)
        {
            var types = assemblies.SelectMany(a =>
                a.GetTypes().Where(t => !t.IsAbstract && !t.IsInterface && t.Name.EndsWith("Options"))).ToList();

            foreach (var t in types) services.RegisterOptions(t);
        }

        public static void RegisterOptions<TOptions>(this IServiceCollection services, IConfiguration configuration)
        {
            services.RegisterOptions(typeof(TOptions));
        }

        public static void RegisterOptions(this IServiceCollection services, Type optionsType)
        {
            RegisterChangeTokenSource(services, optionsType);
            RegisterConfigureOptions(services, optionsType);
            RegisterOptionsInternal(services, optionsType);
        }

        static void RegisterOptionsInternal(IServiceCollection services, Type optionsType)
        {
            services.Add(new ServiceDescriptor(optionsType, serviceProvider =>
            {
                var genericType = typeof(IOptions<>).MakeGenericType(optionsType);
                var options = serviceProvider.GetService(genericType);
                return genericType.GetProperty("Value").GetValue(options);
            }, ServiceLifetime.Singleton));
        }

        static void RegisterConfigureOptions(IServiceCollection services, Type optionsType)
        {
            var configurationOptionsInterfaceType = typeof(IConfigureOptions<>).MakeGenericType(optionsType);

            services.Add(new ServiceDescriptor(configurationOptionsInterfaceType, serviceProvider =>
            {
                var configuration = serviceProvider.GetService<IConfiguration>();
                var configurationOptionsType = typeof(ConfigureFromConfigurationSectionOptions<>).MakeGenericType(optionsType);
                var configurationOptionsTypeInstance = Activator.CreateInstance(configurationOptionsType, configuration);

                return configurationOptionsTypeInstance;
            }, ServiceLifetime.Singleton));
        }

        static void RegisterChangeTokenSource(IServiceCollection services, Type optionsType)
        {
            var optionsChangeTokenSourceInterfaceType = typeof(IOptionsChangeTokenSource<>).MakeGenericType(optionsType);

            services.Add(new ServiceDescriptor(optionsChangeTokenSourceInterfaceType, serviceProvider =>
            {
                var configuration = serviceProvider.GetService<IConfiguration>();
                var changeTokenSourceType = typeof(ConfigurationChangeTokenSource<>).MakeGenericType(optionsType);
                var configurationChangeTokenSourceInstance = Activator.CreateInstance(changeTokenSourceType, Options.DefaultName, configuration);
                return configurationChangeTokenSourceInstance;
            }, ServiceLifetime.Singleton));
        }
    }
}