using CsvHelper.Configuration;
using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class MatTable : CsvTable
{
    public class Data
    {
        public int item_name { get; set; }
        public int item_script { get; set; }
        public int gold { get; set; }

        public Data(int item_name, int item_script, int gold)
        {
            this.item_name = item_name;
            this.item_script = item_script;
            this.gold = gold;
        }
    }
    public Dictionary<int, Data> dataTable = new Dictionary<int, Data>();

    public MatTable() 
    {
        path = "tables/item_table";
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
            dataTable.Add(int.Parse(csv.GetField(0)),
                new Data
                (
                    csv.GetField<int>(1), // item_name
                    csv.GetField<int>(2), // item_script
                    csv.GetField<int>(3) // gold
                )
            );
        }
    }
}