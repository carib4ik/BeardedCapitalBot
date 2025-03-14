# Используем официальный образ .NET Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Копируем собранные файлы из папки "out" в контейнер
COPY out . 

# Запускаем приложение
ENTRYPOINT ["dotnet", "BeardedCapitalBot.dll"]
