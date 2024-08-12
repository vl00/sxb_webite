dotnet pack -c release -p:PackageVersion=$args
dotnet nuget push .\bin\Release\ProductManagement.Tool.HttpRequest.$args.nupkg -k A8119DF6-AFD1-46AF-8DDD-1C14E2EC2B54 -s http://192.168.31.11:10888/nuget
