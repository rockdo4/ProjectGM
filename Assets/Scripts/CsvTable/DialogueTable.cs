using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

public class DialogueTable : CsvTable
{
    public class Data
    {
        public int sceneN { get; set; }
        public int dialogueID { get; set; }
        public int character { get; set; }
        public int dialType { get; set; }

        public Data(int sceneN, int dialogueID, int character, int dialType)
        {
            this.sceneN = sceneN;
            this.dialogueID = dialogueID;
            this.character = character;
            this.dialType = dialType;
        }
    }
    public Dictionary<int, Data> dataTable = new Dictionary<int, Data>();

    public DialogueTable()
    {
        path = "tables/dialogue_table";
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
                    csv.GetField<int>(4)
                )
            );
        }
    }
}