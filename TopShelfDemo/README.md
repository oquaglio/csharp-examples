#

## How to Use

1. Install TopShelf: In Visual Studio/VS Code, add via NuGet: Install-Package Topshelf (or dotnet add package Topshelf).
1. Build: Compile the project to get BasicTopShelfService.exe.
1. Run as Console: BasicTopShelfService.exe (runs in console mode for testing).
1. Install as Service: BasicTopShelfService.exe install (admin rights needed).
1. Start/Stop Service: BasicTopShelfService.exe start / stop, or via Services.msc.
1. Uninstall: BasicTopShelfService.exe uninstall.

```sh
dotnet add package Topshelf
```

This will run in Linux as a Console app, but you can't install it as a systemd daemon.
