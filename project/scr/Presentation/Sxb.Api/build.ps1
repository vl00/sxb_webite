dotnet publish -c release -o ./publish
Compress-Archive ./publish/* -DestinationPath ./release.zip -Force
Remove-Item .\publish -Force -Recurse






