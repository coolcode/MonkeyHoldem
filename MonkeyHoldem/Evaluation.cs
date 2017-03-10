using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonkeyHoldem
{
    public class Evaluation
    {
        public static void BubbleSort<T>(List<T> arrayList) where T : IComparable<T>
        {
            for (int i = 0; i < arrayList.Count - 1; i++)
            {
                for (int j = 0; j < arrayList.Count - i - 1; j++)
                {
                    if (arrayList[j].CompareTo(arrayList[j + 1]) > 0)
                    {
                        T temp = arrayList[j];
                        arrayList[j] = arrayList[j + 1];
                        arrayList[j + 1] = temp;
                    }
                }
            }
        }

        public static EvalResult Eval(Hand hand)
        {
            var bestHand = new List<int>();

            var cardsType = CardsType.GaoPai;
            hand.Sort();
            //BubbleSort(hand);

            var suitsCount = new int[4];
            var numbersCount = new Dictionary<int, int>(13);

            foreach (var item in hand)
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
                cardsType = CardsType.SiTiao;

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
                cardsType = CardsType.ManTangHon;

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
                cardsType = CardsType.TongHuaShun;

                bestHand.AddRange(card5);
            }
            else if (suitsCount.Any(c => c >= 5))
            {
                cardsType = CardsType.TongHua;

                for (var suit = Suits.Diamond; suit <= Suits.Spade; suit++)
                {
                    if (suitsCount[suit] >= 5)
                    {
                        bestHand = (
                            hand.Where(c => c.Suit == suit)
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
                cardsType = CardsType.ShunZi;
                bestHand.AddRange(card5);
            }
            else if (counts[3] >= 1)
            {
                cardsType = CardsType.SanTiao;

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
                cardsType = CardsType.LiangDui;

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
                cardsType = CardsType.YiDui;

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
                cardsType = CardsType.GaoPai;
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
