FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

WORKDIR /app

COPY . .


RUN dotnet build Services/API_FFMS/API_FFMS.csproj -c Release
RUN dotnet build Buildings/AppCore/AppCore.csproj -c Release
RUN dotnet build Buildings/MainData/MainData.csproj -c Release
RUN dotnet build Workers/Worker_Notify/Worker_Notify.csproj -c Release

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime


WORKDIR /app

COPY --from=build /app/Services/API_FFMS/bin/Release/net7.0 Services/API_FFMS
COPY --from=build /app/Buildings/MainData/bin/Release/net7.0 Buildings/MainData
COPY --from=build /app/Buildings/AppCore/bin/Release/net7.0 Buildings/AppCore
COPY --from=build /app/Workers/Worker_Notify/bin/Release/net7.0 Workers/Worker_Notify



# Chạy ứng dụng
ENTRYPOINT ["dotnet", "Workers/Worker_Notify/Worker_Notify.dll"]
#ENTRYPOINT ["dotnet", "/app/out/API_FFMS.dll"]
#CMD ["dotnet", "run", "-p", "Services/API_FFMS", "--", "--port", "5000"]

