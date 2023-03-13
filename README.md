# PanoramicData.Extensions.DomainSpecificStaticFiles
Middleware for serving Domain-specific static files from ASP.NET websites, including for the purposes of white labelling sites.

## Usage
There are two tasks to do in startup of an ASP.NET Core site to make this middleware function as intended.

### Step 1 - Add services (and the associated config)
Add the services that are used by the subsystem into DI in Program.cs (or Startup.cs in older systems) using code similar to the following:

```
builder.Services.AddDomainSpecificStaticFiles(builder.Configuration, ApplyDomainSpecificStaticFileOptions);
```

Here we make use of the AddDomainSpecificStaticFiles extension method to add the necessary services, as well as defining the action that is 
used to apply modifications to the options on which the interception of requests is based.

In this example we have separated out the action that is used to apply configuration into a separate method, the code of which is seen below. 
However, it is not a requirement that you separate these as shown; you can specify the configuration inline using the lambda syntax if you prefer.
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
### Step 2 - add the middleware to the pipeline
```
app.UseDomainSpecificStaticFiles();
```
The order in which pipline registration methods are called is important, as the order of execution of middleware is determined by the 
order in which they are specified at startup. If we want to serve files only for users who are authenticated then this call should come 
after the call to UseAuthorization(). However, if files are to be served to all users, even those who are unauthenticated, we should 
add our static file middleware before the application of authorization.

It is common for Domain-specific static files to be treated in the same way as regular static files. As a result, it is common
for UseDomainSpecificStaticFiles() to be called immediately after calling UseStaticFiles(), as shown below:
```
// In this example, static files do not have any authorization applied.
app.UseStaticFiles();
app.UseDomainSpecificStaticFiles();

app.UseRouting();

// Turn on authentication
app.UseAuthentication();
app.UseAuthorization();
```
Alternatively, to force users to authenticate before they can be served Domain-specific static files, instead call UseDomainSpecificStaticFiles() 
after you call UseAuthorization(), as shown in this alternative code sample:
```
app.UseStaticFiles();

app.UseRouting();

// Turn on authentication
app.UseAuthentication();
app.UseAuthorization();

// In this example, we restrict Domain-specific static files to users who are logged in
app.UseDomainSpecificStaticFiles();
```
Use the correct code sample to meet your requirements.
> CAUTION: Consider your authentication requirements carefully. Failure to apply the correct rules could result in the exposure 
> of sensitive information to unauthenticated users.
