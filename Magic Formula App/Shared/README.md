Migrations Commands

dotnet ef migrations add InitialCreate --context SqlServerCompanyData --output-dir Migrations/SqlServerMigrations
dotnet ef migrations add InitialCreate --context SqliteCompanyData --output-dir Migrations/SqliteMigrations
dotnet ef migrations add InitialCreate --context PostgresCompanyData --output-dir Migrations/PostgresMigrations

dotnet ef migrations remove --context SqlServerCompanyData
dotnet ef migrations remove --context SqliteCompanyData
dotnet ef migrations remove --context PostgresCompanyData

dotnet ef database update --context SqlServerCompanyData
dotnet ef database update --context SqliteCompanyData
dotnet ef database update --context PostgresCompanyData

Unapply migrations:
dotnet ef database update 0 --context SqlServerCompanyData
dotnet ef database update 0 --context SqliteCompanyData
dotnet ef database update 0 --context PostgresCompanyData

For SQL Server Management Studio or other programs, use this string to open the server connection: (localdb)\MSSQLLocalDB


What the project does
Why the project is useful
How users can get started with the project
Where users can get help with your project
Who maintains and contributes to the project