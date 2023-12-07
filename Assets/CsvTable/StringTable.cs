using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

public class StringTable : CsvTable
{
    public Dictionary<int, string> dataTable = new Dictionary<int, string>();

    public StringTable()
    {
        path = "tables/string_table";
        Load();
    }

    public override void Load()
    {
        for (int i = 1; i <= 8; i++)
        {
            string p = path + i.ToString();

            var csvStr = Resources.Load<TextAsset>(p);
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
                dataTable.Add(csv.GetField<int>(0), csv.GetField<string>(1));
            }
        }
        
    }
}