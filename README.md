# ChronoVoid 2500 - Space Trading Game

A 2D space trading game built with .NET MAUI mobile client and .NET Web API backend.

## Project Structure

- **ChronoVoid.API** - Backend Web API with PostgreSQL database
- **ChronoVoid2500.Mobile** - Cross-platform mobile game client (MAUI)

## Features Implemented

### Backend API
- ✅ User authentication (register/login)
- ✅ Realm management (game instances)
- ✅ Node generation with star names and planet counts
- ✅ Hyper-tunnel navigation system
- ✅ Quantum station placement
- ✅ Player ship creation with 300 cargo holds

### Mobile Game
- ✅ Login/Register screen
- ✅ Realm selection with join confirmation
- ✅ Real-time node navigation
- ✅ Star system information display
- ✅ Quantum station indicators
- ✅ Responsive UI with live updates

## Getting Started

### Prerequisites
- .NET 10.0 SDK
- PostgreSQL database
- Visual Studio 2022 or VS Code with MAUI workload
- Windows 10/11 with developer mode enabled

### Quick Start (Debug Mode)
1. **Run the debug script** to test everything:
   ```bash
   .\run-debug-mobile.ps1
   ```
   This will check API connectivity and start the mobile app with debug features.

### Running the API
1. Navigate to the API directory:
   ```bash
   cd ChronoVoid.API
   ```

2. Update the connection string in `appsettings.json` to point to your PostgreSQL database

3. Run database migrations:
   ```bash
   dotnet ef database update
   ```

4. Start the API:
   ```bash
   dotnet run
   ```
   The API will be available at `https://localhost:7001`

### Running the Mobile App

#### Debug Mode (Recommended for Development)
```bash
.\run-debug-mobile.ps1
```
This starts the app with a debug page that helps identify any Shell.Current issues.

#### Manual Start
1. Navigate to the mobile directory:
   ```bash
   cd ChronoVoid2500.Mobile
   ```

2. Update the API URL in `Services/ApiService.cs` if needed (currently set to `https://localhost:7001`)

3. Run the mobile app:
   ```bash
   dotnet run -f net10.0-windows10.0.19041.0
   ```

#### Troubleshooting
If you encounter issues (especially Shell.Current null exceptions), see [TROUBLESHOOTING.md](TROUBLESHOOTING.md) for detailed debugging steps.

## Game Flow

1. **Login/Register** - Create an account or login with existing credentials
2. **Select Realm** - Choose from available game instances (realms)
3. **Join Realm** - Confirm joining, get assigned to Node 1 with 300 cargo holds
4. **Navigate** - Use the Navigate button to see available hyper-tunnels
5. **Explore** - Move between nodes, see star names, planet counts, and quantum stations

## API Endpoints

### Authentication
- `POST /api/auth/register` - Create new user account
- `POST /api/auth/login` - Login with username/password
- `POST /api/auth/join-realm` - Join a specific realm

### Realms
- `GET /api/realm` - Get list of active realms
- `GET /api/realm/{id}` - Get specific realm details
- `POST /api/realm` - Create new realm (admin)
- `GET /api/realm/{id}/nodes` - Get all nodes in realm
- `GET /api/realm/{id}/connectivity-test` - Test realm connectivity

### Navigation
- `GET /api/navigation/node/{nodeId}` - Get node details with connections
- `POST /api/navigation/move` - Move between connected nodes
- `GET /api/navigation/user/{userId}/location` - Get user's current location
- `GET /api/navigation/realm/{realmId}/users` - Get all users in realm

## Database Schema

### Key Tables
- **Users** - Player accounts with current location and cargo
- **NexusRealms** - Game instances with configuration
- **NeuralNodes** - Star systems with coordinates and properties
- **HyperTunnels** - Connections between nodes
- **StarNames** - Generated using Greek letters + constellation names

## Technical Features

### Real-time Updates
- The mobile app is designed for real-time updates using MVVM pattern
- Observable properties automatically update the UI
- Ready for SignalR integration for live multiplayer features

### Responsive Design
- Dark space theme with green accent colors
- Optimized for mobile devices
- Smooth navigation animations
- Loading indicators and error handling

### Scalable Architecture
- Clean separation between API and mobile client
- Repository pattern with Entity Framework
- Dependency injection throughout
- Async/await for all operations

## Future Enhancements

- [ ] Resource trading system
- [ ] Real-time multiplayer with SignalR
- [ ] Ship upgrades and customization
- [ ] Combat system
- [ ] Market economics
- [ ] Guild/faction system
- [ ] 2D visual map of nodes
- [ ] Sound effects and music

## Development Notes

- Built with .NET 10.0 preview
- Uses Entity Framework Core with PostgreSQL
- MAUI for cross-platform mobile development
- MVVM pattern with CommunityToolkit.Mvvm
- RESTful API design
- Comprehensive error handling and validation