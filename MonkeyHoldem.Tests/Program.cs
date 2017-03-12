using LiteDB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkeyHoldem.Tests
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            try
            {
                //EvalTest();
                SolveTest();
            }
            catch
            {
                throw;
            }
            Console.WriteLine("done");
            Console.Read();
        }

        private static void SolveTest()
        {
            var evaluation = new Evaluation();

            var sw = new Stopwatch();
            sw.Start();
            Console.WriteLine("solving...");
            var ers = evaluation.Solve();
            using (var db = new LiteDatabase(@"holdem.db"))
            {
                var col = db.GetCollection<SolvedResult>("hand2solve");
                col.EnsureIndex(x => x.Id, true);
                foreach (var er in ers)
                {
                    Console.WriteLine($"{er.Id}: {er}");
                    Console.WriteLine("saving...");
                    col.Insert(er);
                    Console.WriteLine($"time span {sw.Elapsed}");
                }
            }
        }

        private static void EvalTest()
        {
            var evaluation = new Evaluation();
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
                var ev = evaluation.Eval(hand);
                Console.WriteLine($"{hand}: {ev}");
            }
        }
    }
}
