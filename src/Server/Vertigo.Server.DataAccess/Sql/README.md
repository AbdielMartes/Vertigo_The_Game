# SQL Server schema updates

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
