using CsvHelper.Configuration;
using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class StageTable : CsvTable
{
    public class Data
    {
        public int type { get; set; }
        public int name { get; set; }
        public int script { get; set; }
        public int iconName { get; set; }
        public int map_id { get; set; }
        public int monster_id { get; set; }
        public int time_limit { get; set; }
        public int unlock { get; set; }
        public int gold { get; set; }
        public int clear1 { get; set; }
        public int clear1_count { get; set; }
        public int clear2 { get; set; }
        public int clear2_count { get; set; }
        public int clear3 { get; set; }
        public int clear3_count { get; set; }
        public int clear4 { get; set; }
        public int clear4_count { get; set; }
        public int clear5 { get; set; }
        public int clear5_count { get; set; }

        public Data(int type, int name, int script, int iconName, int map_id, int monster_id, 
            int time_limit, int unlock, int gold, 
            int clear1, int clear1_count, 
            int clear2, int clear2_count, 
            int clear3, int clear3_count, 
            int clear4, int clear4_count,
            int clear5, int clear5_count)
        {
            this.type = type;
            this.name = name;
            this.script = script;
            this.iconName = iconName;
            this.map_id = map_id;
            this.monster_id = monster_id;
            this.time_limit = time_limit;
            this.unlock = unlock;
            this.gold = gold;
            this.clear1 = clear1;
            this.clear1_count = clear1_count;
            this.clear2 = clear2;
            this.clear2_count = clear2_count;
            this.clear3 = clear3;
            this.clear3_count = clear3_count;
            this.clear4 = clear4;
            this.clear4_count = clear4_count;
            this.clear5 = clear5;
            this.clear5_count = clear5_count;
        }
    }
    public Dictionary<int, Data> dataTable = new Dictionary<int, Data>();

    public StageTable()
    {
        path = "tables/stage_table";
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
                    csv.GetField<int>(8),
                    csv.GetField<int>(9),
                    csv.GetField<int>(10),
                    csv.GetField<int>(11),
                    csv.GetField<int>(12),
                    csv.GetField<int>(13),
                    csv.GetField<int>(14),
                    csv.GetField<int>(15),
                    csv.GetField<int>(16),
                    csv.GetField<int>(17),
                    csv.GetField<int>(18),
                    csv.GetField<int>(19)
                )
            );
        }
    }
}
