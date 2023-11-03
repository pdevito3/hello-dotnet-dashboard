# hellodash

This project was created with [Craftsman](https://github.com/pdevito3/craftsman).

## Getting Started
1. Run `docker-compose up` from your `.sln` directory to spin up your database. Migrations will be ran automatically when staritng the db.

### Running Your Project(s)
Once you have your database(s) running, you can run your API(s), BFF, and Auth Servers by using 
the `dotnet run` command or running your project(s) from your IDE of choice.   

### Migrations
Migrations should be applied for you automatically on startup, but if you have any any issues, you can do the following:
    1. Make sure you have a migrations in your boundary project (there should be a `Migrations` directory in the project directory). 
    If there isn't see [Running Migrations](#running-migrations) below.
    2. Confirm your environment (`ASPNETCORE_ENVIRONMENT`) is set to `Development` using 
    `$Env:ASPNETCORE_ENVIRONMENT = "Development"` for powershell or `export ASPNETCORE_ENVIRONMENT=Development` for bash.
    3. `cd` to the boundary project root (e.g. `cd RecipeManagement/src/RecipeManagement`)
    4. Run your project and your migrations should be applied automatically. Alternatively, you can run `dotnet ef database update` to apply your migrations manually.

    > You can also stay in the `sln` root and run something like `dotnet ef database update --project RecipeManagement/src/RecipeManagement`

## Running Integration Tests
To run integration tests:

1. Ensure that you have docker installed.
2. Go to your src directory for the bounded context that you want to test.
3. Confirm that you have migrations in your infrastructure project. If you need to add them, see the [instructions below](#running-migrations).
4. Run the tests

> ⏳ If you don't have the database image pulled down to your machine, they will take some time on the first run.

### Troubleshooting
- If your entity has foreign keys, you might need to adjust some of your tests after scaffolding to accomodate them.

## Running Migrations
To create a new migration, make sure your environment is set to `Development`:

### Powershell
```powershell
$Env:ASPNETCORE_ENVIRONMENT = "Development"
```

### Bash
```bash
export ASPNETCORE_ENVIRONMENT=Development
```

Then run the following:

```shell
cd YourBoundedContextName/src/YourBoundedContextName
dotnet ef migrations add "MigrationDescription"
```

To apply your migrations to your local db, make sure your database is running in docker run the following:

```bash
cd YourBoundedContextName/src/YourBoundedContextName
dotnet ef database update
```
