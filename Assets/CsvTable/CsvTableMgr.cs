using System;
using System.Collections.Generic;

public static class CsvTableMgr
{
    private static Dictionary<Type, CsvTable> tables = new Dictionary<Type, CsvTable>();

    static CsvTableMgr()
    {
        tables.Clear();

        tables.Add(typeof(StringTable), new StringTable());

        tables.Add(typeof(WeaponTable), new WeaponTable());

        tables.Add(typeof(ArmorTable), new ArmorTable());

        tables.Add(typeof(MatTable), new MatTable());

        tables.Add(typeof(CraftTable), new CraftTable());

        tables.Add(typeof(CodeTable), new CodeTable());

        tables.Add(typeof(SkillTable), new SkillTable());

        tables.Add(typeof(StageTable), new StageTable());
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
