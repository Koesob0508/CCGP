using CCGP.AspectContainer;
using CCGP.Shared;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;

namespace CCGP.Server
{
    public class Tile : Container, IAspect
    {
        private const int defaultIndex = -1;
        public IContainer Container { get; set; }
        public Board Board => Container as Board;
        public int AgentIndex { get; set; } = defaultIndex;

        public string ID;
        public string Name;
        public Space Space;

        public virtual void Load(Dictionary<string, object> data)
        {
            ID = (string)data["ID"];
            Name = (string)data["Name"];
            var strSpace = (string)data["Space"];
            Enum.TryParse<Space>(strSpace.ToString(), out var result);
            Space = result;
            if (data.TryGetValue("Cost", out object costObject))
            {
                // value를 Dictionary<string, object>로 캐스팅 시도
                if (costObject is Dictionary<string, object> dictObj)
                {
                    // Dictionary<string, object>의 각 값을 문자열로 변환하여 새 Dictionary<string, string>으로 만듭니다.
                    var dict = new Dictionary<string, string>();
                    foreach (var kvp in dictObj)
                    {
                        dict[kvp.Key] = kvp.Value?.ToString();
                    }

                    // ResourceType 문자열을 enum으로 변환
                    if (Enum.TryParse<ResourceType>(dict["ResourceType"], out var costType))
                    {
                        var cost = AddAspect<Cost>();
                        LogUtility.Log<Tile>($"Add cost to {Name}");
                        cost.Type = costType;
                        // Amount를 uint로 변환
                        cost.Amount = uint.Parse(dict["Amount"]);
                    }
                    else
                    {
                        LogUtility.LogWarning<Tile>("Failed to parse ResourceType from Cost.");
                    }
                }
            }

            if (data.TryGetValue("Condition", out object conditionObject))
            {
                if (conditionObject is Dictionary<string, object> dictObj)
                {
                    var dict = new Dictionary<string, string>();
                    foreach (var kvp in dictObj)
                    {
                        dict[kvp.Key] = kvp.Value?.ToString();
                    }

                    if (Enum.TryParse<ConditionType>(dict["FactionType"], out var factionType))
                    {
                        var condition = AddAspect<Condition>();
                        LogUtility.Log<Tile>($"Add condition to {Name}");
                        condition.Type = factionType;
                        condition.Amount = uint.Parse(dict["Amount"]);
                    }
                    else
                    {
                        LogUtility.LogWarning<Tile>("Failed to parse FactionType from Condition.");
                    }
                }
            }
        }

        public Tile() { }

        public Tile(SerializedTile sTile)
        {
            Name = sTile.Name;
            Space = sTile.Space;
            AgentIndex = sTile.AgentIndex;
        }
    }
}