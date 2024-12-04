# Flight Scanner

## Usage
1. Use CLI project to generate database file locally with IATA codes from [Wikipedia](https://en.wikipedia.org/wiki/List_of_airports_by_IATA_code:_A) page. This file can also be downloaded from GitHub action artifacts after running action 'Upload database'.

## Architecture
- **CLI project** - it is responsible for scraping data from wikipedia page and exporting it to database file. This is done since IATA codes sholdn't be taken from resource that almost everyone can edit. This database file can afterwards be used on server side of application.
    - CLI application has clean decoupling of input received from CLI UI and business logic, which enables good testability.
    - SQLite is used as database because of simlicity. There is no need for separate server installation, it's stored in single file which simplifies deployment and enhances it's portability. It also has support for Entity Framework which will afterwards be used as database manager.
- **Domain** - contains entities and abstractions for business services.
- **Persistence** - implements communication to SQLite database.
    - implements Repository pattern to decouple access to database from application logic.
- **Application** - contains implementations for domain service abstractions, repositories, CQRS, caching...
- **Web API** - used for communication with flight APIs by controllers, and for communication to client application.
    - uses CQRS pattern for decoupling controllers from outer infrastructure code.
    - uses Repository pattern for decoupling access to database.
    - contains various middleware: caching, validation of input IATA code, invalid request oarameter to database filter, URI disconitinuation filter
- **UI** - use Blazor Web Assembly as frontend technology