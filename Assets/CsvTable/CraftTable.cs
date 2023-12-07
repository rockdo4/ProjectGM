using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

public class CraftTable : CsvTable
{
    public class Data
    {
        public Equip.EquipType _class;
        public int mf_module { get; set; }
        public int number_1 { get; set; }
        public int mon_core { get; set; }
        public int number_2 { get; set; }
        public int lvup_module { get; set; }
        public int number_3 { get; set; }
        public int ingredients { get; set; }
        public int number_4 { get; set; }
        public int gold { get; set; }

        public Data(int _class,
            int mf_module, int number_1,
            int mon_core, int number_2,
            int lvup_module, int number_3,
            int ingredients, int number_4,
            int gold)
        {
            this._class = (Equip.EquipType)_class;
            this.mf_module = mf_module;
            this.number_1 = number_1;
            this.mon_core = mon_core;
            this.number_2 = number_2;
            this.lvup_module = lvup_module;
            this.number_3 = number_3;
            this.ingredients = ingredients;
            this.number_4 = number_4;
            this.gold = gold;
        }
    }
    public Dictionary<int, Data> dataTable = new Dictionary<int, Data>();

    public CraftTable()
    {
        path = "tables/craft_table";
        Load();
    }

    public override void Load()
    {
        var csvStr = Resources.Load<TextAsset>(path);
        TextReader reader = new StringReader(csvStr.text);
        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false
        };

        var csv = new CsvReader(reader, csvConfig);

        csv.Read(); // 타입 제거
        csv.Read(); // 헤더 제거

        while (csv.Read())
        {
            dataTable.Add(csv.GetField<int>(0),
                new Data
                (
                    csv.GetField<int>(1), // _class
                    csv.GetField<int>(2), // mf_module
                    csv.GetField<int>(3), // number_1
                    csv.GetField<int>(4), // mon_core
                    csv.GetField<int>(5), // number_2
                    csv.GetField<int>(6), // lvup_module
                    csv.GetField<int>(7), // number_3
                    csv.GetField<int>(8), // ingredients
                    csv.GetField<int>(9), // number_4
                    csv.GetField<int>(10) // gold
                )
            );
        }
    }
}