# PanoramicData.Extensions.DomainSpecificStaticFiles

Middleware for serving domain-specific static files from ASP.NET Core websites, including for the purposes of white labelling sites.

## Usage

There are two tasks to do during the startup of an ASP.NET Core site to make this middleware function as intended.

### Step 1 - Add services (and, optionally, the associated config)

Add the services that are used by the subsystem into DI in Program.cs (or Startup.cs in older systems) using code similar to the following:

```
builder.Services.AddDomainSpecificStaticFiles(builder.Configuration, ApplyDomainSpecificStaticFileOptions);
```

Here we make use of the AddDomainSpecificStaticFiles extension method to add the necessary services, as well as defining the action that is 
used to apply modifications to the options on which the interception of requests is based.

In this example we have separated out the action that is used to apply configuration into a separate custom method named 
ApplyDomainSpecificStaticFileOptions, the code of which is seen below. However, it is not a requirement that you separate these as shown; 
you can specify the configuration inline using the lambda syntax if you prefer.

```
static void ApplyDomainSpecificStaticFileOptions(DomainSpecificStaticFileOptions options)
	=> options.RequestMappings.Add(
		new()
		{
			RelativeUrlPrefix = "style",
			ResourcePathPrefix = "Style",
			DomainMappings = new()
			{
				new DomainMapping { Domain = "localhost", ResourceSubPath = "Default" }
			}
		});
```

Version 1.1 of the package introduces the ability to load configuration asynchronously from a data store, so there are now 3 overloads 
of the `AddDomainSpecificStaticFiles` method. The options for loading configuration are detailed towards the end of this document.

### Step 2 - add the middleware to the pipeline

Next, we must include the middleware in the pipeline in the correct place.

```
app.UseDomainSpecificStaticFiles();
```

The order in which pipline registration methods are called is important, as the order of execution of middleware is determined by the 
order in which they are specified at startup. If we want to serve files only for users who are authenticated then this call should come 
after the call to `UseAuthorization()`. However, if files are to be served to all users, even those who are unauthenticated, we should 
add our static file middleware before the application of authorization.

It is common for domain-specific static files to be treated in the same way as regular static files. As a result, it is common
for `UseDomainSpecificStaticFiles()` to be called immediately after calling `UseStaticFiles()`, as shown below:

```
// In this example, static files do not have any authorization applied.
app.UseStaticFiles();
app.UseDomainSpecificStaticFiles();

app.UseRouting();

// Turn on authentication
app.UseAuthentication();
app.UseAuthorization();
```

Alternatively, to force users to authenticate before they can be served Domain-specific static files, instead call 
`UseDomainSpecificStaticFiles()` **after** you call `UseAuthorization()`, as shown in this alternative code sample:

```
app.UseStaticFiles();

app.UseRouting();

// Turn on authentication
app.UseAuthentication();
app.UseAuthorization();

// In this example, we restrict Domain-specific static files to users who are logged in
app.UseDomainSpecificStaticFiles();
```

Use the correct code sample to meet your specific requirements.

> CAUTION: Consider your authentication requirements carefully. Failure to apply the correct rules could 
> result in the exposure of sensitive information to unauthenticated users!

## Configuration loading options

Version 1.1 of the package enhances the options available to you for loading mapping configuration. This is achieved 
by exposing 3 overloads of the `AddDomainSpecificStaticFiles` method. Understanding these 3 options is key to making 
best use of the package in your specific circumstances.

### 1. Static configuration in code

The first overload of the `AddDomainSpecificStaticFiles` method takes static, hard-coded configuration, and is the 
simplest to use. This is the method shown in the first section, and used in the demo.

### 2. Async loading of configuration from a data store (no refreshes)

The second overload of the `AddDomainSpecificStaticFiles` method takes as its parameter a 
`Func<Task<DomainSpecificStaticFileOptions>>` which is the code to be called to load configuration once the system 
has started. The loading method is asynchronous to support accessing the config from a slow, external data store, 
and is delayed until application startup has completed to ensure that your data store has finished initializing.

Prior to loading the config, the config is empty, resulting in no static files being served. Therefore, you should 
aim to optimise loading of config as much as possible, to reduce to a minimum the period for which there is no 
config available.

### 3. Customised async loading (optionally with refreshes/reloading)

The third overload of the `AddDomainSpecificStaticFiles` method takes as its parameter an instance of a class 
inheriting from `MappingConfigurationProviderBase`. This class offers the asynchronous startup and loading of the
configuration system, but enables more flexibility, including the ability to reload configuration during the 
application's lifetime. The implementation of the `InitializeAsync` method that the implementing class is required 
to override is called to load the configuration once the system has started. The loading method is asynchronous to 
support accessing the config from a slow, external data store, and is delayed until startup has completed to ensure 
that your data store initialization will have completed.

One public implementation of `MappingConfigurationProviderBase` is provided, namely 
`RefreshingMappingConfigurationProvider`. This class can be used to call your asynchronous loading code to a 
predetermined schedule, with the refresh period specified as a parameter of the constructor.

