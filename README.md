# MES File Processor

A Windows desktop application designed to monitor file system changes and automatically process files for MES (Manufacturing Execution System) integration.

## Overview

MES File Processor is a file monitoring and processing tool that watches designated directories for new files and automatically processes them by sending data to MES (Manufacturing Execution System) APIs. The application provides real-time monitoring, automatic file processing, and comprehensive logging capabilities.

## Features

- **Real-time File Monitoring**: Continuously watches specified directories for new files
- **Automated Processing**: Automatically processes files when they appear in monitored directories
- **MES Integration**: Sends processed data to MES system APIs
- **Comprehensive Logging**: Detailed logging of all operations, API calls, and errors
- **Configuration Management**: Easy-to-use configuration interface
- **User-Friendly Interface**: Simple Windows Forms interface for monitoring and configuration

## Project Structure

```
MESFileProcessor/
├── MainForm.cs              # Main application window
├── ConfigForm.cs            # Configuration settings window
├── FileWatcher.cs           # File system monitoring logic
├── MESService.cs            # MES API integration
├── ConfigManager.cs         # Configuration management
├── LogManager.cs            # Logging functionality
├── Program.cs               # Application entry point
├── build.bat                # Build automation script
├── DEPLOYMENT.md            # Deployment instructions
└── MESFileProcessor.csproj  # Project configuration
```

## Tech Stack

- **Framework**: .NET Framework 4.7.2
- **Language**: C#
- **UI Framework**: Windows Forms
- **Dependencies**: Newtonsoft.Json (13.0.3)
- **Platform**: Windows 10/11, Windows Server 2019/2022

## Quick Start

### Prerequisites
- Windows 10/11 or Windows Server 2019/2022
- .NET Framework 4.7.2 or later (usually pre-installed on Windows)

### Running the Application

1. **Clone the repository**:
   ```bash
   git clone <repository-url>
   cd MESFileProcessor
   ```

2. **Build the project**:
   ```bash
   # Using the provided build script
   build.bat

   # Or manually with MSBuild
   msbuild MESFileProcessor.sln /p:Configuration=Release
   ```

3. **Run the application**:
   - The executable will be located at: `bin/Release/MESFileProcessor.exe`
   - Or use the build script which offers to run after building

### Configuration

1. **First Run**: The application will create a default configuration file
2. **Configuration File**: Located at `config.json` in the application directory
3. **Settings**: Configure MES API URL, station name, and line name through the UI
4. **File Monitoring**: Set up directories to monitor and output locations

## Usage

### Basic Workflow

1. **Start the Application**: Launch MESFileProcessor.exe
2. **Configure Settings**: Click "程序配置" (Program Configuration) to set up:
   - MES system API URL
   - Station name
   - Line name
   - Monitoring directories
3. **Start Monitoring**: Click "开始监控" (Start Monitoring) to begin file watching
4. **Monitor Status**: View real-time status in the main window
5. **View Logs**: Check the Logs folder for detailed operation logs

### File Processing

- **Input**: Files placed in the monitored directory
- **Processing**: Files are automatically processed and data sent to MES
- **Output**: Processed files are moved to the output directory
- **Logs**: All operations are logged with timestamps and details

## Development

### Building from Source

#### Using Build Script
```bash
build.bat
```

#### Manual Build
```bash
# Restore packages
nuget restore MESFileProcessor.sln

# Build Debug
dotnet build --configuration Debug

# Build Release
dotnet build --configuration Release
```

### Project Dependencies

- **Newtonsoft.Json**: JSON serialization/deserialization
- **System.Windows.Forms**: Windows Forms UI framework
- **System.Net.Http**: HTTP client for API calls

## Deployment

### Standalone Deployment
Create a standalone executable that includes all dependencies:

```bash
# For .NET Framework (current project)
# Build Release configuration and distribute the bin/Release folder

# For future .NET Core/.NET 5+ migration
# dotnet publish --configuration Release --self-contained true --runtime win-x64
```

### Distribution Files
After building, distribute the following:
- `MESFileProcessor.exe` - Main executable
- `MESFileProcessor.exe.config` - Application configuration
- `Newtonsoft.Json.dll` - JSON library
- `config.json` - User configuration (created on first run)
- `Logs/` - Log directory (created at runtime)

## Configuration

### config.json Structure
```json
{
  "MesUrl": "https://your-mes-server.com/api",
  "StationName": "Station-01",
  "LineName": "Line-A",
  "WatchPath": "C:\\input",
  "OutputPath": "C:\\output"
}
```

### Environment Variables
- No environment variables required
- All configuration through config.json and UI

## Logging

### Log Location
- **Path**: `Logs/yyyy-MM-dd.log`
- **Rotation**: Daily log files
- **Retention**: Manual cleanup recommended

### Log Contents
- Application startup/shutdown events
- File monitoring status changes
- File processing operations
- API call details (requests and responses)
- Error messages and exceptions
- Performance metrics

## Troubleshooting

### Common Issues

#### Application Won't Start
- Check .NET Framework 4.7.2 installation
- Verify all required files are present
- Check Windows Event Logs

#### File Monitoring Not Working
- Verify monitoring directory exists and has read permissions
- Check output directory has write permissions
- Confirm configuration is correct

#### API Calls Failing
- Check network connectivity
- Verify MES server URL is correct
- Review log files for detailed error messages
- Test MES system availability

### Debug Mode
Build in Debug configuration for detailed debugging:
```bash
# Using build script
build.bat debug

# Or manual
dotnet build --configuration Debug
dotnet run --configuration Debug
```

## Contributing

### Development Setup
1. Clone the repository
2. Open MESFileProcessor.sln in Visual Studio
3. Build and run the project
4. Make changes and test thoroughly
5. Submit pull requests with clear descriptions

### Code Style
- Follow C# coding conventions
- Use meaningful variable and method names
- Add comments for complex logic
- Maintain consistent formatting

## Security Considerations

- **Network Security**: Ensure MES API uses HTTPS
- **File Permissions**: Restrict application file system access
- **Configuration Security**: Consider encrypting sensitive configuration data
- **Log Security**: Avoid logging sensitive information like passwords or tokens

## Future Enhancements

- **Multi-threaded Processing**: Parallel file processing
- **Database Integration**: Store processing history
- **Email Notifications**: Alert on errors or completion
- **Web Dashboard**: Remote monitoring interface
- **Plugin System**: Extensible file format support
- **Performance Metrics**: Real-time performance monitoring

## License

This project is proprietary software for internal use only.

## Support

For technical support or questions:
1. Check the Logs directory for detailed error information
2. Review the DEPLOYMENT.md file for setup instructions
3. Consult the troubleshooting section above
4. Contact the development team

---

*Last updated: January 2026*