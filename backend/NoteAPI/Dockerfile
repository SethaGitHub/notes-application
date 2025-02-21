FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["NoteAPI.csproj", "./"]
RUN dotnet restore "NoteAPI.csproj"

RUN dotnet add "NoteAPI.csproj" package Microsoft.EntityFrameworkCore.SqlServer
RUN dotnet add "NoteAPI.csproj" package Microsoft.EntityFrameworkCore.Design

COPY . .
WORKDIR "/src/."
RUN dotnet build "NoteAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NoteAPI.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "NoteAPI.dll"]
