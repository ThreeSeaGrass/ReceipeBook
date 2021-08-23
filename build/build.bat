cd ../src/ReceipeBook.api

dotnet restore
dotnet build no-restore
dotnet publish -o ../../deploy