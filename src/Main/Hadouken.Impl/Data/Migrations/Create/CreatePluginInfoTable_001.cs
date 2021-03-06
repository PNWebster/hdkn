﻿using Migrator.Framework;

namespace Hadouken.Impl.Data.Migrations.Create
{
    [Migration(201206251201)]
    public class CreatePluginInfoTable_001 : Migration
    {
        public override void Up()
        {
            Database.AddTable("PluginInfo",
                new Column("Id", System.Data.DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                new Column("Path", System.Data.DbType.String)
            );
        }

        public override void Down()
        {
            Database.RemoveTable("PluginInfo");
        }
    }
}
