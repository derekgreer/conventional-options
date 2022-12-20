using System;
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

        public static IKernel RegisterOptionsFromAssemblies(this IKernel builder, params Assembly[] assemblies)
        {
            var types = assemblies.SelectMany(a => a.GetTypes().Where(t => !t.IsAbstract && !t.IsInterface && t.Name.EndsWith("Options"))).ToList();

            foreach (var t in types) builder.RegisterOptions(t);

            return builder;
        }

        public static IKernel RegisterOptions<TOptions>(this IKernel kernel)
        {
            kernel.RegisterOptions(typeof(TOptions));

            return kernel;
        }

        public static IKernel RegisterOptions(this IKernel kernel, Type optionsType)
        {
            RegisterChangeTokenSource(kernel, optionsType);
            RegisterConfigureOptions(kernel, optionsType);
            RegisterOptionsInternal(kernel, optionsType);

            return kernel;
        }

        static void RegisterOptionsInternal(IKernel kernel, Type optionsType)
        {
            kernel.Bind(optionsType).ToMethod(context =>
                {
                    var genericType = typeof(IOptions<>).MakeGenericType(optionsType);
                    var options = context.Kernel.Get(genericType);
                    return genericType.GetProperty("Value").GetValue(options);
                }
            );
        }

        static void RegisterConfigureOptions(IKernel kernel, Type optionsType)
        {
            var configurationOptionsInterfaceType = typeof(IConfigureOptions<>).MakeGenericType(optionsType);

            kernel.Bind(configurationOptionsInterfaceType).ToMethod(context =>
            {
                var configuration = context.Kernel.Get<IConfiguration>();
                var configurationOptionsType = typeof(ConfigureFromConfigurationSectionOptions<>).MakeGenericType(optionsType);
                var configurationOptionsTypeInstance = Activator.CreateInstance(configurationOptionsType, configuration);

                return configurationOptionsTypeInstance;
            }).InSingletonScope();
        }

        static void RegisterChangeTokenSource(IKernel kernel, Type optionsType)
        {
            var optionsChangeTokenSourceInterfaceType = typeof(IOptionsChangeTokenSource<>).MakeGenericType(optionsType);

            kernel.Bind(optionsChangeTokenSourceInterfaceType).ToMethod(context =>
            {
                var configuration = context.Kernel.Get<IConfiguration>();
                var changeTokenSourceType = typeof(ConfigurationChangeTokenSource<>).MakeGenericType(optionsType);
                var configurationChangeTokenSourceInstance = Activator.CreateInstance(changeTokenSourceType, Options.DefaultName, configuration);
                return configurationChangeTokenSourceInstance;
            }).InSingletonScope();
        }
    }
}