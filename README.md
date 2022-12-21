# Conventional Options 
[![Conventional Commits](https://img.shields.io/badge/Conventional%20Commits-1.0.0-yellow.svg)](https://conventionalcommits.org)


ConventionalOptions is a convenience library for working with Microsoft's .Net Core configuration API.

The goal of ConventionalOptions is to simplify creation and use of POCO option types.


## Quickstart

### Step 1
Install ConventionalOptions for the target DI container:

```
$> nuget install ConventionalOptions.DependencyInjection
```

### Step 2
Add Microsoft's Options feature and register option types:

```
services.AddOptions();
services.RegisterOptionsFromAssemblies(Assembly.GetExecutingAssembly());
```

### Step 3
Create an Options class:

```csharp
    public class OrderServiceOptions
    {
        public string StringProperty { get; set; }
        public int IntProperty { get; set; }
    }
```

### Step 4
Provide a corresponding configuration section (e.g. in appsettings.json):

```json
{
  "OrderService": {
    "StringProperty": "Some value",
    "IntProperty": 42
  }
}
```

### Step 5
Inject the options into types resolved from the container:

```csharp
    public class OrderService
    {
        public OrderService(OrderServiceOptions options)
        {
            // ... use options
        }
    }
```

That's it!


For more examples, see the [documentation](https://github.com/derekgreer/conventional-options/wiki) or [browse the specifications](https://github.com/derekgreer/conventional-options/tree/master/src/ConventionalOptions.Specs).
