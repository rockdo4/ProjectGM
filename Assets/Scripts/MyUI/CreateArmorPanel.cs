using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateArmorPanel : MonoBehaviour, IRenewal
{
    private Equip item = null;

    public void SetEquip(Equip item)
    {
        this.item = item;
    }

    public void Renewal()
    {
        gameObject.SetActive(true);
    }

    public void CraftEquip()
    {
        var table = CsvTableMgr.GetTable<CraftTable>().dataTable;


    }
}
