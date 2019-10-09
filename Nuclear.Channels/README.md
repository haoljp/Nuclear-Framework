﻿# Nuclear Channels

Channels Library is part of the Nuclear Framework set of .NET Standard class libraries used to build lightweight API Endpoints using HttpListener class. 

# How to use

Decorate class in which you have methods you want to expose as endpoints with ChannelAttribute and methods you want to expose as API endpoints with ChannelMethodAttribute. Channels Library is using ExportLocator for dependency injection. Every service from this Library can be injected from IServiceLocator.

## ChannelAttribute

```
[Channel]
public class TestChannel
{
	...
}
```
This code will make base route for all ChannelMethods in ~/channels/TestChannel/. In case you want to change this route you can provide Name property of the ChannelAttribute.
```
[Channel(Name = "CustomChannel")]
public class TestChannel
{
	...
}
```
Route for this will be ~/channels/CustomChannel/ instead of ~/channels/TestChannel/.
Other property of the ChannelAttribute is Description property which when provided will generate Description for the builtin documentation tool.
```
[Channel(Description = "Test Channel Description")]
```
If you want to make global authorization for targeted channel you can provide AuthorizeChannelAttribute beneath ChannelAttribute.

```
[Channel]
[AuthorizeChannel(Schema = AuthenticationSchemes.Basic)]
public class TestChannel
{
	...
}
```

## ChannelMethodAttribute

Now that you exposed class as a Channel you need to provide endpoints for your methods.

```
[ChannelMethod]
public string HelloWorld()
{
	return "Hello World from ChannelMethod";
}
```
This method will be exposed as an API Endpoint on ~/channels/{Name of your channel}/HelloWorld/.

### HttpMethod GET
	
If you dont specify HttpMethod property of the ChannelMethod , it will accept both GET and POST requests.
```
[ChannelMethod(HttpMethod = ChannelHttpMethod.GET)]
public string HelloWorld()
{
	return "Hello World from ChannelMethod";
}
```
If you want to provide parameters with GET as usual you need to provide them in QueryString
```
[ChannelMethod(HttpMethod = ChannelHttpMethod.GET)]
public string HelloWorld(string name)
{
	return $"Hello World {name} from ChannelMethod";
}
```
~/channels/{Name of you channel}/HelloWorld/?name=Nikola. Important thing is that the parameter in the QueryString must match parameter in the ChannelMethod. Its case insensitive

### HttpMethod POST

```
[ChannelMethod(HttpMethod = ChannelHttpMethod.POST)]
public string HelloWorld(string name)
{
	return $"Hello World {name} from ChannelMethod";
}
```
For this route will be the same ~/channels/{Name of you channel}/HelloWorld/ but the input body must be in xml format.

```
<channels>
	<name>nikola</name>
</channels>
```
For complex entities is the same.
```
[ChannelMethod(HttpMethod = ChannelHttpMethod.POST)]
public SomeEntity EntityMethod(SomeEntity entity)
{
	return entity;
}
 ```

 ```
<channels>
	<SomeEntity>
		<Id>1</Id>
		<Name>Your Name</Name>
		...
	</SomeEntity>
</channels>
```
Other properties are Description and Schema. Both are the same as for ChannelAttribute , Description if provided will autogenerate description for builtin tool for documentation. AuthenticationSchema will enforce desired authentication.

```
[ChannelMethod(HttpMethod = ChannelHttpMethod.POST,Schema = AuthenticationSchemes.Basic,Description = "EntityMethod Description")]
public SomeEntity EntityMethod(SomeEntity entity)
{
	return entity;
}
```

## Full Example
```
[Channel]
public class TestChannel
{
        [ChannelMethod]
        public string HelloWorld()
        {
            return "Hello World from ChannelMethod";
        }

        [ChannelMethod(ChannelHttpMethod.GET)]
        public string Hello(string name)
        {
            return $"Hello {name} from Hello ChannelMethod";
        }

	[ChannelMethod(HttpMethod = ChannelHttpMethod.POST)]
	public SomeEntity EntityMethod(SomeEntity entity)
	{
		return entity;
	}
}
```

# Documentation tool

Documentation for channels is autogenerated by IChannelDocumentationService.
```
IServiceLocator Services = ServiceLocator.GetInstance;
IChannelDocumentationService _documentationService = Services.Get<IChannelDocumentationService>().GetDocumentation(AppDomain.CurrentDomain);
```

# Initializing Channels

You can test and initialize Channels in both Console Apps and Web Apps. To initialize it internaly as a Windows Service you can create Console app.
```
    class Program
    {
        static void Main(string[] args)
        {
            IChannelHost host = ChannelHost.GetHost;
            host.LoadAssemblies(AppDomain.CurrentDomain, null);
            host.StartHosting(null);

            Console.ReadLine();
        }
    }
```
IChannelHost service will start the hosting of the channels. In Web apps just apply same 3 lines of code in Program.cs.

# Authors
 Nikola Milinkovic - *initial work and maintainer* - [Mycenaean](https://github.com/Mycenaean)

# License
 Nuclear.Channels are MIT Licensed. Check [License](https://github.com/Mycenaean/Nuclear-Framework/LICENSE).

# Known Issues
Sending JSON instead of XML does not work for now.