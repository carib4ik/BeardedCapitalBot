# Используем официальный .NET SDK образ для сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Копируем проект и восстанавливаем зависимости
COPY . .
RUN dotnet restore

# Собираем проект и публикуем в папку out
RUN dotnet publish -c Release -o /app/out

# Запускаем контейнер на более лёгком образе
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

ENV TELEGRAM_BOT_TOKEN=${TELEGRAM_BOT_TOKEN}
ENV MANAGER_CHANNEL_ID=${MANAGER_CHANNEL_ID}
ENV SUBSCRIBE_CHANNEL_ID=${SUBSCRIBE_CHANNEL_ID}
ENV ERRORS_LOG_CHANNEL_ID=${ERRORS_LOG_CHANNEL_ID}
ENV ERRORS_FILE_PATH=${ERRORS_FILE_PATH}

# Запускаем приложение
CMD ["dotnet", "BeardedCapitalBot.dll"]
