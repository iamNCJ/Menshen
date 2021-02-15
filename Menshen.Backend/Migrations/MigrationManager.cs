using System;
using System.Linq;
using Dapper;
using Menshen.Backend.Migrations.Routines;
using Menshen.Backend.Services;
using Microsoft.Extensions.Logging;

namespace Menshen.Backend.Migrations
{
    public class MigrationManager
    {
        private readonly IAppDb _db;
        private readonly ILogger<MigrationManager> _logger;

        private static readonly Type[] MigrationTypes =
        {
            typeof(InitDb),
        };

        public MigrationManager(IServiceProvider serviceProvider)
        {
            _db = (IAppDb) serviceProvider.GetService(typeof(IAppDb));
            _logger = (ILogger<MigrationManager>) serviceProvider.GetService(typeof(ILogger<MigrationManager>));
        }

        public void Migrate()
        {
            var conn = _db.GetDbConnection();
            conn.Open();
            // get current db version
            // test if the table exists
            var dbVersion = 0;
            var hasMetaTable = conn.Query(@"SELECT name FROM sqlite_master WHERE name = 'META_INFO';").Any();
            if (hasMetaTable)
            {
                dbVersion = conn.ExecuteScalar<int?>(@"SELECT `Value` FROM META_INFO WHERE `Key` = 'DB_VERSION';") ?? 0;
            }
            
            // run migrations
            var versionType = typeof(MigrationVersion);
            foreach (var migrationType in MigrationTypes)
            {
                try
                {
                    var version = ((MigrationVersion) System.Attribute.GetCustomAttribute(migrationType, versionType))
                        .version;
                    if (version == dbVersion + 1)
                    {
                        dbVersion++;
                        var routine = (IMigrationRoutine) Activator.CreateInstance(migrationType);
                        routine.Migrate(conn);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogCritical($"Failed to run migration for {versionType.Name}: {e.Message}");
                    conn.Close();
                    Environment.Exit(-1);
                }
            }
            
            conn.Close();
        }
    }
}