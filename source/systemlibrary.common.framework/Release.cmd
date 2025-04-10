dotnet build SystemLibrary.Common.Framework.csproj -c Release
dotnet pack SystemLibrary.Common.Framework.csproj -p Configuration=Release /p:IncludeBuildOutput=false

pause

