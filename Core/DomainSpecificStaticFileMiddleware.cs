namespace PanoramicData.Extensions.DomainSpecificStaticFiles;

/// <summary>
/// The middleware for serving domain-specific static files
/// </summary>
internal class DomainSpecificStaticFileMiddleware : IMiddleware
{
	private readonly DomainSpecificStaticFileRequestHandler _handler;
	private readonly DomainSpecificStaticFileConfiguration _configuration;
	private readonly ILogger<DomainSpecificStaticFileMiddleware> _logger;

	public DomainSpecificStaticFileMiddleware(DomainSpecificStaticFileRequestHandler handler, DomainSpecificStaticFileConfiguration configuration, ILogger<DomainSpecificStaticFileMiddleware> logger)
	{
		_handler = handler;
		_configuration = configuration;
		_logger = logger;
	}

	/// <summary>
	/// Method that is invoked as part of the request pipeline
	/// </summary>
	/// <param name="context">The HttpContext of the current request</param>
	/// <param name="next">The next piece of middleware in the request pipeline</param>
	public async Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		ArgumentNullException.ThrowIfNull(context, nameof(context));
		ArgumentNullException.ThrowIfNull(next, nameof(next));

		var interceptionContext = new InterceptionContext(context, _configuration);
		if (interceptionContext.CanInterceptRequest)
		{
			// This is a request that we are supposed to intercept
			await _handler
				.InterceptRequestAsync(interceptionContext, context.RequestAborted)
				.ConfigureAwait(false);
			return;
		}

		// This is not a request we are responsible for
		LogNonInterceptedRequest(interceptionContext);
		await next.Invoke(context);
	}

	/// <summary>
	/// Log a debug message to indicate that the current request was not handled
	/// </summary>
	/// <param name="interceptionContext">The context of the interception request</param>
	private void LogNonInterceptedRequest(InterceptionContext interceptionContext)
	{
		if (_logger.IsEnabled(LogLevel.Debug))
		{
			_logger.LogDebug("DomainSpecificStaticFileMiddleware: Request not handled: {RequestUrl}", interceptionContext.RequestPath);
		}
	}
}
