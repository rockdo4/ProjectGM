using CsvHelper.Configuration;
using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class ArmorTable : CsvTable
{
    public class Data
    {
        public int Armor_name { get; set; }
        public Armor.ArmorType Armor_type { get; set; }
        public int def { get; set; }
        public int skill1_id { get; set; }
        public int skill1_lv { get; set; }
        public int skill2_id { get; set; }
        public int skill2_lv { get; set; }
        public int skill3_id { get; set; }
        public int skill3_lv { get; set; }
        public int set_skill { get; set; }
        public int socket { get; set; }

        public Data(int Armor_name, Armor.ArmorType Armor_type, int def,
            int skill1_id, int skill1_lv,
            int skill2_id, int skill2_lv,
            int skill3_id, int skill3_lv,
            int set_skill, int socket)
        {
            this.Armor_name = Armor_name;
            this.Armor_type = Armor_type;
            this.def = def;
            this.skill1_id = skill1_id;
            this.skill1_lv = skill1_lv;
            this.skill2_id = skill2_id;
            this.skill2_lv = skill2_lv;
            this.skill3_id = skill3_id;
            this.skill3_lv = skill3_lv;
            this.set_skill = set_skill;
            this.socket = socket;
        }
    }
    public Dictionary<int, Data> dataTable = new Dictionary<int, Data>();

    public ArmorTable()
    {
        path = "tables/armor_table";
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
                    csv.GetField<int>(1), // Armor_name
                    (Armor.ArmorType)csv.GetField<int>(2), // Armor_type
                    csv.GetField<int>(3), // def
                    csv.GetField<int>(4), // skill1_id
                    csv.GetField<int>(5), // skill1_lv
                    csv.GetField<int>(6), // skill2_id
                    csv.GetField<int>(7), // skill2_lv
                    csv.GetField<int>(8), // skill3_id
                    csv.GetField<int>(9), // skill3_lv
                    csv.GetField<int>(10), // set_skill
                    csv.GetField<int>(11) // socket
                )
            );
        }

    }
}