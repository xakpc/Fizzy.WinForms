# Fizzy WinForms

A Windows Forms desktop application that wraps a Docker-containerized Rails application, providing a native desktop experience for the Fizzy web application.

<img width="802" height="636" alt="image" src="https://github.com/user-attachments/assets/97968268-c8d3-49b2-85b9-600c473710b1" />

## Overview

Fizzy WinForms serves as a client wrapper around a Docker container running a Rails backend. It provides:

- Embedded web view using Microsoft WebView2 (Chromium-based)
- Docker container lifecycle management (start, stop, pause, update)
- Persistent data storage via volume mounts
- Modern dark-themed UI

## Requirements

- Windows 10/11
- [.NET 10.0 Runtime](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) installed and running
- Internet connection (for initial container image download)

## Installation

1. Ensure Docker Desktop is installed and running
2. Download the latest release
3. Run `Xakpc.Fizzy.WinForms.exe`

The application will automatically:
- Pull the container image on first run
- Create a `data/` directory for persistent storage
- Start the container and load the web interface

> **Note:** The first launch will download the Docker image (~500MB), which may take several minutes depending on your internet connection. Subsequent launches will be much faster.

## Usage

### Menu Options

| Menu Item | Action |
|-----------|--------|
| **Container > Start** | Start or resume the Docker container |
| **Container > Stop** | Stop the running container |
| **Container > Update** | Pull the latest image and recreate the container |
| **Container > Exit** | Close the application |

### Data Persistence

Application data is stored in the `data/` folder next to the executable. This directory is mounted to the container's `/rails/storage` path, ensuring your data persists across container updates and application restarts.

## Building from Source

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- Visual Studio 2022 or later (optional)

### Build Commands

```bash
# Clone the repository
git clone https://github.com/xakpc/Fizzy.WinForms.git
cd Fizzy.WinForms

# Build the project
dotnet build

# Run the application
dotnet run --project src/Xakpc.Fizzy.WinForms
```

### Build Output

Compiled binaries are output to:
- Debug: `build/bin/Xakpc.Fizzy.WinForms/Debug/net10.0-windows/`
- Release: `build/bin/Xakpc.Fizzy.WinForms/Release/net10.0-windows/`

## Project Structure

```
Xakpc.Fizzy.WinForms/
├── assets/                     # Application icons and images
├── src/
│   └── Xakpc.Fizzy.WinForms/
│       ├── Program.cs          # Application entry point
│       ├── FrmMain.cs          # Main form logic
│       ├── DockerHelper.cs     # Docker container management
│       └── Properties/         # Resources
├── Directory.Build.props       # Centralized build configuration
└── Xakpc.Fizzy.WinForms.slnx   # Solution file
```

## Technical Details

- **Container Image**: `ghcr.io/xakpc/fizzy-win-local`
- **Container Name**: `fizzy`
- **Port Mapping**: `9461:80`
- **Database**: SQLite (persisted in mounted volume)

## Related Projects

- [fizzy-win-local](https://github.com/xakpc/fizzy-win-local) - The Docker container image (Rails backend) used by this application

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Author

Pavel Osadchuk
