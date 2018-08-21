using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Creekdream.Discovery.Consul
{
    /// <inheritdoc />
    public static class ConsulServiceCollectionExtensions
    {
        /// <summary>
        /// 添加Consul服务客户端
        /// </summary>
        public static void AddConsul(this IServiceCollection services, IConfigurationSection consulClientConfiguration)
            => services.AddConsul(consulClientConfiguration.Get<ConsulClientOptions>());

        /// <summary>
        /// 添加Consul服务客户端
        /// </summary>
        public static void AddConsul(this IServiceCollection services, Action<ConsulClientOptions> config)
        {
            var clientOptions = new ConsulClientOptions();
            config.Invoke(clientOptions);

            services.AddConsul(clientOptions);
        }

        /// <inheritdoc />
        private static void AddConsul(this IServiceCollection services, ConsulClientOptions clientOptions)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (clientOptions == null)
            {
                throw new ArgumentNullException(nameof(clientOptions));
            }

            var consulClient = new ConsulClient(
                clientConfig =>
                {
                    clientConfig.Address = clientOptions.Address;
                    clientConfig.Datacenter = clientOptions.Datacenter;
                    clientConfig.Token = clientOptions.Token;
                    if (clientOptions.WaitTime.HasValue)
                    {
                        clientConfig.WaitTime = TimeSpan.FromSeconds(clientOptions.WaitTime.Value);
                    }
                });

            services.AddSingleton<IConsulClient>(consulClient);
        }
    }
}
