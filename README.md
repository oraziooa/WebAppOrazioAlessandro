# WebAppOrazioAlessandro – Backend .NET 8

##Descrizione

Web app backend sviluppata in **.NET 8** con **Entity Framework Core** e database **PostgreSQL**.

L’applicazione espone un set di **REST API** per la gestione dell’anagrafica di:

- Padiglione
- Settore
- Categoria Merceologica
- Stand

Include:

- Autenticazione e autorizzazione tramite ASP.NET Core Identity
- Gestione ruoli (Admin, Supervisor, User)
- CRUD completo
- Delete gestito come Background Job con Hangfire
- Sistema di notifiche real-time con SignalR
- Face Detection API tramite OpenCV
- Documentazione API con Swagger


--------------------------------

## Tecnologie Utilizzate

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- ASP.NET Core Identity
- Hangfire (Background Jobs)
- SignalR (Notifiche real-time)
- OpenCV (Face Detection)
- Swagger (OpenAPI)

## Requisiti di Sistema

- .NET 8 SDK
- PostgreSQL (io ho installato la v18)
- Visual Studio 2022 oppure CLI .NET

## Configurazione Database

Creare il database 

sql CREATE DATABASE FieraDb

Verificare la stringa di connessione in appsettings.json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=FieraDb;Username=postgres;Password=YOUR_PASSWORD"
}

Applicare le migration:

Da Visual Studio (Package Manager Console)
Update-Database

Oppure via CLI:

dotnet ef database update

Avviare l'app.
Si aprirà lo swagger con le api esposte.

--
per dashboard hangfire 
https://localhost:7112/hangfire

https://localhost:7112/swagger/index.html
--

Utente di default verrà creato al primo avvio
Ruolo		Email			Password
Admin		admin@admin.com		Admin123!

Autenticazione e Ruoli

Per utilizzare le API protette:

Effettuare il login tramite endpoint /api/Auth/login

Copiare il token JWT restituito

Inserire il token nella sezione Authorize di Swagger

I nuovi utenti registrati tramite /api/Auth/register assumono automaticamente il ruolo User.

L’assegnazione dei ruoli Admin o Supervisor può essere effettuata solo da un utente con ruolo Admin.
--------------------------------------
Notifiche Real-Time (SignalR)

È incluso un client console (WebAppOrazioAlessandro.Client) che si connette all’Hub SignalR.

Per testare le notifiche:

Avviare l’API

Avviare il progetto console client

Eseguire operazioni CRUD dall’API

Le notifiche verranno visualizzate nella console
