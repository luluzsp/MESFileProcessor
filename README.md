# MES 文件处理器

一款 Windows 桌面应用程序，用于监控文件系统变化并自动处理文件以实现 MES（制造执行系统）集成。

## 项目概述

MES 文件处理器是一个文件监控和处理工具，能够监视指定目录中的新文件，并通过调用 MES（制造执行系统）API 自动处理这些文件。该应用程序提供实时监控、自动文件处理和全面的日志记录功能。

## 主要功能

- **实时文件监控**：持续监控指定目录中的新文件
- **自动化处理**：当文件出现在监控目录中时自动处理
- **MES 系统集成**：将处理后的数据发送到 MES 系统 API
- **全面日志记录**：详细记录所有操作、API 调用和错误
- **配置管理**：易于使用的配置界面
- **用户友好界面**：用于监控和配置的简单 Windows 窗体界面

## 项目结构

```
MESFileProcessor/
├── MainForm.cs              # 主应用程序窗口
├── ConfigForm.cs            # 配置设置窗口
├── FileWatcher.cs           # 文件系统监控逻辑
├── MESService.cs            # MES API 集成
├── ConfigManager.cs         # 配置管理
├── LogManager.cs            # 日志功能
├── Program.cs               # 应用程序入口点
├── build.bat                # 构建自动化脚本
├── DEPLOYMENT.md            # 部署说明
└── MESFileProcessor.csproj  # 项目配置
```

## 技术栈

- **框架**: .NET Framework 4.7.2
- **语言**: C#
- **UI 框架**: Windows Forms
- **依赖**: Newtonsoft.Json (13.0.3)
- **平台**: Windows 10/11, Windows Server 2019/2022

## 快速开始

### 系统要求
- Windows 10/11 或 Windows Server 2019/2022
- .NET Framework 4.7.2 或更高版本（通常已预装在 Windows 上）

### 运行应用程序

1. **克隆仓库**:
   ```bash
   git clone <repository-url>
   cd MESFileProcessor
   ```

2. **构建项目**:
   ```bash
   # 使用提供的构建脚本
   build.bat

   # 或使用 MSBuild 手动构建
   msbuild MESFileProcessor.sln /p:Configuration=Release
   ```

3. **运行应用程序**:
   - 可执行文件位于: `bin/Release/MESFileProcessor.exe`
   - 或使用构建脚本，构建后会询问是否运行

### 配置

1. **首次运行**: 应用程序将创建默认配置文件
2. **配置文件**: 位于应用程序目录中的 `config.json`
3. **设置**: 通过界面配置 MES API URL、站点名称和产线名称
4. **文件监控**: 设置要监控的目录和输出位置

## 使用方法

### 基本工作流程

1. **启动应用程序**: 运行 MESFileProcessor.exe
2. **配置设置**: 点击"程序配置"设置：
   - MES 系统 API 地址
   - 站点名称
   - 产线名称
   - 监控目录
3. **开始监控**: 点击"开始监控"开始文件监视
4. **监控状态**: 在主窗口中查看实时状态
5. **查看日志**: 检查 Logs 文件夹中的详细操作日志

### 文件处理流程

- **输入**: 放置在监控目录中的文件
- **处理**: 文件被自动处理，数据发送到 MES
- **输出**: 处理后的文件移动到输出目录
- **日志**: 所有操作都带有时间戳和详细信息记录

## 开发

### 从源代码构建

#### 使用构建脚本
```bash
build.bat
```

#### 手动构建
```bash
# 还原包
nuget restore MESFileProcessor.sln

# 构建调试版本
msbuild MESFileProcessor.sln /p:Configuration=Debug

# 构建发布版本
msbuild MESFileProcessor.sln /p:Configuration=Release
```

### 项目依赖

- **Newtonsoft.Json**: JSON 序列化/反序列化
- **System.Windows.Forms**: Windows 窗体 UI 框架
- **System.Net.Http**: API 调用的 HTTP 客户端

## 部署

### 独立部署
创建一个包含所有依赖项的独立可执行文件：

```bash
# 对于 .NET Framework（当前项目）
# 构建发布配置并分发 bin/Release 文件夹

# 对于未来的 .NET Core/.NET 5+ 迁移
# dotnet publish --configuration Release --self-contained true --runtime win-x64
```

### 分发文件
构建后分发以下内容：
- `MESFileProcessor.exe` - 主可执行文件
- `MESFileProcessor.exe.config` - 应用程序配置
- `Newtonsoft.Json.dll` - JSON 库
- `config.json` - 用户配置（首次运行时创建）
- `Logs/` - 日志目录（运行时创建）

## 配置

### config.json 结构
```json
{
  "MesUrl": "https://your-mes-server.com/api",
  "StationName": "站点-01",
  "LineName": "产线-A",
  "WatchPath": "C:\\input",
  "OutputPath": "C:\\output"
}
```

### 环境变量
- 不需要环境变量
- 所有配置通过 config.json 和 UI 进行

## 日志记录

### 日志位置
- **路径**: `Logs/yyyy-MM-dd.log`
- **轮换**: 每日日志文件
- **保留**: 建议手动清理

### 日志内容
- 应用程序启动/关闭事件
- 文件监控状态变化
- 文件处理操作
- API 调用详情（请求和响应）
- 错误消息和异常
- 性能指标

## 故障排除

### 常见问题

#### 应用程序无法启动
- 检查 .NET Framework 4.7.2 安装
- 验证所有必需文件是否存在
- 检查 Windows 事件日志

#### 文件监控不工作
- 验证监控目录是否存在且有读取权限
- 检查输出目录是否有写入权限
- 确认配置正确

#### API 调用失败
- 检查网络连接
- 验证 MES 服务器 URL 是否正确
- 查看日志文件中的详细错误消息
- 测试 MES 系统可用性

### 调试模式
以调试配置构建以进行详细调试：
```bash
# 使用构建脚本
build.bat debug

# 或手动
msbuild MESFileProcessor.sln /p:Configuration=Debug
MESFileProcessor.exe
```

## 贡献

### 开发设置
1. 克隆仓库
2. 在 Visual Studio 中打开 MESFileProcessor.sln
3. 构建并运行项目
4. 进行更改并彻底测试
5. 提交带有清晰描述的拉取请求

### 代码风格
- 遵循 C# 编码约定
- 使用有意义的变量和方法名称
- 为复杂逻辑添加注释
- 保持一致的格式

## 安全注意事项

- **网络安全**: 确保 MES API 使用 HTTPS
- **文件权限**: 限制应用程序文件系统访问
- **配置安全**: 考虑对敏感配置数据进行加密
- **日志安全**: 避免在日志中记录敏感信息如密码或令牌

## 未来增强

- **多线程处理**: 并行文件处理
- **数据库集成**: 存储处理历史
- **邮件通知**: 错误或完成时警报
- **Web 仪表板**: 远程监控界面
- **插件系统**: 可扩展文件格式支持
- **性能指标**: 实时性能监控

## 许可证

本项目为仅供内部使用的专有软件。

## 支持

如需技术支持或有疑问：
1. 检查 Logs 目录中的详细错误信息
2. 查看 DEPLOYMENT.md 文件中的设置说明
3. 查阅上面的故障排除部分
4. 联系开发团队

---

*最后更新: 2026年1月*