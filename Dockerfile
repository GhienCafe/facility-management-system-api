FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

WORKDIR /app

COPY . .


RUN dotnet build Services/API_Admin/API_Admin.csproj -c Release
RUN dotnet build Buildings/AppCore/AppCore.csproj -c Release
RUN dotnet build Buildings/MainData/MainData.csproj -c Release


FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime


WORKDIR /app

COPY --from=build /app/Services/API_Admin/bin/Release/net7.0 Services/API_Admin
COPY --from=build /app/Buildings/AppCore/bin/Release/net7.0 Buildings/AppCore
COPY --from=build /app/Buildings/MainData/bin/Release/net7.0 Buildings/MainData


# Chạy ứng dụng
ENTRYPOINT ["dotnet", "Services/API_Admin/API_Admin.dll"]

