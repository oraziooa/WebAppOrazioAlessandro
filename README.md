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

### Creare il database 

```sql CREATE DATABASE webapp_fiera;

Modificare nell'appsettings.json il valore di "DefaultConnection" inserendop la stringa di connessione al db

Su Visual Studio 2022
Da Package Manager Console eseguire :
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

per richiamare le api bisognerà registrarsi o effettuare il login per avere il token jwt da copiare e incollare nella box Authorize dello swagger.
I nuovi utenti registrati assumeranno il ruolo User.
Sarà possibile assegnare un ruolo (Admin o Supervisor) al nuovo utente user solo se l'utente che effettua la chiamata avra il ruolo di admin. Quindi dovrà essere inserito in Authorize il token dell admin.

Per le notifiche (SignalR) ho creato un piccolo client console (WebAppOrazioAlessandro.Client) da avviare insieme all'app web in cui verranno visualizzate le notifiche inviate durante le operazioni crud sulla console.
