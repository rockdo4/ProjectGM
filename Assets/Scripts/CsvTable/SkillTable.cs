using CsvHelper.Configuration;
using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class SkillTable : CsvTable
{
    public class Data
    {
        public int name { get; set; }
        public int type { get; set; }
        public int info { get; set; }
        public float value { get; set; }
        public int max_lv { get; set; }

        public Data(int name, int type, int info, float value, int max_lv)
        {
            this.name = name;
            this.type = type;
            this.info = info;
            this.value = value;
            this.max_lv = max_lv;
        }
    }
    public Dictionary<int, Data> dataTable = new Dictionary<int, Data>();

    public SkillTable()
    {
        path = "tables/skill_table";
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
                    csv.GetField<float>(4),
                    csv.GetField<int>(5)
                )
            );
        }
    }
}
