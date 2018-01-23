Remove-Item -Path .\Migrations -Recurse
Remove-Item -Path .\app.db
dotnet ef migrations add InitModel
dotnet ef database update
Remove-Item -Path .\Migrations -Recurse