A custom implementation inheriting from `MappingConfigurationProviderBase` can be provided if you prefer.
This enables you to implement a custom caching scheme making use of cache invalidation to reload configuration 
as quickly as possible after a change is detected, if you consider this useful. Custom implementations should call 
the `ApplyConfiguration` method on the base class to apply the new configuration to the underlying subsystem.

Developers should note that their code is not directly called by the subsystem to retrieve configuration on each 
request. The middleware in this package sits on a very hot path in the request pipeline, and it is therefore 
inappropriate to allow developers to run a lot of code during the request. Instead, `MappingConfigurationProviderBase` 
is responsible for delivering the configuration on request, and developers of custom providers are required to call 
the `ApplyConfiguration` method on the base class to have it swap out its configuration with an updated set in a 
highly optimised, thread-safe way. This affects the design of your provider; you are responsible for wiring up 
tasks to complete any future loading/reloading operations as part of the work done in the `InitializeAsync` method.

Prior to loading the config, the config is empty, resulting in no static files being served. Therefore, you should 
aim to optimise loading of config as much as possible, to reduce to a minimum the period for which there is no config 
available.

## Overriding mappings during development

If you are using the static configuration loading approach - option 1 above - you can override that static, hard-coded 
configuration by creating entries in the .NET Core configuration subsystem (appSettings.json, secrets.json, environment 
variables and so on). Note that this only applies to the static configuration method, as any other mechanism is likely 
to draw it from an environment-specific data store, where loading from configuration would be less useful, and would 
significantly complicate the asynchronous nature of the loading of configuration.

Given the use of static configuration detailed above, the component will load request mappings from configuration, 
from an element called DomainMappings, if it is available. Any configuration found overrides/replaces the request 
mappings defined in code. This provides developers the ability to force a specific set of resources to be used in 
development, where it is likely that the application is hosted on localhost.

Loading override mappings from configuration resolves a problem that is somewhat specific to development. In test 
and production environments, it is common to have multiple DNS entries resolving to the same website with different 
hosts, allowing easy identification of each. However, in development this is slightly less common, and less convenient. 
Some teams do not have their own DNS servers, which would force any dev-only DNS entries to be specified on each 
developer's machine. This introduces a maintenance overhead that you might wish to avoid. Configuration offers an 
alternative approach to enable developer testing of a website with a specific set of resources served, even when hosting 
all of the code on localhost.

Here is an example config section showing the current format of the override configuration:

```
  "DomainMappings": {
    "RequestMappings": [
      {
        "RelativeUrlPrefix": "/style/",
        "ResourcePathPrefix": "Style",
        "DomainMappings": [
          {
            "Domain": "localhost",
            "ResourceSubPath": "Alternative"
          }
        ]
      }
    ]
  }
```

This override, when placed into one of the configuration files (such as the user secrets file) will change the mapping 
for localhost from 'Default' (defined in code in the ApplyDomainSpecificStaticFileOptions method shown above) to 
'Alternative', allowing temporary testing of the site using the alternative set of resources.

The user secrets file is an ideal place to configure temporary overrides, as it is not committed to source control. 
This allows individual developers in a team to work independently without affecting others by accidentally committing 
changes that were intended to affect them alone.

The demo website includes userSecrets.example.json which can be copied and pasted into user secrets as a basis for your 
own custom override configuration.

At this time, only request mapping overrides are loaded from configuration. All other options must be specified in code.

## Proxies/load balancers

If your website is hosted behind a proxy/load balancer e.g. Azure FrontDoor, you may need to configure the middleware 
to use the X-Forwarded-Host header to present the correct host values to the DomainSpecificStaticFiles middleware.

Configure the middleware to use the X-Forwarded-Host header by adding the following code:

```
// Configure headers that are forwarded
builder.Services.Configure<ForwardedHeadersOptions>(
	options =>
	{
		options.ForwardedHeaders =
				ForwardedHeaders.XForwardedFor
				| ForwardedHeaders.XForwardedProto
				| ForwardedHeaders.XForwardedHost;
		// Need to clear out restrictions, (FrontDoor overrides the value of x-forwarded-host if one was)
		// provided explicitly by the client
		// https://learn.microsoft.com/en-us/azure/frontdoor/front-door-http-headers-protocol#from-the-front-door-to-the-backend
		// https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer?view=aspnetcore-7.0#configuration-for-a-proxy-that-uses-different-header-names
		options.KnownNetworks.Clear();
		options.KnownProxies.Clear();

		// This will restrict the allowed values coming in through the x-forwarded-host header
		//options.AllowedHosts = new List<string>() { "yourallowed.domain.name" };
	});

...
```

Then add the a call to `app.UseForwardedHeaders()` e.g.

```
// This allows the host to be set from the x-forwarded-host etc so we can see the original host after
// coming through the proxy and arriving at the application
// Place this code before you need to access the host header, e.g. before app.UseHsts
app.UseForwardedHeaders();

if (app.Environment.IsDevelopment())
{
	app.UseWebAssemblyDebugging();
}
else
{
	_ = app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	_ = app.UseHsts();
}

```