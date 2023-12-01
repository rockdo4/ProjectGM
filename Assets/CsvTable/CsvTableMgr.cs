using System;
using System.Collections.Generic;

public static class CsvTableMgr
{
    private static Dictionary<Type, CsvTable> tables = new Dictionary<Type, CsvTable>();

    static CsvTableMgr()
    {
        tables.Clear();

        var weaponTable = new WeaponTable();
        tables.Add(typeof(WeaponTable), weaponTable);
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
