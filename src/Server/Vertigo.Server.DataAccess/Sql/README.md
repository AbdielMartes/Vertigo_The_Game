# SQL Server schema updates

To apply both scripts through `VertigoDatabaseContext`, run the host once in
provisioning mode against the existing database using a provisioning connection
as `ConnectionStrings__VertigoDb`.

```sh
dotnet run --project src/Server/Vertigo.Server.Host -- --DatabaseSchema:UpgradeOnly=true
```

This mode exits after applying the scripts. Start the server normally with the
restricted application connection and without the provisioning flag.

Run `001_ReducePlayerRegistrationLengths.sql` against the existing Vértigo database
before deploying the registration update. For a fresh database, load the team's
base schema first. The script expects `dbo.Players` with separate unique constraints
on `Username` and `Email`.

The script keeps existing accounts and stops if any value exceeds the new column
capacity. To find the affected accounts, run this query in the same database.

```sql
SELECT PlayerId, DATALENGTH(Username) / 2 AS UsernameLength, DATALENGTH(Email) / 2 AS EmailLength
FROM dbo.Players
WHERE DATALENGTH(Username) > 40 OR DATALENGTH(Email) > 140;
```

Resolve those accounts before rerunning the script. A successful run changes the
columns to `nvarchar(20)` and `nvarchar(70)` and preserves uniqueness. A second run
against the updated schema exits successfully.

## Player profile

After applying the base schema and 001, run `002_AddPlayerCountryCode.sql` with
the provisioning account before starting the updated server. It adds the optional
`dbo.Players.CountryCode` column. Existing accounts keep their creation date and
avatar, and their country stays unspecified. Rerunning the script skips the column
when it already exists.
