using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonkeyHoldem
{
    public class Evaluation
    {
        private readonly static Random rand = new Random();
        private const int TotalCards = 52;
        private Card[] AllCards = new Card[TotalCards];
        private Dictionary<Card, int> CardIndexMap = new Dictionary<Card, int>(TotalCards);

        public Evaluation()
        {
            Init();
        }

        public void Init()
        {
            var i = 0;
            for (var suit = Suits.Diamond; suit <= Suits.Spade; suit++)
            {
                for (var n = 2; n <= Numbers.A; n++)
                {
                    var card = new Card { Suit = suit, Number = n };
                    AllCards[i] = card;
                    CardIndexMap[card] = i;
                    i++;
                }
            }
        }

        public IEnumerable<SolvedResult> Solve(int players=6)
        {
            for(var i = 0; i < AllCards.Length-1; i++)
            {
                for(var j = i+1; j < AllCards.Length; j++)
                {
                    var hand = new Hand(new[] { AllCards[i], AllCards[j] });
                    var sr = Solve(hand, new Hand(), players);
                    sr.Id = hand.Uid;

                    Console.WriteLine($"solving {hand}...");

                    yield return sr;
                }
            }

            yield break;
        }

        public SolvedResult Solve(Hand ownHand, Hand deskHand, int players)
        {
            var hand = new Hand();
            hand.AddRange(ownHand);
            hand.AddRange(deskHand);

            var ownSimHand = new Hand(hand);

            var erList = new List<EvalResult>();
            var simIndex = 0;
            var simCount = 10000;
            var winCount = 0;
            while (simIndex++ < simCount)
            {
                if (hand.Count < 7)
                {
                    ownSimHand = RandomHand(hand);
                }
                var ownEr = Eval(ownSimHand);
                erList.Add(ownEr);

                var filters = new Hand(ownSimHand);

                //win or not
                var lose = false;
                for (var i = 0; i < players - 1; i++)
                {
                    var oppSimHand = RandomHand(deskHand, filters);
                    var oppEr = Eval(oppSimHand);

                    if (oppEr.Value > ownEr.Value)
                    {
                        lose = true;
                        break;
                    }

                    filters.AddRange(oppSimHand);
                }

                if (!lose)
                {
                    winCount++;
                }
            }

            var sr = new SolvedResult();
            sr.WinRate = winCount / (double)simCount;
            sr.CardsTypeProbabilities = erList.GroupBy(c => c.Type)
                .Select(g => new { Prob = g.Count() / (double)simCount, Type = g.Key })
                .ToDictionary(c => c.Type, c => c.Prob);

            return sr;
        }

        public Hand[] RandomHand(Hand deskHand, Hand filterCards, int players)
        {
            var fc = new List<Card>(filterCards);
            var simHands = new List<Hand>();

            for (var i = 0; i < players; i++)
            {
                //sim a hand
                var simHand = new Hand(deskHand);
                while (simHand.Count < 7)
                {
                    var index = rand.Next(0, TotalCards);
                    var randCard = AllCards[index];
                    if (fc.Contains(randCard))
                    {
                        continue;
                    }

                    simHand.Add(randCard);

                    fc.Add(randCard);
                }

                simHands.Add(simHand);
            }

            return simHands.ToArray();
        }

        public Hand RandomHand(Hand ownHand, Hand filterCards = null)
        {
            var fc = new List<Card>(filterCards ?? ownHand);
            var indexRange = Enumerable.Range(0, TotalCards)
                .Except(fc.Select(c => CardIndexMap[c]))
                .ToList();

            //sim a hand
            var simHand = new Hand(ownHand);
            while (simHand.Count < 7)
            {
                //var index = rand.Next(0, TotalCards);
                //var randCard = AllCards[index];
                //if (fc.Contains(randCard))
                //{
                //    continue;
                //}
                //simHand.Add(randCard);

                //fc.Add(randCard);

                var index = rand.Next(0, indexRange.Count);
                var randCard = AllCards[indexRange[index]];

                simHand.Add(randCard);

                indexRange.RemoveAt(index);
            }

            return simHand;
        }

        public EvalResult Eval(Hand hand)
        {
            var bestHand = new List<int>();

            var cardsType = CardsType.Invalid;

            var evalHand = new Hand(hand);
            evalHand.Sort();

            var suitsCount = new int[4];
            var numbersCount = new Dictionary<int, int>(13);

            foreach (var item in evalHand)
            {
                suitsCount[item.Suit]++;
                var numCount = 0;
                if (!numbersCount.TryGetValue(item.Number, out numCount))
                {
                    numbersCount[item.Number] = 1;
                }
                else
                {
                    numbersCount[item.Number]++;
                }
            }

            var numbers = numbersCount.Keys.ToList();
            numbers.Sort();
            var card5 = new int[5];

            var straight = false;
            if (numbers.Count >= 5)
            {
                for (var i = numbers.Count - 5; i >= 0; i--)
                {
                    if (numbers[i] + 1 == numbers[i + 1]
                        && numbers[i + 1] + 1 == numbers[i + 2]
                        && numbers[i + 2] + 1 == numbers[i + 3]
                        && numbers[i + 3] + 1 == numbers[i + 4])
                    {
                        straight = true;
                        card5 = new[] { numbers[i + 4], numbers[i + 3], numbers[i + 2], numbers[i + 1], numbers[i] };

                        break;
                    }
                }

                if (!straight)
                {
                    if (numbers[0] == 2
                       && numbers[1] == 3
                       && numbers[2] == 4
                       && numbers[3] == 5
                       && numbers[numbers.Count - 1] == Numbers.A)
                    {
                        straight = true;
                        card5 = new[] { 5, 4, 3, 2, 1 };
                    }
                }
            }

            //number counts
            var counts = new int[5];
            foreach (var numCount in numbersCount.Values)
            {
                counts[numCount]++;
            }

            if (counts[4] == 1)
            {
                cardsType = CardsType.FourOfaKind;

                foreach (var item in numbersCount)
                {
                    if (item.Value == 4)
                    {
                        bestHand.AddRange(new[] { item.Key, item.Key, item.Key, item.Key });
                        numbers.Remove(item.Key);
                        bestHand.Add(numbers.Last());
                        break;
                    }
                }

            }
            else if (counts[3] == 1 && counts[2] > 0)
            {
                cardsType = CardsType.FullHouse;

                var threeOfaKind = numbersCount.Where(c => c.Value == 3)
                    .Select(c => c.Key)
                    .Max();

                var bestPair = numbersCount.Where(c => c.Value == 2)
                    .Select(c => c.Key)
                    .Max();

                bestHand.AddRange(new[] { threeOfaKind, threeOfaKind, threeOfaKind, bestPair, bestPair });
            }
            else if (straight && suitsCount.Any(c => c >= 5))
            {
                cardsType = CardsType.StraightFlush;

                bestHand.AddRange(card5);
            }
            else if (suitsCount.Any(c => c >= 5))
            {
                cardsType = CardsType.Flush;

                for (var suit = Suits.Diamond; suit <= Suits.Spade; suit++)
                {
                    if (suitsCount[suit] >= 5)
                    {
                        bestHand = (
                            evalHand.Where(c => c.Suit == suit)
                                .Select(c => c.Number)
                                .OrderByDescending(c => c)
                                .Take(5)
                                .ToList()
                                );
                        break;
                    }
                }
            }
            else if (straight)
            {
                cardsType = CardsType.Straight;
                bestHand.AddRange(card5);
            }
            else if (counts[3] >= 1)
            {
                cardsType = CardsType.ThreeOfaKind;

                var threeOfaKind = numbersCount.Where(c => c.Value == 3)
                    .Select(c => c.Key)
                    .Max();

                var single = numbersCount.Where(c => c.Value == 1)
                    .Select(c => c.Key)
                    .OrderByDescending(c => c)
                    .Take(2)
                    .ToArray();


                if (single.Length == 1)
                {
                    bestHand.AddRange(new[] { threeOfaKind, threeOfaKind, threeOfaKind, single[0], 0 });
                }
                else if (single.Length == 2)
                {
                    bestHand.AddRange(new[] { threeOfaKind, threeOfaKind, threeOfaKind, single[0], single[1] });
                }
            }
            else if (counts[2] >= 2)
            {
                //two pairs
                cardsType = CardsType.TwoPairs;

                var pairs = numbersCount.Where(c => c.Value == 2)
                    .Select(c => c.Key)
                    .OrderByDescending(c => c)
                    .Take(2)
                    .ToArray();

                var single = numbersCount.Where(c => c.Value == 1)
                  .Select(c => c.Key)
                  .Max();

                bestHand.AddRange(new[] { pairs[0], pairs[0], pairs[1], pairs[1], single });
            }
            else if (counts[2] == 1)
            {
                cardsType = CardsType.OnePair;

                var pair = numbersCount.Where(c => c.Value == 2)
                    .Select(c => c.Key)
                    .First();

                var single = numbersCount.Where(c => c.Value == 1)
                    .Select(c => c.Key)
                    .OrderByDescending(c => c)
                    .Take(3)
                    .ToArray();

                bestHand.AddRange(new[] { pair, pair, single[0], single[1], single[2] });
            }
            else
            {
                cardsType = CardsType.HighCard;
                bestHand = numbers.OrderByDescending(c => c)
                    .Take(5)
                    .ToList();
            }

            var evalValue = (ulong)cardsType;
            foreach (var num in bestHand)
            {
                evalValue = evalValue * 100 + (ulong)num;
            }

            return new EvalResult { Type = cardsType, Value = evalValue };
        }

    }
}
