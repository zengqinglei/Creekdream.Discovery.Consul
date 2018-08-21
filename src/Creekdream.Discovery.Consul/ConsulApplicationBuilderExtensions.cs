using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Creekdream.Discovery.Consul
{
    /// <inheritdoc />
    public static class ConsulApplicationBuilderExtensions
    {
        /// <summary>
        /// 注册服务、心跳检测
        /// </summary>
        public static void UseConsul(this IApplicationBuilder app, IConfigurationSection consulServiceConfiguration)
            => app.UseConsul(consulServiceConfiguration.Get<ConsulServiceOptions>());

        /// <summary>
        /// 注册服务、心跳检测
        /// </summary>
        public static void UseConsul(this IApplicationBuilder app, Action<ConsulServiceOptions> config)
        {
            var options = new ConsulServiceOptions();
            config.Invoke(options);

            app.UseConsul(options);
        }

        /// <inheritdoc />
        private static void UseConsul(this IApplicationBuilder app, ConsulServiceOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var lifetime = app.ApplicationServices.GetService<IApplicationLifetime>();
            var consulClient = app.ApplicationServices.GetService<IConsulClient>();
            if (options.ServiceUri == null)
            {
                var addresses = app.ServerFeatures.Get<IServerAddressesFeature>();
                var address = addresses.Addresses.First();
                var ip = GetLocalIPAddress();
                if (!string.IsNullOrEmpty(ip))
                {
                    var ipAddress = new Uri(address);
                    options.ServiceUri = new Uri($"{ipAddress.Scheme}://{ip}:{ipAddress.Port}");
                }
            }

            var httpCheck = new AgentServiceCheck()
            {
                HTTP = new Uri(options.ServiceUri, options.HealthUrl).AbsoluteUri
            };
            if (options.DeregisterCriticalServiceAfter.HasValue)
            {
                httpCheck.DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(options.DeregisterCriticalServiceAfter.Value);
            }
            if (options.Interval.HasValue)
            {
                httpCheck.Interval = TimeSpan.FromSeconds(options.Interval.Value);
            }
            if (options.Timeout.HasValue)
            {
                httpCheck.Timeout = TimeSpan.FromSeconds(options.Timeout.Value);
            }

            var registration = new AgentServiceRegistration()
            {
                Checks = new[] { httpCheck },
                ID = Guid.NewGuid().ToString(),
                Name = options.ServiceName,
                Tags = options.Tags,
                Address = options.ServiceUri.Host,
                Port = options.ServiceUri.Port
            };

            consulClient.Agent.ServiceRegister(registration).Wait();
            lifetime.ApplicationStopping.Register(() =>
            {
                consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            });
        }

        /// <summary>
        /// 获取本地IP地址
        /// </summary>
        private static string GetLocalIPAddress()
        {
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var network in networkInterfaces)
            {
                if (network.OperationalStatus != OperationalStatus.Up)
                    continue;
                var properties = network.GetIPProperties();
                if (properties.GatewayAddresses.Count == 0)
                    continue;

                foreach (var address in properties.UnicastAddresses)
                {
                    if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                        continue;
                    if (IPAddress.IsLoopback(address.Address))
                        continue;
                    return address.Address.ToString();
                }
            }
            return "127.0.0.1";
        }
    }
}
