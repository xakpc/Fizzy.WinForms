# Docker Container Management

## Overview

The WinForms app manages a Docker container running a Rails application (Fizzy). The container is controlled via `DockerHelper` class.

## Container Configuration

- **Container Name**: `fizzy`
- **Image**: `ghcr.io/fizzy-win-local`
- **Port Mapping**: `9461:80` (host:container)
- **Environment**: `SECRET_KEY_BASE` required for Rails

## Data Persistence

SQLite database is persisted using a volume mount:

- **Host Path**: `%LOCALAPPDATA%\Fizzy` (e.g., `C:\Users\{username}\AppData\Local\Fizzy`)
- **Container Path**: `/rails/storage`

The Rails app stores its SQLite database at `/rails/storage/production.sqlite3`. By mounting the data folder from the user's AppData directory, the database survives container recreation during updates and avoids permission issues when the app is installed in Program Files.

**Note**: Previously stored in `<exe location>/data/`, moved to LocalApplicationData in v1.0.1 to resolve multi-user access rights issues.

## Container Lifecycle

### Start
1. Check if container exists and is running
2. If paused, unpause it
3. If stopped, start it
4. If doesn't exist, create with volume mount
5. Wait for health check at `http://localhost:9461`

Status updates are shown in the status strip during startup:
- "Checking container status..."
- "Creating container (first launch)..." / "Starting container..." / "Resuming container..."
- "Waiting for container to be ready..."
- "Loading model... This may take a while on first launch." (after 2.5s)
- "Container ready!"
- "Initializing browser..."

### Stop
Stops the container gracefully via `docker stop`. Shows "Stopping container..." status message.

### Pause/Unpause
Used when app closes/reopens to preserve container state without full restart

### Update
1. Stop container
2. Pull latest image from `ghcr.io/fizzy-win-local`
3. Remove old container (`docker rm`)
4. Create new container with same volume mount
5. Reload webview

Shows "Updating container..." status during the process.

## Key Implementation Details

- Update runs in a visible `cmd` window (`/k` flag) so user can see progress
- `WaitForExit()` ensures update completes before reloading UI
- Volume mount preserves data across updates
- Data folder is created automatically if missing
- Status strip shows progress during operations and auto-hides when complete
- About dialog displays the full data folder path for user reference
- Status callback pattern allows `DockerHelper` to report progress to UI
