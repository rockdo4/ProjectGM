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
        public int name { get; set; }
        public Armor.ArmorType type { get; set; }
        public int sellgold { get; set; }
        public int defence { get; set; }
        public int skill1_id { get; set; }
        public int skill1_lv { get; set; }
        public int skill2_id { get; set; }
        public int skill2_lv { get; set; }
        public int set_skill_id { get; set; }
        public int req_parts { get; set; }
        public int socket { get; set; }
        public int upgrade { get; set; }

        public Data(int name, Armor.ArmorType type, int sellgold,
            int defence,
            int skill1_id, int skill1_lv,
            int skill2_id, int skill2_lv,
            int set_skill_id, int req_parts, int socket, int upgrade)
        {
            this.name = name;
            this.type = type;
            this.sellgold = sellgold;
            this.defence = defence;
            this.skill1_id = skill1_id;
            this.skill1_lv = skill1_lv;
            this.skill2_id = skill2_id;
            this.skill2_lv = skill2_lv;
            this.set_skill_id = set_skill_id;
            this.req_parts = req_parts;
            this.socket = socket;
            this.upgrade = upgrade;
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
            dataTable.Add(csv.GetField<int>(0),
                new Data
                (
                    csv.GetField<int>(1), // name
                    (Armor.ArmorType)csv.GetField<int>(2), // type
                    csv.GetField<int>(3), // sellgold
                    csv.GetField<int>(4), // defence
                    csv.GetField<int>(5), // skill1_id
                    csv.GetField<int>(6), // skill1_lv
                    csv.GetField<int>(7), // skill2_id
                    csv.GetField<int>(8), // skill2_lv
                    csv.GetField<int>(9), // set_skill_id
                    csv.GetField<int>(10), // req_parts
                    csv.GetField<int>(11), // socket
                    csv.GetField<int>(12) // upgrade
                )
            );
        }

    }
}