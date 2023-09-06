FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

WORKDIR /app

COPY . .


RUN dotnet build Services/API_FFMS/API_FFMS.csproj -c Release
RUN dotnet build Buildings/AppCore/AppCore.csproj -c Release
RUN dotnet build Buildings/MainData/MainData.csproj -c Release

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime


WORKDIR /app

COPY --from=build /app/Services/API_FFMS/bin/Release/net7.0 Services/API_FFMS
COPY --from=build /app/Buildings/AppCore/bin/Release/net7.0 Buildings/AppCore
COPY --from=build /app/Buildings/MainData/bin/Release/net7.0 Buildings/MainData


# Chạy ứng dụng
ENTRYPOINT ["dotnet", "Services/API_FFMS/API_FFMS.dll"]
#ENTRYPOINT ["dotnet", "/app/out/API_FFMS.dll"]
#CMD ["dotnet", "run", "-p", "Services/API_FFMS", "--", "--port", "5000"]

