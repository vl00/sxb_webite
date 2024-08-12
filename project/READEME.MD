# Dockerfile 
用于构造netcore sdk 镜像，基镜像为netcore3.1，构造该镜像的目的是为了将项目对Nuget包的依赖缓存下来，降低以此镜像为sdk编译所需的时间。镜像目前托管在阿里云registry.cn-shenzhen.aliyuncs.com/jamesrepo/netcore仓库当中。