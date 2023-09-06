FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

WORKDIR /app

COPY . .


RUN dotnet build -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime


WORKDIR /app

COPY --from=build /app/out ./


# Chạy ứng dụng
ENTRYPOINT ["dotnet", "Services/API_FFMS/bin/Debug/net7.0/API_FFMS.dll"]
#CMD ["dotnet", "run", "-p", "Services/API_FFMS", "--", "--port", "5000"]

