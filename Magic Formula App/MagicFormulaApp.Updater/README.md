## Updater Configuration

These are the configuration values that you need to set.


I suggest to set ***CompanyFactsFolder*** to the **LocalData** folder on your system to not need administrator privileges.

I suggest to leave ***BatchSize*** and ***DefaultConnection*** connection string to what it is.


```
{
  "UserAgent": "Sample Company Name AdminContact@<sample company domain>.com",
  "CompanyFactsFolder": "C:\\Users\\aldol\\AppData\\Local\\companyfacts",
  "BatchSize": "100",
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MagicFormulaApp;Trusted_Connection=True;"
  }
}
```
