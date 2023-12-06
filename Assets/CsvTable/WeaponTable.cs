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
        public string weapon_id { get; set; }
        public string weapon_name { get; set; }
        public string gold { get; set; }
        public string type { get; set; }
        public string hand { get; set; }
        public string atk { get; set; }
        public string weakpoint { get; set; }
    }

    public class Data_Weapon
    {
        public int weapon_name { get; set; }
        public int gold { get; set; }
        public AttackType property { get; set; }
        public WeaponType type { get; set; }
        public float atk { get; set; }
        public float weakpoint { get; set; }

        public Data_Weapon(int weapon_name, int gold, int property, int type, float atk, float weakpoint)
        {
            this.weapon_name = weapon_name;
            this.gold = gold;
            this.property = (AttackType)property;
            this.type = (WeaponType)type;
            this.atk = atk;
            this.weakpoint = weakpoint;
        }
    }
    public Dictionary<int, Data_Weapon> dataTable = new Dictionary<int, Data_Weapon>();

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
                new Data_Weapon
                (
                    int.Parse(csv.GetField(1)), // weapon_name
                    int.Parse(csv.GetField(2)), // gold
                    int.Parse(csv.GetField(3)), // property
                    int.Parse(csv.GetField(4)), // type
                    float.Parse(csv.GetField(5)), // atk
                    float.Parse(csv.GetField(6)) // weakpoint
                )
            );
        }
    

    }
}
