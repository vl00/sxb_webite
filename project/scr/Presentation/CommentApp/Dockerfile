FROM registry.cn-shenzhen.aliyuncs.com/jamesrepo/netcore AS publish
WORKDIR /src
COPY ["scr","scr/"]
COPY ["libs","libs"]
RUN dotnet publish "/src/scr/Presentation/CommentApp/Sxb.Web.csproj" -c Release -o /app/publish

FROM registry.cn-shenzhen.aliyuncs.com/jamesrepo/netcore:runtime3.1_libgdi
WORKDIR /app
COPY --from=publish /app/publish .
ENV TZ="Asia/Shanghai"
ENTRYPOINT ["dotnet", "Sxb.Web.dll"]
