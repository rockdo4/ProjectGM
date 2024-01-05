using CsvHelper.Configuration;
using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class CodeTable : CsvTable
{
    public class Data
    {
        public int type { get; set; }
        public int name { get; set; }
        public int script { get; set; }
        public int skill1_id { get; set; }
        public int skill1_lv { get; set; }
        public int skill2_id { get; set; }
        public int skill2_lv { get; set; }
        public int sellgold { get; set; }

        public Data(int type, int name, int script,
            int skill1_id, int skill1_lv,
            int skill2_id, int skill2_lv,
            int sellgold)
        {
            this.type = type;
            this.name = name;
            this.script = script;
            this.skill1_id = skill1_id;
            this.skill1_lv = skill1_lv;
            this.skill2_id = skill2_id;
            this.skill2_lv = skill2_lv;
            this.sellgold = sellgold;
        }
    }
    public Dictionary<int, Data> dataTable = new Dictionary<int, Data>();

    public CodeTable()
    {
        path = "tables/skillcode_table";
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
                    csv.GetField<int>(1),
                    csv.GetField<int>(2),
                    csv.GetField<int>(3),
                    csv.GetField<int>(4),
                    csv.GetField<int>(5),
                    csv.GetField<int>(6),
                    csv.GetField<int>(7),
                    csv.GetField<int>(8)
                )
            );
        }
    }
}