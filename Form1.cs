using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RacingAssignment
{
    public partial class Form1 : Form
    {
        Greyhound[] Cyclez = new Greyhound[4];
        Punter[] punters = new Punter[3];
        Greyhound winnerCycle;

        Timer timer1, timer2, timer3, timer4;

        public Form1()
        {
            InitializeComponent();
            InitilizeRaceInformation();
            
        }
        private void InitilizeRaceInformation()
        {
            // Cycles Info
            Cyclez[0] = new Greyhound() { CycleName = "Cycle 1", RaceTrackLength = 930, MyPictureBox = pictureBox1 };
            Cyclez[1] = new Greyhound() { CycleName = "Cycle 2", RaceTrackLength = 930, MyPictureBox = pictureBox2 };
            Cyclez[2] = new Greyhound() { CycleName = "Cycle 3", RaceTrackLength = 930, MyPictureBox = pictureBox3 };
            Cyclez[3] = new Greyhound() { CycleName = "Cycle 4", RaceTrackLength = 930, MyPictureBox = pictureBox4 };

            //Punter Info
            punters[0] = Factory.GetAPunter("Joe");
            punters[1] = Factory.GetAPunter("Bob");
            punters[2] = Factory.GetAPunter("Al");

            punters[0].MyLabel = max_bet;
            punters[0].MyRadioButton = L1;
            punters[0].MyText = textBox1;
            punters[0].MyRadioButton.Text = punters[0].Name;


            punters[1].MyLabel = max_bet;
            punters[1].MyRadioButton = L2;
            punters[1].MyText = textBox2;
            punters[1].MyRadioButton.Text = punters[1].Name;


            punters[2].MyLabel = max_bet;
            punters[2].MyRadioButton = L3;
            punters[2].MyText = textBox3;
            punters[2].MyRadioButton.Text = punters[2].Name;

            numericUpDown2.Minimum = 1;
            numericUpDown2.Maximum = 4;
            numericUpDown2.Value = 1;
        }


        private void MoveCycles(object sender, EventArgs e)
        {
            if (sender is Timer)
            {
                int index = -1;
                Timer timer = sender as Timer;
                if (timer == timer1)
                {
                    index = 0;
                }
                else if (timer == timer2)
                {
                    index = 1;
                }
                else if (timer == timer3)
                {
                    index = 2;
                }
                else if (timer == timer4)
                {
                    index = 3;
                }

                if (index != -1)
                {
                    PictureBox pbox = Cyclez[index].MyPictureBox;
                    if (pbox.Location.X + pbox.Width > Cyclez[index].RaceTrackLength)
                    {
                        if (winnerCycle == null)
                        {
                            winnerCycle = Cyclez[index];
                        }
                        timer1.Stop();
                        timer2.Stop();
                        timer3.Stop();
                        timer4.Stop();
                    }
                    else
                    {
                        int jump = new Random().Next(1, 15);
                        pbox.Location = new Point(pbox.Location.X + jump, pbox.Location.Y);
                    }
                }
            }
            if (winnerCycle != null)
            {
                MessageBox.Show("Congratulation! " + winnerCycle.CycleName + " Win The Race");
                BetSetInfo();
                foreach (Punter punter in punters)
                {
                    if (punter.MyBet != null)
                    {
                        if (punter.MyBet.Cycle == winnerCycle)
                        {
                            punter.Cash += punter.MyBet.Amount;
                            punter.MyText.Text = punter.Name + " Won and now has $" + punter.Cash;
                            punter.Winner = true;
                        }
                        else
                        {
                            punter.Cash -= punter.MyBet.Amount;
                            if (punter.Cash == 0)
                            {
                                punter.MyText.Text = "BUSTED";
                                punter.Busted = true;
                                punter.MyRadioButton.Enabled = false;
                            }
                            else
                            {
                                punter.MyText.Text = punter.Name + " Lost and now has $" + punter.Cash;
                            }
                        }
                    }
                }
                winnerCycle = null;
                timer1 = timer2 = timer3 = timer4 = null;
                int count = 0;
                foreach (Punter punter in punters)
                {
                    if (punter.Busted)
                    {
                        count++;
                    }
                    if (punter.MyRadioButton.Enabled && punter.MyRadioButton.Checked)
                    {
                        label2.Text = "Max Bet is $" + punter.Cash;
                        numericUpDown1.Maximum = punter.Cash;
                        numericUpDown1.Minimum = 1;
                    }
                    punter.MyBet = null;
                    punter.Winner = false;
                }
                if (count == punters.Length)
                {
                    button1.Text = "Game Over";

                }
                foreach (Greyhound Cycle in Cyclez)
                {
                    Cycle.MyPictureBox.Location = new Point(12, Cycle.MyPictureBox.Location.Y);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void puntersRadio_CheckedChanged(object sender, EventArgs e)
        {
            BetSetInfo(); //method calling- place bet
        }
        private void BetSetInfo()
        {
            foreach (Punter punter in punters)
            {
                if (punter.Busted)
                {
                    punter.MyText.Text = "BUSTED";
                }
                else
                {
                    if (punter.MyBet == null)
                    {
                        punter.MyText.Text = punter.Name + " hasn't placed a bet";
                    }
                    else
                    {
                        punter.MyText.Text = punter.Name + " bets $" + punter.MyBet.Amount + " on " + punter.MyBet.Cycle.CycleName;
                    }
                    if (punter.MyRadioButton.Checked)
                    {
                        label2.Text = "Max Bet Amount is $" + punter.Cash.ToString();
                        button1.Text = "Place Bet for " + punter.Name;
                        punter.MyLabel.Text = punter.Name + " Bets Amount $";
                        numericUpDown1.Minimum = 1;
                        numericUpDown1.Maximum = punter.Cash;
                        numericUpDown1.Value = 1;
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text.Contains("Place"))
            {
                int count = 0;
                int total_active = 0;
                foreach (Punter punter in punters)
                {
                    if (punter.Busted)
                    {
                        //MessageBox.Show("Bet is Not Placed Because " + punter.Name + " is BUSTED");
                    }
                    else
                    {
                        total_active++;
                        if (punter.MyRadioButton.Checked)
                        {
                            if (punter.MyBet == null)
                            {
                                int number = (int)numericUpDown2.Value;
                                int amount = (int)numericUpDown1.Value;
                                bool alreadyPlaced = false;
                                foreach (Punter pun in punters)
                                {
                                    if (pun.MyBet != null && pun.MyBet.Cycle == Cyclez[number - 1])
                                    {
                                        alreadyPlaced = true;
                                        break;
                                    }
                                }
                                if (alreadyPlaced)
                                {
                                    MessageBox.Show("This Cycle Number is Already Taken By Another Better.");
                                }
                                else
                                {
                                    punter.MyBet = new Bet() { Amount = amount, Cycle = Cyclez[number - 1] };
                                }

                            }
                            else
                            {
                                MessageBox.Show("You Already Place Bet for " + punter.Name);
                            }
                        }
                        if (punter.MyBet != null)
                        {
                            count++;
                        }
                    }
                }
                BetSetInfo();
                if (count == total_active)
                {
                    button1.Text = "Start The Race Now";
                }
            }
            else if (button1.Text.Contains("Start"))
            {
                timer1 = new Timer();
                timer1.Interval = 15;
                timer1.Tick += MoveCycles;

                timer2 = new Timer();
                timer2.Interval = 15;
                timer2.Tick += MoveCycles;

                timer3 = new Timer();
                timer3.Interval = 15;
                timer3.Tick += MoveCycles;

                timer4 = new Timer();
                timer4.Interval = 15;
                timer4.Tick += MoveCycles;

                timer1.Start();
                timer2.Start();
                timer3.Start();
                timer4.Start();

            }
            else if (button1.Text.Contains("GAME"))
            {
                MessageBox.Show("GAME OVER!!");
                Application.Exit();
            }
        }
    }
}
