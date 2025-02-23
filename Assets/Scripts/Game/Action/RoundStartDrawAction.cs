using System.Collections.Generic;
using CCGP.Shared;

namespace CCGP.Server
{
    public class RoundStartDrawAction : GameAction
    {
        public List<uint> Amounts;
        public List<Card> Cards1;
        public List<Card> Cards2;
        public List<Card> Cards3;
        public List<Card> Cards4;

        public RoundStartDrawAction()
        {
            Cards1 = new();
            Cards2 = new();
            Cards3 = new();
            Cards4 = new();
        }

        public List<Card> this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0:
                        return Cards1;
                    case 1:
                        return Cards2;
                    case 2:
                        return Cards3;
                    case 3:
                        return Cards4;
                    default:
                        LogUtility.LogError<RoundStartDrawAction>($"잘못된 값입니다. : {i}");
                        return null;
                }
            }
            set
            {
                switch (i)
                {
                    case 0:
                        Cards1 = value;
                        break;
                    case 1:
                        Cards2 = value;
                        break;
                    case 2:
                        Cards3 = value;
                        break;
                    case 3:
                        Cards4 = value;
                        break;
                    default:
                        LogUtility.LogError<RoundStartDrawAction>($"잘못된 값입니다. : {i}");
                        break;
                }
            }
        }
    }
}