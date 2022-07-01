BUILDNAME=capslock-fix
PROJECTNAME=$BUILDNAME.csproj

dotnet publish ./src/$PROJECTNAME -r linux-x64 --self-contained -c Release -o ./build/ /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true /property:GenerateFullPaths=true /consoleloggerparameters:NoSummary /p:DebugType=None
cp -f ./build/$BUILDNAME $HOME/.local/bin/
cp -f ./capslock-fix.desktop $HOME/.config/autostart/