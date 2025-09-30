#!/bin/bash

# Функция для проверки доступности базы данных с помощью psql
wait_for_db() {
    echo "Ожидание доступности PostgreSQL..."
    until pg_isready -h db -p 5432 -U postgres; do
        echo "PostgreSQL недоступен, жду..."
        sleep 2
    done
    echo "PostgreSQL доступен!"
}

# Альтернатива: используем ping (если нет pg_isready)
wait_for_db_ping() {
    echo "Ожидание доступности PostgreSQL..."
    while ! timeout 1 bash -c 'cat < /dev/null > /dev/tcp/db/5432' 2>/dev/null; do
        echo "PostgreSQL недоступен, жду..."
        sleep 2
    done
    echo "PostgreSQL доступен!"
}

# Альтернатива: используем .NET для проверки
wait_for_db_dotnet() {
    echo "Ожидание доступности PostgreSQL..."
    until dotnet /app/waitfordb.dll "Host=db;Port=5432;Database=user-messenger;Username=postgres;Password=1234"; do
        echo "PostgreSQL недоступен, жду..."
        sleep 2
    done
    echo "PostgreSQL доступен!"
}

# Ожидаем доступности базы данных
wait_for_db_ping

# Применяем миграции
echo "Применение миграций..."
dotnet ef database update --project ServerMessenger.Presentation.csproj --verbose

# Запускаем приложение
echo "Запуск приложения..."
exec dotnet ServerMessenger.Presentation.dll
