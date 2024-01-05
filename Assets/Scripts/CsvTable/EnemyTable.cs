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
        public int name { get; set; }
        public int hp { get; set; }
        public int defence { get; set; }
        public int attack { get; set; }
        public int phase { get; set; }
        public int phaseAttack { get; set; }
        public int type { get; set; }
        public float deley { get; set; }

        public Data(int name, int hp, int defence, int attack, int phase, int phaseAttack, int type, float deley)
        {
            this.name = name;
            this.hp = hp;
            this.defence = defence;
            this.attack = attack;
            this.phase = phase;
            this.phaseAttack = phaseAttack;
            this.type = type;
            this.deley = deley;
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

        csv.Read();
        csv.Read();

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
                    csv.GetField<float>(7)
                )
            );
        }
    }
}