using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkeyHoldem
{
    public class Card : IComparable<Card>
    {
        /// <summary>
        ///  diamonds (♦), clubs (♣),hearts (♥), spades (♠)
        /// </summary>
        public int Suit { get; set; }

        /// <summary>
        /// 2-14
        /// </summary>
        public int Number { get; set; }


        public int CompareTo(Card other)
        {
            return Number.CompareTo(other.Number);
        }

        public override int GetHashCode()
        {
            return Suit * 100 + Number;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            return ((Card)obj).GetHashCode() == GetHashCode();
        }

        public override string ToString()
        {
            var suitmap = new Dictionary<int, string>
            {
                {0,"♦"},
                {1,"♣" },
                {2,"♥"},
                {3,"♠"}
            };

            var nummap = new Dictionary<int, string>
            {
                {2,"2"},
                {3,"3"},
                {4,"4"},
                {5,"5"},
                {6,"6"},
                {7,"7"},
                {8,"8"},
                {9,"9"},
                {10,"T"},
                {11,"J"},
                {12,"Q"},
                {13,"K"},
                {14,"A"},
            };

            return $"{suitmap[Suit]}{nummap[Number]}";
        }
    }

    public class Hand : List<Card>
    {
        private string text = string.Empty;

        public ulong Uid
        {
            get
            {
                return (ulong)(this[0].GetHashCode() * 1000 + this[1].GetHashCode());
            }
        }

        public Hand()
        {

        }

        public Hand(IEnumerable<Card> cards)
        {
            AddRange(cards);
        }

        public Hand(string text)
        {
            this.text = text;

            var suitmap = new Dictionary<char, int>
            {
                {'♦' ,Suits.Diamond},
                {'♣' ,Suits.Club},
                {'♥' ,Suits.Heart},
                {'♠' ,Suits.Spade}
            };

            var nummap = new Dictionary<char, int>
            {
                {'1' , Numbers.A},
                {'2' , 2},
                {'3' , 3},
                {'4' , 4},
                {'5' , 5},
                {'6' , 6},
                {'7' , 7},
                {'8' , 8},
                {'9' , 9},
                {'T' , 10},
                {'J' , Numbers.J},
                {'Q' , Numbers.Q},
                {'K' , Numbers.K},
                {'A' , Numbers.A},
            };

            for (var i = 0; i < text.Length - 1; i = i + 2)
            {
                //TODO: if text.length is odds
                var card = new Card
                {
                    Suit = suitmap[text[i]],
                    Number = nummap[text[i + 1]]
                };

                Add(card);
            }
        }

        public override string ToString()
        {
            return string.Join("", this);
        }
    }


    /// <summary>
    /// diamonds (♦), clubs (♣),hearts (♥), spades (♠)
    /// </summary>
    public class Suits
    {
        /// <summary>
        /// diamonds (♦)
        /// </summary>
        public const int Diamond = 0;

        /// <summary>
        /// clubs (♣)
        /// </summary>
        public const int Club = 1;

        /// <summary>
        /// hearts (♥)
        /// </summary>
        public const int Heart = 2;

        /// <summary>
        /// spades (♠)
        /// </summary>
        public const int Spade = 3;
    }

    public class Numbers
    {
        public const int J = 11;
        public const int Q = 12;
        public const int K = 13;
        public const int A = 14;
    }

    /// <summary>
    /// 
    /// </summary>
    public enum CardsType
    {
        StraightFlush = 9,
        FourOfaKind = 8,
        FullHouse = 7,
        Flush = 6,
        Straight = 5,
        ThreeOfaKind = 4,
        TwoPairs = 3,
        OnePair = 2,
        HighCard = 1,
        Invalid = 0
    }

    public class EvalResult
    {
        public CardsType Type { get; set; }
        public ulong Value { get; set; }

        public override string ToString()
        {
            return $"(t:{Type,13}, v:{Value})";
        }
    }

    public class SolvedResult
    {
        public ulong Id { get; set; }

        public double WinRate { get; set; }

        public Dictionary<CardsType, double> CardsTypeProbabilities = new Dictionary<CardsType, double>(10);

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"win%: {WinRate.ToString("p2")}");
            foreach (var item in CardsTypeProbabilities.OrderByDescending(c=> c.Value))
            {
                sb.AppendLine($"{item.Key}'s Prob%: {item.Value.ToString("p2")}");
            }

            return sb.ToString();
        }
    }
}
