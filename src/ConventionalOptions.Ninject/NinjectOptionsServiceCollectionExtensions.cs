using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Ninject;

namespace ConventionalOptions.Ninject
{
    public static class NinjectOptionsServiceCollectionExtensions
    {
        /// <summary>
        /// Registers base option types with the Ninject Container.
        ///
        /// The intent of this method is to provide registration of base option types
        /// when used outside the context of an Asp.Net Core Web application.
        /// </summary>
        /// <param name="kernel"></param>
        /// <returns></returns>
        public static IKernel AddOptions(this IKernel kernel)
        {
            if (kernel == null)
            {
                throw new ArgumentNullException(nameof(kernel));
            }

            kernel.Bind(typeof(IOptions<>)).To(typeof(OptionsManager<>)).InSingletonScope();
            kernel.Bind(typeof(IOptionsSnapshot<>)).To(typeof(OptionsManager<>)).InSingletonScope();
            kernel.Bind(typeof(IOptionsMonitor<>)).To(typeof(OptionsMonitor<>)).InSingletonScope();
            kernel.Bind(typeof(IOptionsFactory<>)).To(typeof(OptionsFactory<>)).InSingletonScope();
            kernel.Bind(typeof(IOptionsMonitorCache<>)).To(typeof(OptionsCache<>)).InSingletonScope();

            return kernel;
        }

        public static IKernel RegisterOptionsFromAssemblies(this IKernel builder, IConfiguration configuration, params Assembly[] assemblies)
        {
            var types = assemblies.SelectMany(a => a.GetTypes().Where(t => !t.IsAbstract && !t.IsInterface && t.Name.EndsWith("Options"))).ToList();

            foreach (var t in types) builder.RegisterOptions(configuration, t);

            return builder;
        }

        public static IKernel RegisterOptions<TOptions>(this IKernel kernel, IConfiguration configuration)
        {
            kernel.RegisterOptions(configuration, typeof(TOptions));

            return kernel;
        }

        public static IKernel RegisterOptions(this IKernel kernel, IConfiguration configuration, Type optionsType)
        {
            RegisterChangeTokenSource(kernel, optionsType, configuration);
            RegisterConfigureOptions(kernel, optionsType, configuration);
            RegisterOptions(kernel, optionsType);

            return kernel;
        }

        static void RegisterOptions(IKernel kernel, Type optionsType)
        {
            kernel.Bind(optionsType).ToMethod(context =>
                {
                    var genericType = typeof(IOptions<>).MakeGenericType(optionsType);
                    dynamic options = context.Kernel.Get(genericType);
                    return options.Value;
                }
            );
        }

        static void RegisterConfigureOptions(IKernel kernel, Type optionsType, IConfiguration configuration)
        {
            var configurationOptionsInterfaceType = typeof(IConfigureOptions<>).MakeGenericType(optionsType);

            kernel.Bind(configurationOptionsInterfaceType).ToMethod(context =>
            {
                var configurationOptionsType = typeof(ConfigureFromConfigurationSectionOptions<>).MakeGenericType(optionsType);
                var configurationOptionsTypeInstance = Activator.CreateInstance(configurationOptionsType, configuration);

                return configurationOptionsTypeInstance;
            }).InSingletonScope();
        }

        static void RegisterChangeTokenSource(IKernel kernel, Type optionsType, IConfiguration configuration)
        {
            var optionsChangeTokenSourceInterfaceType = typeof(IOptionsChangeTokenSource<>).MakeGenericType(optionsType);

            kernel.Bind(optionsChangeTokenSourceInterfaceType).ToMethod(context =>
            {
                var changeTokenSourceType = typeof(ConfigurationChangeTokenSource<>).MakeGenericType(optionsType);
                var configurationChangeTokenSourceInstance = Activator.CreateInstance(changeTokenSourceType, Options.DefaultName, configuration);
                return configurationChangeTokenSourceInstance;
            }).InSingletonScope();
        }
    }
}