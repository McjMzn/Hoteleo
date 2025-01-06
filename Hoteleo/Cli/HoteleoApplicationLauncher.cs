using Hoteleo.Cli.Commands;
using Hoteleo.Cli.Generic;
using Hoteleo.Domain.Repositories;
using Hoteleo.Domain.Services;
using Hoteleo.Utilities;
using Hoteleo.Utilities.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hoteleo.Cli
{
    internal partial class HoteleoApplicationLauncher
    {
        private readonly string[] _arguments;

        public HoteleoApplicationLauncher(string[] arguments)
        {
            _arguments = arguments;
        }

        public int Launch(Action<IServiceCollection> configureServiceCollection = null, CancellationToken cancellationToken = default)
        {
            CliApplication application;
            
            try
            {
                var applicationConfiguration = LoadApplicationConfiguration();
                var serviceProvider = BuildServiceProvider(applicationConfiguration, configureServiceCollection);
                application = serviceProvider.GetRequiredService<CliApplication>();
            }
            catch
            {
                return ExitCode.StartupError;
            }
            
            return application.Run(cancellationToken);
        }

        private HoteleoApplicationConfiguration LoadApplicationConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder();
            var argumentMapping = new Dictionary<string, string>
            {
                ["--hotels"] = nameof(IJsonHotelsRepositoryConfiguration.HotelsJsonFilePath),
                ["--bookings"] = nameof(IJsonBookingsRepositoryConfiguration.BookingsJsonFilePath)
            };

            configurationBuilder.AddCommandLine(_arguments, argumentMapping);

            var applicationConfiguration = new HoteleoApplicationConfiguration();
            configurationBuilder.Build().Bind(applicationConfiguration);

            return applicationConfiguration;
        }

        private IServiceProvider BuildServiceProvider(HoteleoApplicationConfiguration applicationConfiguration, Action<IServiceCollection> configureServiceCollection = null)
        {
            var services = new ServiceCollection();

            // Configuration
            services.AddSingleton<IJsonBookingsRepositoryConfiguration>(applicationConfiguration);
            services.AddSingleton<IJsonHotelsRepositoryConfiguration>(applicationConfiguration);

            // Environment
            services.AddSingleton<ISystemConsole, SystemConsole>();
            services.AddSingleton<IFileSystem, FileSystem>();
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            services.AddSingleton<IJsonSerializer, JsonSerializer>();

            // Domain
            services.AddSingleton<IBookingsRepository, JsonBookingsRepository>();
            services.AddSingleton<IHotelsRepository, JsonHotelsRepository>();
            services.AddSingleton<IBookingsAnalyzisService, BookingsAnalyzisService>();
            services.AddSingleton<IRoomFindingService, RoomFindingService>();

            // Application
            services.AddSingleton<ICliApplicationCommand, ExitCommand>();
            services.AddSingleton<ICliApplicationCommand, SearchCommand>();
            services.AddSingleton<ICliApplicationCommand, AvailabilityCommand>();
            services.AddSingleton<CliApplication>();

            // Overrides
            configureServiceCollection?.Invoke(services);

            return services.BuildServiceProvider();
        }
    }
}
