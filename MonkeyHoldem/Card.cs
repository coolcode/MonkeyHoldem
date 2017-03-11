using System;
using System.Collections.Generic;
using System.Linq;
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
    }

    public class Hand : List<Card>
    {
        private string text = string.Empty;

        public Hand()
        {

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

            for (var i = 0; i < text.Length-1; i=i+2)
            {
                //TODO: if text.length is odds
                var card = new Card {
                    Suit = suitmap[text[i]],
                    Number = nummap[text[i + 1]]
                    };

                Add(card);
            }
        }

        public override string ToString()
        {
            return text;
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
        TongHuaShun = 9,
        SiTiao = 8,
        ManTangHon = 7,
        TongHua = 6,
        ShunZi = 5,
        SanTiao = 4,
        LiangDui = 3,
        YiDui = 2,
        GaoPai = 1,
        Invalid = 0
    }

    public class EvalResult
    {
        public CardsType Type { get; set; }
        public ulong Value { get; set; }

        public override string ToString()
        {
            return $"(t:{Type}, v:{Value})";
        }
    }
}
