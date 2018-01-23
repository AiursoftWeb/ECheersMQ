rm ./Migrations -rf
rm ./app.db
dotnet ef migrations add InitModel
dotnet ef database update
rm ./Migrations -rf
