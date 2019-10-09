﻿# Nuclear ExportLocator Class Library 

ExportLocator Library is part of the Nuclear Framework set of .NET Standard helper class libraries.

# How to use

## Register IServiceLocator
This class library will register dependencies in IServiceLocator with the ExportAttribute without the need to register services in Startup.cs.
```
IServiceLocator Services = ServiceLocator.GetInstance; 
```



## Register needed assemblies
If you have Interface in one project and class implementation in other and if you decorate it with the ExportAttribute make sure that you load those assemblies in Program.cs in order for IServiceLocator to discover Exports. Add all assemblies in which you have Exported Service.

```
AppDomain.CurrentDomain.Load("Your Class Library 1");
AppDomain.CurrentDomain.Load("Your Class Library 2");
AppDomain.CurrentDomain.Load("Your Class Library 3");
...
```

### Registering Service
If you ever worked with the MEF Framework then you are familiar with this implementation. Only thing that is added is Lifetime object
```
[Export(typeof(ITestService),Lifetime = ExportLocator.Enumerations.ExportLifetime.Singleton)]
public class TestService : ITestService
```
If you dont specify the ExportLifetime
```
[Export(typeof(ITestService)]
public class TestService : ITestService
```
it will be registered as Transient. Transient is the default lifetime.

### Calling your service
One way is to assing it like (lets say you injected IServiceLocator to _serviceLocator)
```
ITestService _testService = _serviceLocator.Get<ITestService>();
```
but you call also use methods directly from IServiceLocator
```
var someresult = _serviceLocator.Get<ITestService>().SomeMethodFromITestService();
```

## Known issues
Current version does not support EntityFrameworkCore DbContext with the Constructor which has parameters for options.
In case you still want to Export DbContext as a Service you need to override OnConfiguring method and leave constructor parameterless.

# Authors
Nikola Milinkovic - *initial work and maintainer* - [Mycenaean](https://github.com/Mycenaean)
