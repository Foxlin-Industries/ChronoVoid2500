---
description: Repository Information Overview
alwaysApply: true
---

# Repository Information Overview

## Repository Summary
ChronoVoid 2500 is a 2D space trading game built with .NET technologies. It consists of a .NET Web API backend with PostgreSQL database and multiple client applications including a cross-platform MAUI mobile client and a Unity 6 client.

## Repository Structure
- **ChronoVoid.API**: Backend Web API with PostgreSQL database
- **ChronoVoid2500.Mobile**: Cross-platform mobile game client (MAUI)
- **ChronoVoid.Unity6Client**: Unity 6 game client
- **ChronoVoid.Admin**: Admin panel for game management (Blazor)
- **Test Projects**: Multiple test projects for different components

### Main Repository Components
- **Backend API**: Handles authentication, realm management, star system generation, and navigation
- **Mobile Client**: MAUI-based cross-platform mobile interface
- **Unity Client**: Unity 6-based game client
- **Admin Panel**: Blazor-based administration interface
- **Test Projects**: Various test projects for different components

## Projects

### ChronoVoid.API
**Configuration File**: ChronoVoid.API.csproj

#### Language & Runtime
**Language**: C#
**Version**: .NET 10.0
**Build System**: .NET SDK
**Package Manager**: NuGet

#### Dependencies
**Main Dependencies**:
- Microsoft.AspNetCore.OpenApi (10.0.0-preview.6.25358.103)
- Microsoft.AspNetCore.SignalR (1.2.0)
- Microsoft.EntityFrameworkCore.Design (9.0.7)
- Npgsql.EntityFrameworkCore.PostgreSQL (9.0.4)

#### Build & Installation
```bash
cd ChronoVoid.API
dotnet ef database update
dotnet run
```

#### Testing
**Test Location**: Various test projects (DatabaseTest, UITest, etc.)
**Run Command**:
```bash
dotnet test
```

### ChronoVoid2500.Mobile
**Configuration File**: ChronoVoid2500.Mobile.csproj

#### Language & Runtime
**Language**: C#
**Version**: .NET 10.0
**Build System**: .NET MAUI
**Package Manager**: NuGet

#### Dependencies
**Main Dependencies**:
- Microsoft.Maui.Controls
- Microsoft.AspNetCore.SignalR.Client (10.0.0-preview.6.25358.103)
- Microsoft.Extensions.Http (10.0.0-preview.6.25358.103)
- CommunityToolkit.Mvvm (8.4.0)

#### Build & Installation
```bash
cd ChronoVoid2500.Mobile
dotnet run -f net10.0-windows10.0.19041.0
```

#### Testing
**Test Location**: UITest project
**Run Command**:
```bash
.\run-debug-mobile.ps1
```

### ChronoVoid.Unity6Client
**Configuration File**: Assembly-CSharp.csproj

#### Language & Runtime
**Language**: C#
**Version**: Unity 6000.0.54f1
**Build System**: Unity
**Package Manager**: Unity Package Manager

#### Dependencies
**Main Dependencies**:
- com.unity.collab-proxy (2.5.1)
- com.unity.textmeshpro (3.2.0-pre.11)
- com.unity.timeline (1.8.7)
- com.unity.ugui (2.0.0)
- com.unity.visualscripting (1.9.4)

#### Build & Installation
Build through Unity Editor or using Unity's command-line build tools.

### ChronoVoid.Admin
**Configuration File**: ChronoVoid.Admin.csproj

#### Language & Runtime
**Language**: C#
**Version**: .NET 10.0
**Build System**: .NET SDK
**Package Manager**: NuGet

#### Dependencies
**Main Dependencies**:
- Microsoft.EntityFrameworkCore.Design (9.0.7)
- Npgsql.EntityFrameworkCore.PostgreSQL (9.0.4)
- Radzen.Blazor (7.1.6)
- Reference to ChronoVoid.API project

#### Build & Installation
```bash
cd ChronoVoid.Admin
dotnet run
```

#### Testing
**Test Location**: AdminPanelTest project
**Run Command**:
```bash
dotnet test AdminPanelTest
```