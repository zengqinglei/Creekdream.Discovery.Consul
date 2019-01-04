using System;

namespace Creekdream.Discovery.Consul
{
    /// <summary>
    /// 服务健康检查配置
    /// </summary>
    public class ConsulServiceOptions
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public string[] Tags { get; set; }

        /// <summary>
        /// 服务地址(空则自动获取)
        /// </summary>
        public Uri ServiceUri { get; set; }

        /// <summary>
        /// 心跳检查相对地址(默认：api/health)
        /// </summary>
        public string HealthUrl { get; set; } = "api/health";

        /// <summary>
        /// 检查间隔时间(默认：10s)
        /// </summary>
        public int? Interval { get; set; } = 10;

        /// <summary>
        /// 检查超时时间(默认：10s)
        /// </summary>
        public int? Timeout { get; set; } = 10;

        /// <summary>
        /// 失败后取消注册服务(默认：20s)
        /// </summary>
        public int? DeregisterCriticalServiceAfter { get; set; } = 20;
    }
}
