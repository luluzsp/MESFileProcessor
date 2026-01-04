# 开发和部署说明

## 开发环境设置

### 1. 系统要求
- Windows 10/11 或 Windows Server 2019/2022
- .NET 6.0 SDK 或更高版本
- Visual Studio 2022 或 Visual Studio Code（可选）

### 2. 克隆和编译
```bash
# 如果使用Git克隆
git clone <repository-url>
cd MESFileProcessor

# 还原依赖包
dotnet restore

# 编译项目
dotnet build --configuration Release

# 运行程序
dotnet run
```

### 3. 使用批处理文件
直接双击 `build_and_run.bat` 文件，脚本会自动：
- 检查.NET SDK是否安装
- 还原NuGet包
- 编译项目
- 询问是否运行程序

## 部署说明

### 1. 独立部署
如果目标机器没有安装.NET运行时，可以创建独立部署：

```bash
# 发布为独立应用（包含运行时）
dotnet publish --configuration Release --self-contained true --runtime win-x64

# 发布文件位置
bin\Release\net6.0-windows\win-x64\publish\
```

### 2. 框架依赖部署
如果目标机器已安装.NET运行时：

```bash
# 发布为框架依赖应用
dotnet publish --configuration Release --self-contained false

# 发布文件位置
bin\Release\net6.0-windows\publish\
```

### 3. 部署文件结构
```
发布目录/
├── MESFileProcessor.exe     # 主程序
├── MESFileProcessor.dll     # 主程序库
├── config.json              # 配置文件（运行时生成）
├── Logs/                    # 日志文件夹（运行时生成）
├── Newtonsoft.Json.dll      # 依赖库
└── 其他运行时文件...
```

## 配置管理

### 1. 配置文件位置
- 开发环境: `项目根目录/config.json`
- 部署环境: `程序所在目录/config.json`

### 2. 配置文件格式
```json
{
  "MesUrl": "MES系统API地址",
  "StationName": "工站名称",
  "LineName": "线体名称"
}
```

### 3. 首次运行
- 程序首次运行时会创建默认配置文件
- 用户需要通过"程序配置"按钮设置正确的参数
- 配置自动保存，下次启动时自动加载

## 日志管理

### 1. 日志文件位置
`程序目录/Logs/yyyy-MM-dd.log`

### 2. 日志内容
- 程序启动/关闭事件
- 文件监控状态变化
- 文件处理过程
- API调用详情（包括请求参数和响应）
- 错误和异常信息

### 3. 日志维护
- 建议定期清理旧日志文件
- 可以设置日志文件保留天数（需要自定义实现）

## 故障排查

### 1. 常见问题

#### 程序无法启动
- 检查是否安装了.NET 6.0运行时
- 检查程序文件是否完整
- 查看Windows事件日志

#### 文件监控不工作
- 检查监控文件夹是否存在且有读取权限
- 检查输出文件夹是否有写入权限
- 确认配置信息是否正确

#### API调用失败
- 检查网络连接
- 验证MES服务器地址是否正确
- 查看日志文件中的API调用详情
- 检查MES系统是否正常运行

### 2. 调试模式
编译Debug版本进行详细调试：
```bash
dotnet build --configuration Debug
dotnet run --configuration Debug
```

### 3. 性能监控
- 监控内存使用情况
- 关注文件处理速度
- 查看API响应时间

## 定制开发

### 1. 扩展文件格式支持
在 `FileWatcher.cs` 中修改 `ProcessFile` 方法，添加新的文件格式处理逻辑。

### 2. 添加新的API接口
在 `MESService.cs` 中添加新的API调用方法。

### 3. 自定义日志格式
在 `LogManager.cs` 中修改日志格式和输出方式。

### 4. 界面定制
修改 `MainForm.cs` 和 `ConfigForm.cs` 来调整用户界面。

## 安全注意事项

1. **网络安全**: 确保MES API通信使用HTTPS
2. **文件权限**: 限制程序运行账户的文件系统权限
3. **配置保护**: 考虑对敏感配置信息进行加密
4. **日志安全**: 避免在日志中记录敏感信息

## 维护建议

1. **定期更新**: 保持.NET运行时和依赖包的最新版本
2. **监控运行**: 设置程序运行状态监控
3. **备份配置**: 定期备份配置文件
4. **性能优化**: 根据实际使用情况调整参数
