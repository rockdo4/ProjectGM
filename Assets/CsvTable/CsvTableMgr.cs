using System;
using System.Collections.Generic;

public static class CsvTableMgr
{
    private static Dictionary<Type, CsvTable> tables = new Dictionary<Type, CsvTable>();

    static CsvTableMgr()
    {
        tables.Clear();

        /*
        var upgradeTable = new UpgradeTable();
        tables.Add(typeof(UpgradeTable), upgradeTable);

        var monsterTable = new MonsterTable();
        tables.Add(typeof(MonsterTable), monsterTable);

        var arsenalTable = new ArsenalTable();
        tables.Add(typeof(ArsenalTable), arsenalTable);
        */
    }

    public static T GetTable<T>() where T : CsvTable
    {
        var id = typeof(T);
        if (!tables.ContainsKey(id))
        {
            return null;
        }

        return tables[id] as T;
    }
}
