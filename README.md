**This project is built in three parts:**

- **Importer**

Importer is used to download and unzip bulk archive ZIP files of public companies from the [U.S. Securities and Exchange Commision website](https://www.sec.gov/edgar/sec-api-documentation).

[Configuration for Importer](Magic%20Formula%20App/Importer/README.md)

- **Updater**

Updater is responsible for making sense of the data from the previous step. It establishes a connection to a local database and stores relevant data.

Importer and Updater are both background services apps that can run indefinitely on the background and do their job to download and process new daily filings.

[Configuration for Updater](Magic%20Formula%20App/Updater/README.md)

- **Magic Formula App (frontend)**

The app itself will consume the data stored in the database and provide meaningful screens to find attractive stocks to invest into.

[Configuration for the frontend](Magic%20Formula%20App/Magic%20Formula%20App/README.md)

[Useful information and commands for this project](Magic%20Formula%20App/Shared/README.md)

### Other information

How last twelve months operating income is calculated according to [this website](https://www.wallstreetprep.com/knowledge/last-twelve-months-ltm/).

![alt text](Magic%20Formula%20App/Images/Last-Twelve-Months-LTM-Formula.jpg)
