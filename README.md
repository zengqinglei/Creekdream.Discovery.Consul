### 说明

安装包结合Consul服务，实现了以下功能：
* 向Consul注册服务，并自动获取服务IP地址(也可配置)
* 向Consul注册健康检查地址,自动获取IP地址(也可配置)

**[升级日志](https://github.com/zengqinglei/Creekdream.Discovery.Consul/releases)**

##### 安装SDK
```
Install-Package Creekdream.Discovery.Consul
```
##### 初始化配置
```
// appsettings.json 配置如下：
{
  "ConsulClient": {
    "Address": "http://192.168.0.103:8500"
    // 其他配置请参考：ConsulClientOptions.cs
  },
  "ConsulService": {
    "ServiceName": "UserService"
    // 其他配置请参考：ConsulServiceOptions.cs
  }
}
```

```
// ConfigureServices 增加如下配置：
services.AddConsul(_configuration.GetSection("ConsulClient"));
// 或如下：
services.AddConsul(
    config =>
    {
        config.Address = new Uri(_configuration.GetValue<string>("ConsulClient:Address"));
    });
```

```
// Configure 增加如下配置：
app.UseConsul(_configuration.GetSection("ConsulService"));
// 或如下：
app.UseConsul(
    options =>
    {
        options.ServiceName = _configuration.GetValue<string>("ConsulService:ServiceName");
    });
```

##### 注册后效果图如下：
<img src="https://images2018.cnblogs.com/blog/451346/201808/451346-20180815000609695-1545801544.png" />

