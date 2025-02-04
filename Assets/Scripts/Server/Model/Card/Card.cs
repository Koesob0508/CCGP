using CCGP.AspectContainer;
using CCGP.Shared;
using System;
using System.Collections.Generic;

namespace CCGP.Server
{
    public class Card : Container
    {
        public string GUID;
        public int OwnerIndex;

        #region JSON
        public string ID;
        public string Name;
        public int Cost;
        public int Persuasion;
        public Space Space = Space.None;
        #endregion

        public Zone Zone = Zone.Deck;

        public virtual void Load(Dictionary<string, object> data)
        {
            GUID = Guid.NewGuid().ToString();
            ID = (string)data["ID"];
            Name = (string)data["Name"];
            Cost = Convert.ToInt32(data["Cost"]);
            Persuasion = Convert.ToInt32(data["Cost"]);

            Space = Space.None;

            var spaces = (List<object>)data["Space"];

            if (spaces == null) return;

            foreach (var space in spaces)
            {
                switch (space)
                {
                    case "Yellow":
                        Space |= Space.Yellow;
                        break;
                    case "Blue":
                        Space |= Space.Blue;
                        break;
                    case "Green":
                        Space |= Space.Green;
                        break;
                    case "Emperor":
                        Space |= Space.Emperor;
                        break;
                    case "Guild":
                        Space |= Space.Guild;
                        break;
                    case "BeneGesserit":
                        Space |= Space.BeneGesserit;
                        break;
                    case "Fremen":
                        Space |= Space.Fremen;
                        break;
                    default:
                        break;
                }
            }

        }
    }
}