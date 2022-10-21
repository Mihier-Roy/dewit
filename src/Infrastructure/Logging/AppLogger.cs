using Dewit.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Dewit.Infrastructure.Logging
{
	public class AppLogger<T> : IAppLogger<T>
	{
		private readonly ILogger<T> _logger;

		public AppLogger(ILoggerFactory loggerFactory)
		{
			_logger = loggerFactory.CreateLogger<T>();
		}

		public void LogInformation(string? message, params object?[] args)
		{
			_logger.LogInformation(message, args);
		}

		public void LogError(string? message, params object?[] args)
		{
			_logger.LogError(message, args);
		}
	}
}