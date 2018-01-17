using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Memory
{
    public partial class boardForm : Form
    {
        public boardForm()
        {
            InitializeComponent();
        }

        #region Instance Variables
        const int NOT_PICKED_YET = -1;

        int firstCardNumber = NOT_PICKED_YET;
        int secondCardNumber = NOT_PICKED_YET;
        int matches = 0;
        #endregion

        #region Methods

        // This method finds a picture box on the board based on it's number (1 - 20)
        // It takes an integer as it's parameter and returns the picture box controls
        // that's name contains that number
        private PictureBox GetCard(int i)
        {
            PictureBox card = (PictureBox)this.Controls["card" + i];
            return card;
        }

        // This method gets the filename for the image displayed in a picture box given it's number
        // It takes an integer as it's parameter and returns a string containing the 
        // filename for the image in the corresponding picture box
        private string GetCardFilename(int i)
        {
            return GetCard(i).Tag.ToString();
        }

        // This method changes the filename for a given picture box
        // It takes an integer and a string that represents a filename as it's parameters
        // It doesn't return a value but stores the filename for the image to be displayed
        // in the picture box.  It doesn't actually display the new image
        private void SetCardFilename(int i, string filename)
        {
            GetCard(i).Tag = filename;
        }

        // These 2 methods get the value (and suit) of the card in a given picturebox
        // Both methods take an integer as the parameter and return a string
        private string GetCardValue(int index)
        {
            return GetCardFilename(index).Substring(4, 1);
        }

        private string GetCardSuit(int index)
        {
            return GetCardFilename(index).Substring(5, 1);
        }

        // This method returns true if the values of the two indexed cards is the same.
        // Returns false otherwise.
        private bool IsMatch(int index1, int index2)
        {
            if (GetCardValue(index1) == GetCardValue(index2))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // This method fills each picture box with a filename
        private void FillCardFilenames()
        {
            string[] values = { "a", "2", "j", "q", "k" };
            string[] suits = { "c", "d", "h", "s" };
            int i = 1;

            for (int suit = 0; suit <= 3; suit++)
            {
                for (int value = 0; value <= 4; value++)
                {
                    SetCardFilename(i, "card" + values[value] + suits[suit] + ".jpg");
                    i++;
                }
            }
        }

        // This method shuffles the deck by swapping
        // each card from first to last with another.
        private void ShuffleCards()
        {
            Random rndGen = new Random();

            // Swapping each card with another using temp variables.
            for (int i = 1; i <= 20; i++)
            {
                int rndNum = rndGen.Next(1, 21);
                string temp = GetCardFilename(i);
                string tempRandom = GetCardFilename(rndNum);

                SetCardFilename(i, tempRandom);
                SetCardFilename(rndNum, temp);
            }
        }

        // This method loads (shows) an image in a picture box.  Assumes that filenames
        // have been filled in an earlier call to FillCardFilenames
        private void LoadCard(int i)
        {
            PictureBox card = GetCard(i);
            card.Image = Image.FromFile(System.Environment.CurrentDirectory + "\\Cards\\" + GetCardFilename(i));
        }

        // This method loads the image for the back of a card in a picture box
        private void LoadCardBack(int i)
        {
            PictureBox card = GetCard(i);
            card.Image = Image.FromFile(System.Environment.CurrentDirectory + "\\Cards\\black_back.jpg");
        }
 
        // This method shows (loads) the backs of all of the cards
        private void LoadAllCardBacks()
        {
            for (int i = 1; i <= 20; i++)
            {
                LoadCardBack(i);
            }
        }

        // This method hides a picture box
        private void HideCard(int i)
        {
            GetCard(i).Hide();
        }

        // This method hides all picture boxes
        private void HideAllCards()
        {
            for(int i = 1; i <= 20; i++)
            {
                HideCard(i);
            }
        }

        // This method shows a picture box
        private void ShowCard(int i)
        {
            GetCard(i).Show();
        }

        // This method shows all picture boxes
        private void ShowAllCards()
        {
            for (int i = 1; i <= 20; i++)
            {
                ShowCard(i);
            }
        }

        // This method disables a picture box
        private void DisableCard(int i)
        {
            GetCard(i).Enabled = false;
        }

        // This method disables all picture boxes
        private void DisableAllCards()
        {
            for (int i = 1; i <= 20; i++)
            {
                DisableCard(i);
            }
        }

        // This method enables a picture box
        private void EnableCard(int i)
        {
            GetCard(i).Enabled = true;
        }

        // This method enables all picture boxes
        private void EnableAllCards()
        {
            for (int i = 1; i <= 20; i++)
            {
                EnableCard(i);
            }
        }
    
        // This method enables all visible picture boxes.
        private void EnableAllVisibleCards()
        {
            for (int i = 1; i <= 20; i++)
            {
                if(GetCard(i).Visible)
                {
                    EnableCard(i);
                }
            }
        }

        #endregion

        #region EventHandlers
        //Loads the board at the beginning of the game by creating a deck, shuffling it and loading it.
        private void boardForm_Load(object sender, EventArgs e)
        {
            FillCardFilenames();
            ShuffleCards();
            LoadAllCardBacks();
        }

        //When the player clicks on a card, it will flip and compare to another flipped card
        //(if any)
        private void card_Click(object sender, EventArgs e)
        {
            PictureBox card = (PictureBox)sender;
            int cardNumber = int.Parse(card.Name.Substring(4));

            //If this is the first flipped card
            if (firstCardNumber == NOT_PICKED_YET)
            {
                //Flip it
                firstCardNumber = cardNumber;
                LoadCard(cardNumber);
                DisableCard(cardNumber);
            }
            //If this is the second flipped card
            else
            {
                //Flip it and hold it there for a few seconds.
                secondCardNumber = cardNumber;
                LoadCard(cardNumber);
                DisableAllCards();
                flipTimer.Start();
            }
        }

        //Compares the flipped cards and checks if you've won.
        private void flipTimer_Tick(object sender, EventArgs e)
        {
            flipTimer.Stop();
            //If the cards match
            if(IsMatch(firstCardNumber, secondCardNumber))
            {
                //Remove them and increment matches.
                matches++;
                HideCard(firstCardNumber);
                HideCard(secondCardNumber);
                firstCardNumber = NOT_PICKED_YET;
                secondCardNumber = NOT_PICKED_YET;
                //If you've won
                if(matches == 10)
                {
                    MessageBox.Show("You win!");
                }
                else
                {
                    EnableAllVisibleCards();
                }
            }
            //If they don't match
            else
            {
                //Reset the board.
                LoadCardBack(firstCardNumber);
                LoadCardBack(secondCardNumber);
                firstCardNumber = NOT_PICKED_YET;
                secondCardNumber = NOT_PICKED_YET;
                EnableAllVisibleCards();
            }
        }
        #endregion
    }
}
