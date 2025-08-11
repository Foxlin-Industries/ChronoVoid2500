# Setup SSH for Foxlin-Industries GitHub Account
# This script creates SSH keys and config for local directory use only

param (
    [string]$Email = "your-email@example.com",
    [string]$KeyName = "foxlin_industries_chronovoid"
)

Write-Host "Setting up SSH for Foxlin-Industries GitHub account..." -ForegroundColor Green

# Create .ssh directory in current project if it doesn't exist
$sshDir = Join-Path $PWD ".ssh"
if (!(Test-Path $sshDir)) {
    New-Item -ItemType Directory -Path $sshDir -Force
    Write-Host "Created .ssh directory in project root" -ForegroundColor Yellow
}

# Generate SSH key pair
$keyPath = Join-Path $sshDir $KeyName
Write-Host "Generating SSH key pair..." -ForegroundColor Yellow
ssh-keygen -t ed25519 -C $Email -f $keyPath -N '""'

# Create SSH config file for this project
$configPath = Join-Path $sshDir "config"
$configContent = @"
# SSH Config for Foxlin-Industries ChronoVoid2500 Project
Host github-foxlin
    HostName github.com
    User git
    IdentityFile $keyPath
    IdentitiesOnly yes
"@

Set-Content -Path $configPath -Value $configContent
Write-Host "Created SSH config file" -ForegroundColor Yellow

# Display the public key
$publicKeyPath = "$keyPath.pub"
if (Test-Path $publicKeyPath) {
    Write-Host "`nYour SSH Public Key (add this to GitHub):" -ForegroundColor Green
    Write-Host "=" * 60 -ForegroundColor Gray
    Get-Content $publicKeyPath
    Write-Host "=" * 60 -ForegroundColor Gray
    
    # Copy to clipboard
    Get-Content $publicKeyPath | Set-Clipboard
    Write-Host "`nPublic key copied to clipboard!" -ForegroundColor Green
}

# Set up Git config for this repository
Write-Host "`nSetting up Git configuration for this repository..." -ForegroundColor Yellow
git config user.name "Foxlin-Industries"
git config user.email $Email

# Create .gitignore entry for SSH keys
$gitignorePath = Join-Path $PWD ".gitignore"
if (!(Test-Path $gitignorePath)) {
    New-Item -ItemType File -Path $gitignorePath
}

$gitignoreContent = @"

# SSH Keys (local only)
.ssh/
"@

Add-Content -Path $gitignorePath -Value $gitignoreContent

Write-Host "`nNext Steps:" -ForegroundColor Cyan
Write-Host "1. Go to https://github.com/Foxlin-Industries/ChronoVoid2500/settings/keys" -ForegroundColor White
Write-Host "2. Click 'New SSH key'" -ForegroundColor White
Write-Host "3. Paste the public key above" -ForegroundColor White
Write-Host "4. Give it a title like 'ChronoVoid2500 Local Dev'" -ForegroundColor White
Write-Host "5. Click 'Add SSH key'" -ForegroundColor White
Write-Host "`nAfter adding the key, you can clone using:" -ForegroundColor Green
Write-Host "git clone git@github-foxlin:Foxlin-Industries/ChronoVoid2500.git" -ForegroundColor Yellow

Write-Host "`nSSH setup complete for Foxlin-Industries!" -ForegroundColor Green
