using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonkeyHoldem.App
{
    public partial class MainForm : Form
    {
        private ListBox[] suitsBox, numbersBox;

        public MainForm()
        {
            InitializeComponent();

            suitsBox = new ListBox[]
            {
                lbSuit1,lbSuit2,lbSuit3,lbSuit4,lbSuit5,lbSuit6,lbSuit7
            };

            numbersBox = new ListBox[]
            {
                lbNum1,lbNum2,lbNum3,lbNum4,lbNum5,lbNum6,lbNum7
            };

            Load += MainForm_Load;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            var suits = new[] { "♦♣♥♠" };
            var numbers = new[] { "23456789TJQKA" };

            Text = "Monkey Hold'em";

            foreach (var lbSuit in suitsBox)
            {
                var suitsSource = new List<ValueText>
                {
                    new ValueText(0,"♦"),
                    new ValueText(1,"♣"),
                    new ValueText(2,"♥"),
                    new ValueText(3,"♠"),
                };

                lbSuit.DataSource = suitsSource;
                lbSuit.ValueMember = "Value";
                lbSuit.DisplayMember = "Text";
            }

            foreach (var lbNum in numbersBox)
            {
                var numbersSource = new List<ValueText>
                {
                    new ValueText(2,"2"),
                    new ValueText(3,"3"),
                    new ValueText(4,"4"),
                    new ValueText(5,"5"),
                    new ValueText(6,"6"),
                    new ValueText(7,"7"),
                    new ValueText(8,"8"),
                    new ValueText(9,"9"),
                    new ValueText(10,"T"),
                    new ValueText(11,"J"),
                    new ValueText(12,"Q"),
                    new ValueText(13,"K"),
                    new ValueText(14,"A"),
                };

                lbNum.DataSource = numbersSource;
                lbNum.ValueMember = "Value";
                lbNum.DisplayMember = "Text";
            }

        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                var deskCardsCount = (int)numDeskCards.Value;
                var playersCount = (int)numPlayers.Value;

                var ownHand = new Hand();
                var deskHand = new Hand();
                for (var i = 0; i < deskCardsCount; i++)
                {
                    var lbSuit = suitsBox[i];
                    var lbNumber = numbersBox[i];
                    var card = new Card
                    {
                        Suit = (int)lbSuit.SelectedValue,
                        Number = (int)lbNumber.SelectedValue,
                    };

                    if (i < 2)
                    {
                        ownHand.Add(card);
                    }
                    else
                    {
                        deskHand.Add(card);
                    }
                }

                var evaluation = new Evaluation();
                var sw = Stopwatch.StartNew();
                var sr = evaluation.Solve(ownHand, deskHand, playersCount);
                sw.Stop();
                //var evalResult = evaluation.Eval(hand);
                //DisplayMessage($"{evalResult}");
                DisplayMessage($"{sr}");
                DisplayMessage($"Time span: {sw.Elapsed}");

            }
            catch(Exception ex)
            {
                DisplayMessage($"{ex}");
            }
            DisplayMessage($"-----------------------");
            
        }

        private void DisplayMessage(string text)
        {
            rtbMessage.Text += $"{DateTime.Now}: {text}\r\n";
            rtbMessage.SelectionStart = rtbMessage.Text.Length;
            rtbMessage.ScrollToCaret(); 
        }
    }

    class ValueText
    {
        public int Value { get; set; }
        public string Text { get; set; }

        public ValueText(int value, string text)
        {
            Value = value;
            Text = text;
        }
    }
}
