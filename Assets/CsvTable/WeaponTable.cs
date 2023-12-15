using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class WeaponTable : CsvTable
{
    public class Data
    {
        public int name { get; set; }
        public int sellgold { get; set; }
        public AttackType property { get; set; }
        public WeaponType type { get; set; }
        public float atk { get; set; }
        public float weakpoint { get; set; }
        public int grade { get; set; }

        public Data(int name, int sellgold,
            int property, int type,
            float atk, float weakpoint, int grade)
        {
            this.name = name;
            this.sellgold = sellgold;
            this.property = (AttackType)property;
            this.type = (WeaponType)type;
            this.atk = atk;
            this.weakpoint = weakpoint;
            this.grade = grade;
        }
    }
    public Dictionary<int, Data> dataTable = new Dictionary<int, Data>();

    public WeaponTable()
    {
        path = "tables/weapon_table";
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
                    csv.GetField<int>(1), // name
                    csv.GetField<int>(2), // sellgold
                    csv.GetField<int>(3), // property
                    csv.GetField<int>(4), // type
                    csv.GetField<float>(5), // atk
                    csv.GetField<float>(6), // weakpoint
                    csv.GetField<int>(7) // grade
                )
            );
        }
    

    }
}
