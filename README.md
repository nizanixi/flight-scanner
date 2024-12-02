# Flight Scanner

## Usage
1. Use CLI project to generate database file with IATA codes from [Wikipedia](https://en.wikipedia.org/wiki/List_of_airports_by_IATA_code:_A) page

## Architecture
- **CLI project** - it is responsible for scraping data from wikipedia page and exporting it to database file. This is done since IATA codes sholdn't be taken from resource that almost everyone can edit. This database file can afterwards be used on server side of application.
    - CLI application has clean decoupling of input received from CLI UI and business logic, which enables good testability.
    - SQLite is used as database because of simlicity. There is no need for separate server installation, it's stored in single file which simplifies deployment and enhances it's portability. It also has support for Entity Framework which will afterwards be used as database manager.

# Possible improvements
- Implement deploymnet for CLI project so that database file is uploaded to git repo