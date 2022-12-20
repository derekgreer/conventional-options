using System.Collections.Generic;
using System.Reflection;
using ConventionalOptions.Ninject;
using ConventionalOptions.Specs.Model;
using ExpectedObjects;
using Machine.Specifications;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Ninject;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace ConventionalOptions.Specs.Specifications
{
    class NinjectConventionalOptionsSpecs
    {
        [Subject("Ninject")]
        class when_resolving_options_from_the_container
        {
            static ExpectedObject _expectedOptions;
            static TestOptions _actualOptions;
            static IKernel _container;

            Establish context = () =>
            {
                var configuration = new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string>
                    {
                        {"Test:StringProperty", "TestValue"},
                        {"Test:IntProperty", "2"}
                    }).Build();

                _container = new StandardKernel();
                _container.Bind<IConfiguration>().ToConstant(configuration);
                _container.AddOptions().RegisterOptions<TestOptions>();

                _expectedOptions = new
                {
                    StringProperty = "TestValue",
                    IntProperty = 2
                }.ToExpectedObject();
            };

            Because of = () => _actualOptions = _container.Get<TestOptions>();

            It should_resolve_the_expected_options = () => _expectedOptions.ShouldMatch(_actualOptions);
        }

        [Subject("Ninject")]
        class when_resolving_auto_registered_options_from_the_container
        {
            static ExpectedObject _expectedOptions;
            static TestOptions _actualOptions;
            static IKernel _container;

            Establish context = () =>
            {
                var configuration = new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string>
                    {
                        {"Test:StringProperty", "TestValue"},
                        {"Test:IntProperty", "2"}
                    }).Build();

                _container = new StandardKernel();
                _container.Bind<IConfiguration>().ToConstant(configuration);
                _container.AddOptions().RegisterOptionsFromAssemblies(Assembly.GetExecutingAssembly());

                _expectedOptions = new
                {
                    StringProperty = "TestValue",
                    IntProperty = 2
                }.ToExpectedObject();
            };

            Because of = () => _actualOptions = _container.Get<TestOptions>();

            It should_resolve_the_expected_options = () => _expectedOptions.ShouldMatch(_actualOptions);
        }

        [Subject("Ninject")]
        class when_resolving_an_auto_registered_options_wrapper_from_the_container
        {
            static ExpectedObject _expectedOptions;
            static TestOptions _actualOptions;
            static IKernel _container;

            Establish context = () =>
            {
                var configuration = new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string>
                    {
                        {"Test:StringProperty", "TestValue"},
                        {"Test:IntProperty", "2"}
                    }).Build();

                _container = new StandardKernel();
                _container.Bind<IConfiguration>().ToConstant(configuration);
                _container.AddOptions().RegisterOptionsFromAssemblies(Assembly.GetExecutingAssembly());

                _expectedOptions = new
                {
                    StringProperty = "TestValue",
                    IntProperty = 2
                }.ToExpectedObject();
            };

            Because of = () => _actualOptions = _container.Get<IOptions<TestOptions>>().Value;

            It should_resolve_the_expected_options = () => _expectedOptions.ShouldMatch(_actualOptions);
        }

        [Subject("Ninject")]
        class when_resolving_an_auto_registered_options_snapshot_from_the_container
        {
            static ExpectedObject _expectedOptions;
            static TestOptions _actualOptions;
            static IKernel _container;

            Establish context = () =>
            {
                var configuration = new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string>
                    {
                        {"Test:StringProperty", "TestValue"},
                        {"Test:IntProperty", "2"}
                    }).Build();

                _container = new StandardKernel();
                _container.Bind<IConfiguration>().ToConstant(configuration);
                _container.AddOptions().RegisterOptionsFromAssemblies(Assembly.GetExecutingAssembly());

                _expectedOptions = new
                {
                    StringProperty = "TestValue",
                    IntProperty = 2
                }.ToExpectedObject();
            };

            Because of = () => _actualOptions = _container.Get<IOptionsSnapshot<TestOptions>>().Value;

            It should_resolve_the_expected_options = () => _expectedOptions.ShouldMatch(_actualOptions);
        }

        [Subject("Ninject")]
        class when_resolving_an_auto_registered_options_monitor_from_the_container
        {
            static ExpectedObject _expectedOptions;
            static TestOptions _actualOptions;
            static IKernel _container;

            Establish context = () =>
            {
                var configuration = new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string>
                    {
                        {"Test:StringProperty", "TestValue"},
                        {"Test:IntProperty", "2"}
                    }).Build();

                _container = new StandardKernel();
                _container.Bind<IConfiguration>().ToConstant(configuration);
                _container.AddOptions().RegisterOptionsFromAssemblies(Assembly.GetExecutingAssembly());

                _expectedOptions = new
                {
                    StringProperty = "TestValue",
                    IntProperty = 2
                }.ToExpectedObject();
            };

            Because of = () => _actualOptions = _container.Get<IOptionsMonitor<TestOptions>>().CurrentValue;

            It should_resolve_the_expected_options = () => _expectedOptions.ShouldMatch(_actualOptions);
        }
    }
}