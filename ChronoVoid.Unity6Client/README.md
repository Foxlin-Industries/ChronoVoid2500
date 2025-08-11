# 🚀 ChronoVoid 2500 - Unity 6 Mobile Client

## 📱 **Mobile Strategy Game Client**

Unity 6000.0.54f1 client for ChronoVoid 2500 - a multiplayer space strategy game featuring realm exploration, navigation, and tactical gameplay.

## 🎯 **Features Implemented**

### **🌌 Core Systems**
- **API Client** - Full integration with ChronoVoid backend API
- **Game Manager** - Centralized state management and scene coordination
- **Real-time Navigation** - Move between Neural Nodes via HyperTunnels
- **Realm Management** - Create and explore Nexus Realms

### **📱 UI Screens**
- **Realm List** - Browse and select available Nexus Realms
- **Navigation** - Real-time node-to-node movement
- **Create Realm** - Design custom galaxy with configurable parameters
- **User Management** - Multi-user support with location tracking

### **🎮 Mobile Optimized (Unity 6000.0.54f1)**
- **Touch Controls** - Designed for mobile interaction
- **Responsive UI** - Adaptive layouts for different screen sizes  
- **Android Target** - Optimized for Android API 34 (Android 14)
- **Performance** - Unity 6 enhanced networking and profiling
- **Enhanced Logging** - Improved debugging with Unity 6 features
- **Memory Optimization** - Unity 6 ProfilerRecorder integration

## 🔧 **Setup Instructions**

### **API Configuration**
The client connects to your ChronoVoid API server:
- **Default URL**: `http://10.0.2.2:8472` (Android emulator localhost)
- **Edit in**: `Assets/Scripts/ApiClient.cs` → `apiBaseUrl`

### **Building for Android**
1. **Open in Unity 6000.0.54f1** - Load the project in Unity 6000.0.54f1
2. **Switch Platform** - File → Build Settings → Android
3. **Configure SDK** - Set Android SDK path in Unity preferences
4. **Build Settings** (Unity 6 Enhanced):
   - **Target API Level**: 34 (Android 14)
   - **Minimum API Level**: 24 (Android 7.0) - Updated for Unity 6
   - **Architecture**: ARM64
   - **Enhanced Build Pipeline**: Automatic with Unity 6

### **Testing on Emulator**
1. **Create Android Emulator** - Pixel 7 Pro with Android 14
2. **Start ChronoVoid API** - Ensure backend running on port 8472
3. **Build & Run** - Deploy to emulator from Unity
4. **Test Connectivity** - Should connect to localhost API automatically

## 📋 **Project Structure**

```
Assets/
├── Scripts/
│   ├── ApiClient.cs           # Backend API integration
│   ├── GameManager.cs         # Core game state management
│   └── UI/
│       ├── RealmListUI.cs     # Realm browsing interface
│       ├── RealmListItem.cs   # Individual realm display
│       ├── NavigationUI.cs    # Node navigation interface
│       ├── ConnectionItem.cs  # HyperTunnel connections
│       └── CreateRealmUI.cs   # Realm creation form
├── Scenes/
│   ├── RealmListScene.unity   # Main realm selection
│   ├── NavigationScene.unity  # Node navigation
│   └── CreateRealmScene.unity # Realm creation
├── UI/                        # UI prefabs and assets
└── Prefabs/                   # Game object prefabs
```

## 🎮 **How to Use**

### **1. Realm Selection**
- Browse available Nexus Realms
- See node count, station rates, creation dates
- Set your User ID (each user has independent location)
- Select realm to enter navigation mode

### **2. Navigation**
- View current Neural Node details
- See connected nodes via HyperTunnels  
- Tap connections to travel between nodes
- Track multiple users in different locations

### **3. Realm Creation**
- Configure realm name and node count (10-1000)
- Set Quantum Station seed rate (0-100%)
- Enable "No Dead Nodes" for full connectivity
- Preview settings before creation

## 🌐 **API Integration**

### **Endpoints Used**
- `GET /api/realm` - List available realms
- `POST /api/realm` - Create new realm
- `GET /api/navigation/node/{id}` - Get node details
- `POST /api/navigation/move` - Move between nodes
- `GET /api/navigation/user/{id}/location` - Get user location

### **Data Models**
- **NexusRealmDto** - Realm information
- **NodeDetailDto** - Neural node with connections
- **UserLocationDto** - Player position tracking
- **NavigationResultDto** - Movement results

## 🚀 **Development Status**

### **✅ Completed**
- Core API client with all backend endpoints
- Game state management and scene coordination
- Realm browsing and selection UI
- Real-time navigation system
- Realm creation with full parameter control
- Multi-user location tracking
- Android build configuration

### **🔄 Next Phase**
- Visual polish and animations
- Sound effects and background music
- Extended UI feedback and loading states
- Combat system integration
- Trading system UI
- Faction management

## 🎯 **Testing Checklist**

- [ ] **Connection Test** - API responds to realm list request
- [ ] **Realm Creation** - Can create new realms with parameters
- [ ] **Navigation** - Move between connected nodes
- [ ] **Multi-User** - Different User IDs maintain separate locations
- [ ] **Android Build** - Deploys and runs on emulator
- [ ] **UI Responsiveness** - All buttons and interactions work

---

**🎮 Ready to explore the galaxy!** The Unity 6 client provides a complete mobile interface for ChronoVoid 2500's space strategy gameplay.