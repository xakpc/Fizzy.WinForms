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

- **Host Path**: `<exe location>/data/`
- **Container Path**: `/rails/storage`

The Rails app stores its SQLite database at `/rails/storage/production.sqlite3`. By mounting the `data` folder from the host, the database survives container recreation during updates.

## Container Lifecycle

### Start
1. Check if container exists and is running
2. If paused, unpause it
3. If stopped, start it
4. If doesn't exist, create with volume mount
5. Wait for health check at `http://localhost:9461`

### Stop
Stops the container gracefully via `docker stop`

### Pause/Unpause
Used when app closes/reopens to preserve container state without full restart

### Update
1. Stop container
2. Pull latest image from `ghcr.io/fizzy-win-local`
3. Remove old container (`docker rm`)
4. Create new container with same volume mount
5. Reload webview

## Key Implementation Details

- Update runs in a visible `cmd` window (`/k` flag) so user can see progress
- `WaitForExit()` ensures update completes before reloading UI
- Volume mount preserves data across updates
- Data folder is created automatically if missing
