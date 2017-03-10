using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonkeyHoldem.Tests
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                //diamonds (♦), clubs (♣),hearts (♥), spades (♠)
                var handTextList = new[]
                {
                    "♦A♣T♥K♠Q♦3♦6♦9",
                    "♦A♣T♥A♠Q♦3♦6♦9",
                    "♦A♣T♥A♠Q♦3♦6♦Q",
                    "♦A♦T♥A♠Q♦3♦6♦9",
                    "♦A♦3♣A♦6♥A♠A♦9",
                    "♦3♣2♥A♠Q♦4♦6♦5",
                    "♦K♣2♥A♠Q♦T♦6♦J",
                    "♦A♣A♥A♠Q♦3♦6♦K",
                    "♦8♦4♥A♦5♦7♦6♦9",
                    "♦8♦4♦A♦5♦7♦6♦9",
                    "♦A♦6♣9♦6♥A♠A♦9",
                    "♦A♥9♣9♦6♥A♠A♦9",
                };

                foreach (var text in handTextList)
                {
                    var hand = new Hand(text);
                    var ev = Evaluation.Eval(hand);
                    Console.WriteLine($"{hand}: {ev}");
                }
            }
            catch
            {
                throw;
            }
            Console.WriteLine("done");
            Console.Read();
        }
    }
}
