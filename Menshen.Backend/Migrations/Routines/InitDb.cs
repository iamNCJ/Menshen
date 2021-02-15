using System.Data.Common;
using Dapper;

namespace Menshen.Backend.Migrations.Routines
{
    [MigrationVersion(1)]
    public class InitDb : IMigrationRoutine
    {
        public void Migrate(DbConnection conn)
        {
            conn.Execute(@"
CREATE TABLE META_INFO (
id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
key TEXT NOT NULL,
value TEXT
);

CREATE TABLE WHITELIST (
  id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  ip TEXT NOT NULL,
  type integer NOT NULL,
  valid_until integer NOT NULL
);

CREATE TABLE BLACKLIST (
  id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  ip TEXT NOT NULL,
  count integer NOT NULL
);

CREATE TABLE USER (
  id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  name TEXT NOT NULL,
  type integer NOT NULL,
  secret TEXT NOT NULL
);

VACUUM;
");
        }
    }
}