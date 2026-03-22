# DiveLogg (projekt i kursen DT191G, Webbutveckling med .NET)
**Namn:** Emma Larsson och Anton Eriksson\
**Student-ID:** emla2309 och aner2308

DiveLogg är en webbapplikation byggd med ASP.NET Core MVC för att registrera och hantera dyk. Applikationen stödjer CRUD-operationer, filtrering, paginering, kartvisualisering och användarautentisering.

**Tekniker:**
- ASP.NET Core MVC
- Entity Framework Core (Code First + Migrations)
- SQLite
- ASP.NET Core Identity
- Leaflet.js

**Funktioner:**
- Hantera dyk
- Hantera personer kopplade till dyk
- Filtrering av listor
- Paginering av listor
- Kartvisualiseing
- Inloggning och autentisering

**Kartvisualisering:**
Kartvisualiseringen är byggt med Leaflet. Vid skapande av ett nytt dyk väljer användaren en position på kartan. Denna position visualiseras sedan på en karta, antingen tillsammans med alla dyk eller för ett enskilt dyk. Denna plats kan även redigeras eller tas bort.

**Autentisering:**
Inloggning krävs för att skapa, redigera och ta bort data. Autentisering och auktorisering hanteras via ASP.NET Core Identity.

**Kom igång:**
1. Klona projektet och gå in i mappen
git clone https://github.com/QmmaLarsson/DiveLogg.git\
cd DiveLogg

2. Installera nödvändiga paket
dotnet restore

3. Skapa databas
dotnet ef database update

4. Starta applikationen
dotnet run
