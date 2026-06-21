#!/bin/bash

# Запускаем SQL Server в фоне
/opt/mssql/bin/sqlservr &
SQL_PID=$!

# Ждём, пока SQL Server запустится
echo "Waiting for SQL Server to start..."
for i in {1..60}; do
    if /opt/mssql-tools18/bin/sqlcmd -S 127.0.0.1 -U sa -P 'YourStrong@Passw0rd' -C -Q "SELECT 1" > /dev/null 2>&1; then
        echo "SQL Server is ready!"
        break
    fi
    echo "Waiting... ($i/60)"
    sleep 3
done

# Проверяем, запустился ли SQL Server
if ! kill -0 $SQL_PID 2>/dev/null; then
    echo "ERROR: SQL Server failed to start"
    exit 1
fi

# Выполняем скрипты создания таблиц
echo "Creating tables..."
/opt/mssql-tools18/bin/sqlcmd -S 127.0.0.1 -U sa -P 'YourStrong@Passw0rd' -C -i /docker-entrypoint-initdb.d/001_CreateTables.sql
if [ $? -eq 0 ]; then
    echo "Tables created successfully!"
else
    echo "WARNING: Failed to create tables"
fi

# Выполняем скрипты заполнения данными
echo "Inserting seed data..."
/opt/mssql-tools18/bin/sqlcmd -S 127.0.0.1 -U sa -P 'YourStrong@Passw0rd' -C -i /docker-entrypoint-initdb.d/002_InsertSeedData.sql
if [ $? -eq 0 ]; then
    echo "Seed data inserted successfully!"
else
    echo "WARNING: Failed to insert seed data"
fi

echo "Database initialization complete!"

# Ждём завершения SQL Server (foreground)
wait $SQL_PID
