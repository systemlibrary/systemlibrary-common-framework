# License 

## Install msys64
winget install -e --id MSYS2.MSYS2

## Install g++
Start msys64 shell:
pacman -Syu --noconfirm
pacman -S --noconfirm mingw-w64-x86_64-gcc
pacman -S --noconfirm mingw-w64-x86_64-toolchain

./g++ --version


## Build
From MSYS64 shell:
cd /mingw64/bin

./g++ -shared -o "C:/syslib/systemlibrary-common-framework/source/systemlibrary.common.framework.license/SystemLibrary.Common.Framework.LicenseEncKey.dll" "C:/syslib/systemlibrary-common-framework/source/systemlibrary.common.framework.license/licenseEncKey.cpp" -fPIC -nodefaultlibs -nostartfiles

./g++ -shared -o "C:/syslib/systemlibrary-common-framework/source/systemlibrary.common.framework.license/SystemLibrary.Common.Framework.LicenseEncKey.so" "C:/syslib/systemlibrary-common-framework/source/systemlibrary.common.framework.license/licenseEncKey.cpp" -fPIC -nodefaultlibs -nostartfiles