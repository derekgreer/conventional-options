using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ConventionalOptions.Autofac
{
    public static class AutofacOptionsServiceCollectionExtensions
    {
        /// <summary>
        /// Registers base option types with the Autofac Container.
        ///
        /// The intent of this method is to provide registration of base option types
        /// when used outside the context of an Asp.Net Core Web application as
        /// use of Autofac with web apps would typically use the AddOptions() extension
        /// method and rely upon Autofac's Populate() method to register services from
        /// the ServiceCollection.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ContainerBuilder AddOptions(this ContainerBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.RegisterGeneric(typeof(OptionsManager<>)).As(typeof(IOptions<>)).SingleInstance();
            builder.RegisterGeneric(typeof(OptionsManager<>)).As(typeof(IOptionsSnapshot<>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(OptionsMonitor<>)).As(typeof(IOptionsMonitor<>)).SingleInstance();
            builder.RegisterGeneric(typeof(OptionsFactory<>)).As(typeof(IOptionsFactory<>)).InstancePerDependency();
            builder.RegisterGeneric(typeof(OptionsCache<>)).As(typeof(IOptionsMonitorCache<>)).SingleInstance();
            return builder;
        }

        public static ContainerBuilder RegisterOptionsFromAssemblies(this ContainerBuilder builder, IConfiguration configuration, params Assembly[] assemblies)
        {
            var types = assemblies.SelectMany(a => a.GetTypes().Where(t => !t.IsAbstract && !t.IsInterface && t.Name.EndsWith("Options"))).ToList();

            foreach (var t in types) builder.RegisterOptions(configuration, t);

            return builder;
        }

        public static ContainerBuilder RegisterOptions<TOptions>(this ContainerBuilder builder, IConfiguration configuration)
        {
            builder.RegisterOptions(configuration, typeof(TOptions));

            return builder;
        }

        public static ContainerBuilder RegisterOptions(this ContainerBuilder builder, IConfiguration configuration, Type optionsType)
        {
            RegisterChangeTokenSource(builder, optionsType, configuration);
            RegisterConfigureOptions(builder, optionsType, configuration);
            RegisterOptions(builder, optionsType);

            return builder;
        }

        static void RegisterOptions(ContainerBuilder builder, Type optionsType)
        {
            builder.Register(context =>
                {
                    var genericType = typeof(IOptions<>).MakeGenericType(optionsType);
                    var options = context.Resolve(genericType);
                    return genericType.GetProperty("Value").GetValue(options);
                }
            ).As(optionsType).SingleInstance();
        }

        static void RegisterConfigureOptions(ContainerBuilder builder, Type optionsType, IConfiguration configuration)
        {
            var configurationOptionsInterfaceType = typeof(IConfigureOptions<>).MakeGenericType(optionsType);

            builder.Register(context =>
            {
                var configurationOptionsType = typeof(ConfigureFromConfigurationSectionOptions<>).MakeGenericType(optionsType);
                var configurationOptionsTypeInstance = Activator.CreateInstance(configurationOptionsType, configuration);

                return configurationOptionsTypeInstance;
            }).As(configurationOptionsInterfaceType).SingleInstance();
        }

        static void RegisterChangeTokenSource(ContainerBuilder builder, Type optionsType, IConfiguration configuration)
        {
            var optionsChangeTokenSourceInterfaceType = typeof(IOptionsChangeTokenSource<>).MakeGenericType(optionsType);

            builder.Register(context =>
            {
                var changeTokenSourceType = typeof(ConfigurationChangeTokenSource<>).MakeGenericType(optionsType);
                var configurationChangeTokenSourceInstance = Activator.CreateInstance(changeTokenSourceType, Options.DefaultName, configuration);
                return configurationChangeTokenSourceInstance;
            }).As(optionsChangeTokenSourceInterfaceType).SingleInstance();
        }
    }
}