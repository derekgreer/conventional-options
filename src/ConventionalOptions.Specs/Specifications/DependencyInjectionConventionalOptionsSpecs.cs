using System.Collections.Generic;
using System.Reflection;
using ConventionalOptions.DependencyInjection;
using ConventionalOptions.Specs.Model;
using ExpectedObjects;
using Machine.Specifications;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace ConventionalOptions.Specs.Specifications
{
    class DependencyInjectionConventionalOptionsSpecs
    {
        [Subject("DotNetCore DependencyInjection")]
        class when_resolving_options_from_the_container
        {
            static ExpectedObject _expectedOptions;
            static TestOptions _actualOptions;
            static ServiceProvider _container;

            Establish context = () =>
            {
                var configuration = new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string>
                    {
                        {"Test:StringProperty", "TestValue"},
                        {"Test:IntProperty", "2"}
                    }).Build();

                var services = new ServiceCollection();
                services.AddSingleton<IConfiguration>(configuration);
                services.AddOptions();
                services.RegisterOptions<TestOptions>(configuration);
                _container = services.BuildServiceProvider();

                _expectedOptions = new
                {
                    StringProperty = "TestValue",
                    IntProperty = 2
                }.ToExpectedObject();
            };

            Because of = () => _actualOptions = _container.GetService<TestOptions>();

            It should_resolve_the_expected_options = () => _expectedOptions.ShouldMatch(_actualOptions);
        }

        [Subject("DotNetCore DependencyInjection")]
        class when_resolving_auto_registered_options_from_the_container
        {
            static ExpectedObject _expectedOptions;
            static TestOptions _actualOptions;
            static ServiceProvider _container;

            Establish context = () =>
            {
                var configuration = new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string>
                    {
                        {"Test:StringProperty", "TestValue"},
                        {"Test:IntProperty", "2"}
                    }).Build();

                var services = new ServiceCollection();
                services.AddSingleton<IConfiguration>(configuration);
                services.AddOptions();
                services.RegisterOptionsFromAssemblies(Assembly.GetExecutingAssembly());
                _container = services.BuildServiceProvider();

                _expectedOptions = new
                {
                    StringProperty = "TestValue",
                    IntProperty = 2
                }.ToExpectedObject();
            };

            Because of = () => _actualOptions = _container.GetService<TestOptions>();

            It should_resolve_the_expected_options = () => _expectedOptions.ShouldMatch(_actualOptions);
        }
    }
}