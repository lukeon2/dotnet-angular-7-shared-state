# ASP.NET Core + Angular 7 sample application
This sample focuses on sharing data and state between Web app and console app. It's simulator of vending machine where user can insert coins and buy products.

## About
- There are 2 startup projects (Console and Web) + Test project + shared library.
- Console app: .NET Core 2.1 console app.
- Web app: ASP.NET Core 2.1 with Angular 7.
- Both projects are using Entity Framework Core as ORM and data access.
- Application by default is using SQL Server Express LocalDB, if needed it's possible to change this and use different SQL instance. 
- After opening solution in Visual Studio it should be possible to run any of project just by pressing F5. The only requirement it's to have node.js installed on machine, rest dependencies should be restored from nuget and npm.
- It's also possible to build and run apps by using dotnet CLI

## Configuration
Connection strings are defined in "appsettings.json#ConnectionStrings" file.

## Running
After cloning or downloading the sample it should be possible to run it.
The first time it will seed database with data such that you should see products, and you should be to interact with console or web app.

### How console app looks:
![Console app screen](https://i.imgur.com/5rsFsi1.png)


### Web app:
![Web app screen](https://i.imgur.com/lrfubvk.png)
