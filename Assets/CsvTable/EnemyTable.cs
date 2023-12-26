using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

public class EnemyTable : CsvTable
{
    public class Data
    {
        public int Mon_ID { get; set; }
        public string Mon_Name { get; set; }
        public int Mon_HP { get; set; }
        public int Mon_Attack { get; set; }
        public float Mon_AtSpeed { get; set; }
        public int Mon_PhaseAttack { get; set; }
        public float Mon_PhaseAtSpeed { get; set; }
        public int Mon_Range { get; set; }
        public int Mon_Defence { get; set; }
        public int Mon_Phase { get; set; }
        public int Mon_Affinity { get; set; }

        public Data(int id, string name, int hp, int attack, float atSpeed, int phaseAttack, float phaseAtSpeed, int range, int defence, int phase, int affinity)
        {
            this.Mon_ID = id;
            this.Mon_Name = name;
            this.Mon_HP = hp;
            this.Mon_Attack = attack;
            this.Mon_AtSpeed = atSpeed;
            this.Mon_PhaseAttack = phaseAttack;
            this.Mon_PhaseAtSpeed = phaseAtSpeed;
            this.Mon_Range = range;
            this.Mon_Defence = defence;
            this.Mon_Phase = phase;
            this.Mon_Affinity = affinity;
        }
    }
    public Dictionary<int, Data> dataTable = new Dictionary<int, Data>();

    public EnemyTable()
    {
        path = "tables/enemy_table";
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
                    csv.GetField<string>(2),
                    csv.GetField<int>(3),
                    csv.GetField<int>(4),
                    csv.GetField<float>(5),
                    csv.GetField<int>(6),
                    csv.GetField<float>(7),
                    csv.GetField<int>(8), 
                    csv.GetField<int>(9),
                    csv.GetField<int>(10),
                    csv.GetField<int>(11)
                )
            );
        }
    }
}