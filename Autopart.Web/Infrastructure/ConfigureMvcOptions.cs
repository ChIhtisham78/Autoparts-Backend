using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Options;
using Autopart.API.Filters;

namespace Autopart.API.Infrastructure
{
    internal class ConfigureMvcOptions : IConfigureOptions<MvcOptions>
    {
        private readonly IHttpRequestStreamReaderFactory _readerFactory;
        private readonly ILoggerFactory _loggerFactory;

        public ConfigureMvcOptions(
            IHttpRequestStreamReaderFactory readerFactory,
            ILoggerFactory loggerFactory
        )
        {
            _readerFactory = readerFactory;
            _loggerFactory = loggerFactory;
        }

        public void Configure(MvcOptions options)
        {
            options.RespectBrowserAcceptHeader = true;
            options.Filters.Add<WrapResponseResultFilter>();
            options.Filters.Add<HttpGlobalExceptionFilter>();
        }
    }
}
