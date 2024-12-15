# .NET SDK imajını kullanarak derleme aşaması
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Proje dosyalarını kopyala ve bağımlılıkları geri yükle
COPY *.csproj ./
RUN dotnet restore

# Tüm kaynak kodunu kopyala ve projeyi derle
COPY . ./
RUN dotnet publish -c Release -o /out

# Çalışma zamanı imajı oluştur
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "ProductWebApi.dll"]