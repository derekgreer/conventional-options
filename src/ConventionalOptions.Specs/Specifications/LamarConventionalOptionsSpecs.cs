﻿using System.Collections.Generic;
using System.Reflection;
using ConventionalOptions.DependencyInjection;
using ConventionalOptions.Specs.Model;
using ExpectedObjects;
using Lamar;
using Machine.Specifications;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace ConventionalOptions.Specs.Specifications
{
    class LamarConventionalOptionsSpecs
    {
        [Subject("Lamar")]
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

                _container = new Container(services =>
                {
                    services.AddSingleton<IConfiguration>(configuration);
                    services.AddOptions().RegisterOptions<TestOptions>(configuration);
                });

                _expectedOptions = new
                {
                    StringProperty = "TestValue",
                    IntProperty = 2
                }.ToExpectedObject();
            };

            Because of = () => _actualOptions = _container.GetInstance<TestOptions>();

            It should_resolve_the_expected_options = () => _expectedOptions.ShouldMatch(_actualOptions);
        }

        [Subject("Lamar")]
        class when_resolving_auto_registered_options_from_the_container
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

                _container = new Container(services =>
                {
                    services.AddSingleton<IConfiguration>(configuration);
                    services.AddOptions().RegisterOptionsFromAssemblies(Assembly.GetExecutingAssembly());
                });

                _expectedOptions = new
                {
                    StringProperty = "TestValue",
                    IntProperty = 2
                }.ToExpectedObject();
            };

            Because of = () => _actualOptions = _container.GetInstance<TestOptions>();

            It should_resolve_the_expected_options = () => _expectedOptions.ShouldMatch(_actualOptions);
        }

        [Subject("Lamar")]
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

                _container = new Container(services =>
                {
                    services.AddSingleton<IConfiguration>(configuration);
                    services.AddOptions().RegisterOptionsFromAssemblies(Assembly.GetExecutingAssembly());
                });

                _expectedOptions = new
                {
                    StringProperty = "TestValue",
                    IntProperty = 2
                }.ToExpectedObject();
            };

            Because of = () => _actualOptions = _container.GetInstance<IOptions<TestOptions>>().Value;

            It should_resolve_the_expected_options = () => _expectedOptions.ShouldMatch(_actualOptions);
        }

        [Subject("Lamar")]
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

                _container = new Container(services =>
                {
                    services.AddSingleton<IConfiguration>(configuration);
                    services.AddOptions().RegisterOptionsFromAssemblies(Assembly.GetExecutingAssembly());
                });

                _expectedOptions = new
                {
                    StringProperty = "TestValue",
                    IntProperty = 2
                }.ToExpectedObject();
            };

            Because of = () => _actualOptions = _container.GetInstance<IOptionsSnapshot<TestOptions>>().Value;

            It should_resolve_the_expected_options = () => _expectedOptions.ShouldMatch(_actualOptions);
        }

        [Subject("Lamar")]
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

                _container = new Container(services =>
                {
                    services.AddSingleton<IConfiguration>(configuration);
                    services.AddOptions().RegisterOptionsFromAssemblies(Assembly.GetExecutingAssembly());
                });

                _expectedOptions = new
                {
                    StringProperty = "TestValue",
                    IntProperty = 2
                }.ToExpectedObject();
            };

            Because of = () => _actualOptions = _container.GetInstance<IOptionsMonitor<TestOptions>>().CurrentValue;

            It should_resolve_the_expected_options = () => _expectedOptions.ShouldMatch(_actualOptions);
        }
    }
}