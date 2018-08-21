using System;

namespace Creekdream.Discovery.Consul
{
    /// <summary>
    /// Consul 客户端配置
    /// </summary>
    public class ConsulClientOptions
    {
        /// <summary>
        /// Consul 服务地址
        /// </summary>
        public Uri Address { get; set; }

        /// <summary>
        /// Consul 数据中心
        /// </summary>
        public string Datacenter { get; set; }

        /// <summary>
        /// Consul Token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 等待时间(秒)
        /// </summary>
        public long? WaitTime { get; set; }
    }
}
