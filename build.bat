@echo off
echo ========================================
echo MES文件处理程序编译脚本 (.NET Framework 4.7.2)
echo ========================================
echo.

REM 检查MSBuild是否存在
where msbuild >nul 2>&1
if errorlevel 1 (
    echo 正在查找MSBuild...
    
    REM 尝试常见的MSBuild路径
    set MSBUILD_PATH=""
    
    REM Visual Studio 2019
    if exist "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe" (
        set MSBUILD_PATH="C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe"
    )
    if exist "C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe" (
        set MSBUILD_PATH="C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe"
    )
    
    REM Visual Studio 2022
    if exist "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" (
        set MSBUILD_PATH="C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
    )
    if exist "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe" (
        set MSBUILD_PATH="C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe"
    )
    
    REM .NET Framework MSBuild
    if exist "C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" (
        set MSBUILD_PATH="C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
    )
    
    if %MSBUILD_PATH%=="" (
        echo 错误: 未找到MSBuild.exe
        echo 请确保已安装以下任一工具:
        echo - Visual Studio 2019/2022
        echo - .NET Framework 4.7.2 Developer Pack
        echo.
        echo 下载地址: https://dotnet.microsoft.com/download/dotnet-framework/net472
        pause
        exit /b 1
    )
    
    set MSBUILD=%MSBUILD_PATH%
) else (
    set MSBUILD=msbuild
)

echo 找到MSBuild: %MSBUILD%
echo.

REM 还原NuGet包
echo 正在还原NuGet包...
nuget restore MESFileProcessor.sln
if errorlevel 1 (
    echo 警告: NuGet还原失败，尝试继续编译...
    echo 如果编译失败，请手动安装NuGet并运行: nuget restore
)
echo.

REM 清理之前的编译
echo 正在清理之前的编译...
%MSBUILD% MESFileProcessor.sln /t:Clean /p:Configuration=Release /v:m
echo.

REM 编译项目
echo 正在编译项目 (Release配置)...
%MSBUILD% MESFileProcessor.sln /t:Build /p:Configuration=Release /v:m

if errorlevel 1 (
    echo.
    echo ========================================
    echo 编译失败！
    echo ========================================
    pause
    exit /b 1
)

echo.
echo ========================================
echo 编译成功！
echo ========================================
echo.
echo 可执行文件位置: bin\Release\MESFileProcessor.exe
echo.

REM 询问是否运行程序
set /p choice=是否立即运行程序? (y/n): 
if /i "%choice%"=="y" (
    echo 正在启动程序...
    start "" "bin\Release\MESFileProcessor.exe"
)

pause
