FROM registry.cn-shenzhen.aliyuncs.com/jamesrepo/netcore:sdk3.1 AS publish
WORKDIR /src
RUN dotnet nuget add source http://192.168.31.11:10888/nuget -n sxbsource
COPY ["scr","scr/"]
COPY ["libs","libs"]
RUN dotnet build "/src/scr/Presentation/CommentApp/Sxb.Web.csproj" -c Release -o /app/build
RUN  rm -rf /app && rm -rf /src