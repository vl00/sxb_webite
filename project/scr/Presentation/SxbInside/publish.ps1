echo $args
dotnet publish -c $args -o ./publish
Compress-Archive .\publish\* -DestinationPath '.\publish.zip' -f
Remove-Item  -Recurse .\publish\ -Force 
