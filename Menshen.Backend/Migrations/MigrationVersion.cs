namespace Menshen.Backend.Migrations
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class MigrationVersion : System.Attribute
    {
        public uint version;

        public MigrationVersion(uint version)
        {
            this.version = version;
        }
    }
}