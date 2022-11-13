using Net.Core.TextReader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapRecord : TextRecord
{
    protected override void Initialize()
    {
    }
}

public class MapTable : TextTable<MapRecord>
{
    public MapTable() : base(LoadType.FromResourcePath, "map_01", columns: 12, 0, containsHeader: false)
    {
        this.Load(true);
    }
}

public class MapReader : MonoBehaviour
{
    private MapTable mapTable;
    
    /// <summary>
    /// row | col
    /// </summary>
    public int[,] MapData = new int[12, 12];

    public void Init()
    {
        if (mapTable == null)
        {
            mapTable = new MapTable();
            mapTable.Load();
        }

        for (int r = 0; r < mapTable.Rows.Length; r++)
        {
            for (int c = 0; c < mapTable.ColumnsCount; c++)
            {
                //parse map
                MapData[r, c] = ConverMapData(mapTable.Rows[r].Columns[c]);
            }
        }
        
        int ConverMapData(string input)
        {
            var e = EnTileType.Base;
            var isSuccess = Enum.TryParse(input, out EnTileType myStatus);
            if (isSuccess)
            {
                e = myStatus;
            }

            return (int)e;
        }
    }
}




