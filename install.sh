BUILDNAME=capslock-fix
PROJECTNAME=$BUILDNAME.csproj

dotnet publish ./src/$($PROJECTNAME) -r linux-x64 --self-contained -c Release -o ./build/ /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true /property:GenerateFullPaths=true /consoleloggerparameters:NoSummary
rm -r ./build/*.pdb
cp ./build/$BUILDNAME /home/arthur/.local/bin/
cp ./capslock-fix.desktop /home/arthur/.config/autostart/