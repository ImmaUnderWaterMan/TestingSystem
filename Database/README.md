# SQL Scripts для базы данных TestingSystem

## Как использовать:

### Вариант 1: Через SQL Server Management Studio (SSMS)

1. Подключись к SQL Server
2. Открой файл `001_CreateTables.sql`
3. Выполни (F5)
4. Открой файл `002_InsertSeedData.sql`
5. Выполни (F5)

### Вариант 2: Через sqlcmd (командная строка)

```bash
sqlcmd -S localhost -U sa -P YourPassword -i Database/001_CreateTables.sql
sqlcmd -S localhost -U sa -P YourPassword -i Database/002_InsertSeedData.sql