#!/bin/bash

# AppImageHub Submission Helper Script
# This script helps prepare files for AppImageHub submission

set -e

echo "🚀 AppImageHub Submission Preparation"
echo "======================================"
echo ""

# Check if appimage.github.io is already forked/cloned
if [ -d ~/appimage.github.io ]; then
    echo "✅ appimage.github.io repository found"
    cd ~/appimage.github.io
    git pull upstream master 2>/dev/null || echo "⚠️  No upstream remote configured"
else
    echo "📥 Cloning your fork of appimage.github.io..."
    echo "   Make sure you've forked https://github.com/AppImage/appimage.github.io first!"
    echo ""
    read -p "Enter your GitHub username: " username
    
    if [ -z "$username" ]; then
        echo "❌ Username required"
        exit 1
    fi
    
    git clone "https://github.com/$username/appimage.github.io.git" ~/appimage.github.io
    cd ~/appimage.github.io
    
    # Add upstream remote
    git remote add upstream https://github.com/AppImage/appimage.github.io.git
    git fetch upstream
fi

# Create application directory
APP_DIR="database/Aquiis_Property_Management"
echo ""
echo "📁 Creating directory: $APP_DIR"
mkdir -p "$APP_DIR"

# Copy desktop file
echo "📄 Copying desktop file..."
cp ~/Source/Aquiis/4-Aquiis.SimpleStart/Assets/aquiis.desktop \
   "$APP_DIR/aquiis.desktop"

# Copy icon
echo "🎨 Copying icon..."
cp ~/Source/Aquiis/4-Aquiis.SimpleStart/Assets/icon.png \
   "$APP_DIR/aquiis.png"

# Copy AppStream metadata
echo "📋 Copying AppStream metadata..."
cp ~/Source/Aquiis/4-Aquiis.SimpleStart/Assets/com.aquiis.propertymanagement.appdata.xml \
   "$APP_DIR/com.aquiis.propertymanagement.appdata.xml"

# Copy screenshot (use dashboard as primary)
if [ -f ~/Source/Aquiis/Documentation/Screenshots/dashboard.png ]; then
    echo "📸 Copying screenshot..."
    cp ~/Source/Aquiis/Documentation/Screenshots/dashboard.png \
       "$APP_DIR/screenshot.png"
else
    echo "⚠️  Screenshot not found: ~/Source/Aquiis/Documentation/Screenshots/dashboard.png"
    echo "   You'll need to add screenshots manually"
fi

echo ""
echo "✅ Files prepared in: ~/appimage.github.io/$APP_DIR"
echo ""
echo "📝 Next steps:"
echo "   1. Take screenshots of your application (if not done)"
echo "   2. Run application and capture:"
echo "      - Dashboard (primary screenshot)"
echo "      - Property management interface"
echo "      - Lease workflow"
echo "      - Invoice tracking"
echo "   3. Save screenshots to: ~/Source/Aquiis/Documentation/Screenshots/"
echo "   4. Verify files in: ~/appimage.github.io/$APP_DIR"
echo "   5. Create branch: cd ~/appimage.github.io && git checkout -b add-aquiis"
echo "   6. Commit changes: git add . && git commit -m 'Add Aquiis Property Management'"
echo "   7. Push: git push origin add-aquiis"
echo "   8. Create PR on GitHub: https://github.com/AppImage/appimage.github.io/compare"
echo ""
