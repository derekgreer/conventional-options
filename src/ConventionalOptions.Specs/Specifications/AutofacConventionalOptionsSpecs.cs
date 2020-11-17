using System.Collections.Generic;
using System.Reflection;
using Autofac;
using ConventionalOptions.Autofac;
using ConventionalOptions.Specs.Model;
using ExpectedObjects;
using Machine.Specifications;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ConventionalOptions.Specs.Specifications
{
    class AutofacConventionalOptionsSpecs
    {
        [Subject("Autofac")]
        class when_resolving_options_from_the_container
        {
            static ExpectedObject _expectedOptions;
            static TestOptions _actualOptions;
            static IContainer _container;

            Establish context = () =>
            {
                var configuration = new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string>
                    {
                        {"Test:StringProperty", "TestValue"},
                        {"Test:IntProperty", "2"}
                    }).Build();

                _container = new ContainerBuilder()
                    .AddOptions()
                    .RegisterOptions<TestOptions>(configuration)
                    .Build();

                _expectedOptions = new
                {
                    StringProperty = "TestValue",
                    IntProperty = 2
                }.ToExpectedObject();
            };

            Because of = () => _actualOptions = _container.Resolve<TestOptions>();

            It should_resolve_the_expected_options = () => _expectedOptions.ShouldMatch(_actualOptions);
        }

        [Subject("Autofac")]
        class when_resolving_an_auto_registered_options_from_the_container
        {
            static ExpectedObject _expectedOptions;
            static TestOptions _actualOptions;
            static IContainer _container;

            Establish context = () =>
            {
                var configuration = new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string>
                    {
                        {"Test:StringProperty", "TestValue"},
                        {"Test:IntProperty", "2"}
                    }).Build();

                _container = new ContainerBuilder()
                    .AddOptions()
                    .RegisterOptionsFromAssemblies(configuration, Assembly.GetExecutingAssembly())
                    .Build();

                _expectedOptions = new
                {
                    StringProperty = "TestValue",
                    IntProperty = 2
                }.ToExpectedObject();
            };

            Because of = () => _actualOptions = _container.Resolve<TestOptions>();

            It should_resolve_the_expected_options = () => _expectedOptions.ShouldMatch(_actualOptions);
        }

        [Subject("Autofac")]
        class when_resolving_an_auto_registered_options_wrapper_from_the_container
        {
            static ExpectedObject _expectedOptions;
            static TestOptions _actualOptions;
            static IContainer _container;

            Establish context = () =>
            {
                var configuration = new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string>
                    {
                        {"Test:StringProperty", "TestValue"},
                        {"Test:IntProperty", "2"}
                    }).Build();

                _container = new ContainerBuilder()
                    .AddOptions()
                    .RegisterOptionsFromAssemblies(configuration, Assembly.GetExecutingAssembly())
                    .Build();

                _expectedOptions = new
                {
                    StringProperty = "TestValue",
                    IntProperty = 2
                }.ToExpectedObject();
            };

            Because of = () => _actualOptions = _container.Resolve<IOptions<TestOptions>>().Value;

            It should_resolve_the_expected_options = () => _expectedOptions.ShouldMatch(_actualOptions);
        }

        [Subject("Autofac")]
        class when_resolving_an_auto_registered_options_snapshot_from_the_container
        {
            static ExpectedObject _expectedOptions;
            static TestOptions _actualOptions;
            static IContainer _container;

            Establish context = () =>
            {
                var configuration = new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string>
                    {
                        {"Test:StringProperty", "TestValue"},
                        {"Test:IntProperty", "2"}
                    }).Build();

                _container = new ContainerBuilder()
                    .AddOptions()
                    .RegisterOptionsFromAssemblies(configuration, Assembly.GetExecutingAssembly())
                    .Build();

                _expectedOptions = new
                {
                    StringProperty = "TestValue",
                    IntProperty = 2
                }.ToExpectedObject();
            };

            Because of = () => _actualOptions = _container.Resolve<IOptionsSnapshot<TestOptions>>().Value;

            It should_resolve_the_expected_options = () => _expectedOptions.ShouldMatch(_actualOptions);
        }

        [Subject("Autofac")]
        class when_resolving_an_auto_registered_options_monitor_from_the_container
        {
            static ExpectedObject _expectedOptions;
            static TestOptions _actualOptions;
            static IContainer _container;

            Establish context = () =>
            {
                var configuration = new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string>
                    {
                        {"Test:StringProperty", "TestValue"},
                        {"Test:IntProperty", "2"}
                    }).Build();

                _container = new ContainerBuilder()
                    .AddOptions()
                    .RegisterOptionsFromAssemblies(configuration, Assembly.GetExecutingAssembly())
                    .Build();

                _expectedOptions = new
                {
                    StringProperty = "TestValue",
                    IntProperty = 2
                }.ToExpectedObject();
            };

            Because of = () => _actualOptions = _container.Resolve<IOptionsMonitor<TestOptions>>().CurrentValue;

            It should_resolve_the_expected_options = () => _expectedOptions.ShouldMatch(_actualOptions);
        }
    }
}