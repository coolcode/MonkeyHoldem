using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonkeyHoldem.App
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var suits = new[] { "♦♣♥♠" };
            var numbers = new[] { "23456789TJQKA" };

            var suitsSource = new List<ValueText>
            {
                new ValueText(0,"♦"),
                new ValueText(1,"♣"),
                new ValueText(2,"♥"),
                new ValueText(3,"♠"),
            };

            listBox1.DataSource = suitsSource;
            listBox1.ValueMember = "Value";
            listBox1.DisplayMember = "Text";


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

            listBox2.DataSource = numbersSource;
            listBox2.ValueMember = "Value";
            listBox2.DisplayMember = "Text";


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
