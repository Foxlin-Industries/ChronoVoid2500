# Unity 6 Beta Breaking Changes and Known Issues

This document outlines known issues and breaking changes in Unity 6 beta versions that could affect the ChronoVoid 2500 project. Developers should be aware of these issues when implementing new features or debugging existing ones.

## Critical Issues

### Physics
- **Rigidbody Instantiation Crash**: Crash on mono-2.0-bdwgc.dll when instantiating a Prefab with Rigidbody after loading a scene from an Asset Bundle (UUM-108799)
  - **Workaround**: Avoid instantiating prefabs with Rigidbody components immediately after loading scenes from Asset Bundles. Consider adding a frame delay.

### Graphics & Rendering
- **DirectX12 Crashes**: Multiple crash scenarios when using DirectX12:
  - Crash on D3D12SwapChain::Present during various Unity operations (UUM-107390)
  - Crash with multiple stacktraces when rendering large models (UUM-111263)
  - **Workaround**: Use DirectX11 for development until these issues are resolved.

- **Meta Quest Link Crash**: Fixed in 6000.2.0f1, but be aware of potential issues when releasing render textures on Meta Quest Link.

- **Texture2D Subasset Crashes**: Fixed in 6000.2.0b13, but be cautious when reading Texture2D subassets with large data payloads.

- **Metal Freezes on iOS**: The Player freezes with "Execution of the command buffer was aborted due to an error during execution" error (UUM-111494)
  - **Workaround**: Consider using OpenGLES on iOS for testing until fixed.

- **SRP Foundation Gizmos**: Gizmos not rendering in Game view (UUM-104383)
  - **Workaround**: Use Scene view for debugging with gizmos until fixed.

### Animation
- **AnimatorController State Issues**: AnimatorController states stop working when they reach a normalized time which is too large (100,000+ loops) (UUM-111056)
  - **Workaround**: Reset animations before they reach extremely high loop counts.

### UI Toolkit
- **Data Binding Performance**: Fixed in 6000.2.0b13, but be aware that data binding was previously updated twice per frame in runtime panels.

- **Text Change Events**: Fixed in 6000.2.0b13, but previously change events were being sent when text precision didn't match float values exactly.

## Specific Areas to Test Carefully

### Asset Management
- **AssetBundle Loading**: Potential crash when calling AssetBundle.UnloadAsync() followed immediately by AssetBundle.LoadFromFileAsync() for the same bundle.
  - **Workaround**: Add delay between unloading and reloading the same asset bundle.

### Mesh Operations
- **Mesh.GetIndices**: Fixed in 6000.2.0b13, but be cautious when calling with a list and passing -1 to ignore mesh LODs.

- **Mesh.GetIndexStart/GetIndexCount**: Fixed in 6000.2.0b13, but previously always returned values for sub-mesh 0 in meshes with multiple LODs.

### Mobile Platform Specific
- **Android Video Playback**: Fixed in 6000.2.0f1, but be aware of potential video freezing on Galaxy S25 and similar devices.

- **iOS Build Issues**: Fixed in 6000.2.0b13, but be aware of potential build failures with Xcode 26 beta and Metal toolchain.

## Performance Considerations

- **PS5 Performance**: Fixed a performance regression for PS5 by disabling FrameBuffer Fetch emulation (UUM-110424).

- **Skinning Matrices**: Fixed potential race conditions in CalculateSkinningMatrices by ensuring direct job completion.

## Recommendations for ChronoVoid 2500 Development

1. **Physics Implementation**:
   - Implement space combat physics carefully, avoiding rapid scene loading with immediate Rigidbody instantiation
   - Consider using a physics manager that handles instantiation with appropriate delays

2. **Rendering Pipeline**:
   - Use DirectX11 for development until DirectX12 issues are resolved
   - Test thoroughly on mobile devices, especially iOS with Metal
   - Implement fallback rendering paths for problematic platforms

3. **Asset Management**:
   - Implement robust error handling around AssetBundle loading/unloading
   - Add appropriate delays between asset operations
   - Consider preloading critical assets to avoid runtime loading issues

4. **3D Galaxy Map Implementation**:
   - Test with varying numbers of star systems to ensure performance
   - Implement LOD (Level of Detail) for distant star systems
   - Use efficient mesh generation techniques for connection lines

5. **Combat Visualization**:
   - Test animation systems with reasonable loop counts
   - Implement efficient particle systems for weapon effects
   - Use instancing for similar units in planetary combat

6. **UI Implementation**:
   - Be cautious with data binding in UI Toolkit
   - Test text input fields thoroughly, especially with numeric values
   - Ensure UI responsiveness during heavy rendering operations

## ChronoVoid 2500 Specific Compatibility

### ✅ **Current Status (Safe)**
Our codebase is relatively safe from Unity 6000.2.0b12 breaking changes:

- **No Rigidbody Usage**: We don't currently use Rigidbody components that could crash
- **No AssetBundle Loading**: No immediate AssetBundle crash risks
- **No AnimatorController Issues**: No animation loop count problems
- **No UI Toolkit**: Using traditional Unity UI (uGUI)
- **No Mesh Operations**: No direct mesh manipulation affected by changes
- **Updated FindObjectOfType**: Already using `FindFirstObjectByType`

### 🛠️ **Implemented Safeguards**

1. **Unity6Compatibility.cs**: Added compatibility helper class with:
   - Safe Rigidbody instantiation methods
   - AssetBundle loading with delays
   - Animation loop count monitoring
   - Graphics API validation

2. **Unity6ProjectValidator.cs**: Editor tool to validate project settings:
   - Access via `ChronoVoid → Unity 6 Project Validator`
   - Checks graphics APIs, components, and project settings
   - Provides recommendations for Unity 6 compatibility

3. **GameManager Integration**: Added compatibility checks on startup

### 📋 **Upgrade Checklist for Unity 6000.2.0b12**

- [ ] Backup current project
- [ ] Run Unity6ProjectValidator before upgrade
- [ ] Test login system after upgrade
- [ ] Verify graphics API settings (prefer DirectX11)
- [ ] Test on target platforms (Windows, Android, iOS)
- [ ] Monitor console for compatibility warnings
- [ ] Test particle systems and space-themed effects
- [ ] Verify API connectivity and networking

### 🚨 **Post-Upgrade Testing Priority**

1. **High Priority**:
   - Login/registration flow
   - API connectivity
   - Scene transitions
   - UI responsiveness

2. **Medium Priority**:
   - Graphics rendering
   - Audio playback
   - Performance metrics

3. **Low Priority**:
   - Advanced graphics features
   - Platform-specific optimizations

## Version Tracking

This document is based on Unity 6000.2.0b12 release notes. Update this document when upgrading to newer Unity versions to track resolved and new issues.

**Last Updated**: Unity 6000.2.0b12 compatibility analysis
**Project Status**: ✅ Ready for upgrade with safeguards in place
