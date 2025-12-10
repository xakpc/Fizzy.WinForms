# Changelog

## [1.0.1] - 2025-12-11

### Fixed
- Fixed permission errors when sharing app between multiple users by moving data folder from Program Files to user AppData directory

### Changed
- Data folder location moved from `<exe location>\data` to `%LOCALAPPDATA%\Fizzy` (e.g., `C:\Users\{username}\AppData\Local\Fizzy`)
- About dialog now displays the full data folder path

### Added
- Status strip with progress messages during container operations
- Status updates during startup: "Checking Docker", "Creating container (first launch)", "Loading model...", etc.
- Status messages during stop/update operations
- Special message for first launch when model is loading: "Loading model... This may take a while on first launch."
- Refresh menu item to navigate back to home page

### Technical
- Status strip auto-hides after operations complete (300ms delay)
- `DockerHelper.StartContainerAsync()` now accepts optional status callback
- Status strip initially hidden by default

## [1.0.0] - 2025-12-11

Initial release
