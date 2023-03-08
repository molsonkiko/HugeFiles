: this script requires 7-zip to be installed on your computer
: sync testfiles with ones in repo
xcopy .\testfiles .\Hugefiles\bin\Release-x64\testfiles\ /s /y
xcopy .\testfiles .\Hugefiles\bin\Release\testfiles\ /s /y
: zip testfiles and dlls to release zipfiles
: also copy directories to Downloads for easy access later
cd Hugefiles\bin\Release-x64
xcopy . "%userprofile%\Downloads\HugeFiles NEWEST x64\" /s /y
7z -r a ..\..\..\Release_x64.zip Hugefiles.dll testfiles
cd ..\Release
xcopy . "%userprofile%\Downloads\HugeFiles NEWEST x86\" /s /y
7z -r a ..\..\..\Release_x86.zip Hugefiles.dll testfiles
cd ..\..\..