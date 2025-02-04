using System.Collections.Generic;
using UnityEngine;

namespace CCGP.Server
{
    public static class TileFactory
    {
        private static Dictionary<string, Dictionary<string, object>> _tiles = null;
        private static Dictionary<string, Dictionary<string, object>> Tiles
        {
            get
            {
                if(_tiles == null)
                {
                    _tiles = LoadInitialTiles();
                }
                return _tiles;
            }
        }

        private static Dictionary<string, Dictionary<string, object>> LoadInitialTiles()
        {
            var file = Resources.Load<TextAsset>("TileList");

            if(file == null)
            {
                Shared.Logger.LogError<Tile>("TileList.json을 찾을 수 없습니다!");
                return null;
            }

            var dict = MiniJSON.Json.Deserialize(file.text) as Dictionary<string, object>;

            Resources.UnloadAsset(file);

            var tileList = (List<object>)dict["Tiles"];
            var result = new Dictionary<string, Dictionary<string, object>>();
            foreach(object entry in tileList)
            {
                var tileData = (Dictionary<string, object>)entry;
                var id = (string)tileData["ID"];
                result.Add(id, tileData);
            }

            return result;
        }

        public static Tile CreateTile(string id)
        {
            var tileData = Tiles[id];
            Tile tile = CreateTile(tileData);

            return tile;
        }

        public static Tile CreateTile(Dictionary<string, object> data)
        {
            var tile = new Tile();

            tile.Load(data);

            return tile;
        }
    }
}