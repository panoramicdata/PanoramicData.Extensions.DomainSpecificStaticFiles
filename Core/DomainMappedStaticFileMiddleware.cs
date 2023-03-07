using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace PanoramicData.Extensions.DomainMappedStaticFiles;

/// <summary>
/// The middleware for serving whitelabelled static files
/// </summary>
internal class DomainMappedStaticFileMiddleware : IMiddleware
{
	private readonly DomainMappedStaticFileRequestHandler _handler;
	private readonly DomainMappedStaticFileOptions _options;
	private readonly ILogger<DomainMappedStaticFileMiddleware> _logger;

	public DomainMappedStaticFileMiddleware(DomainMappedStaticFileRequestHandler handler, IOptions<DomainMappedStaticFileOptions> options, ILogger<DomainMappedStaticFileMiddleware> logger)
	{
		_handler = handler;
		_options = options.Value;
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

		var interceptionContext = new InterceptionContext(context, _options);
		if (interceptionContext.CanInterceptRequest)
		{
			// This is a request that we are supposed to intercept
			await _handler
				.InterceptRequestAsync(interceptionContext)
				.ConfigureAwait(false);
			return;
		}

		// This is not a request we are responsible for
		await next.Invoke(context);
	}
}
