using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MWBuildings
{
    /// <summary>
    /// A form which allows the user to input their number of each building type in the
    /// mobile app game, Monster Warlord, and their amount of gold they want to spend, and
    /// it calculates and outputs the optimal buildings to buy, such that income is maximized.
    /// Allows saving/loading building numbers from previous calculations.
    /// </summary>
    public partial class Form1 : Form
    {
		private List<NumericUpDown> numberBoxes;
		private List<TextBox> textBoxes;
		private List<int> initialCosts;
		private List<int> incomes;

        public Form1()
        {
            this.InitializeComponent();
			this.addNumberBoxes();
			this.addTextBoxes();
			this.initializeCosts();
			this.initializeIncomes();
			//loadNA1();
        }

        /// <summary>
        /// Adds all the individual number boxes for each input number of building to a list
        /// </summary>
		private void addNumberBoxes()
		{
			this.numberBoxes = new List<NumericUpDown>{
				this.farmNumberBox,
				this.penNumberBox,
				this.storageNumberBox,
				this.hutNumberBox,
				this.waterWheelNumberBox,
				this.lumbermillNumberBox,
				this.forgeNumberBox,
				this.bakeryNumberBox,
				this.marketNumberBox,
				this.restaurantNumberBox,
				this.windmillNumberBox,
				this.castleNumberBox,
				this.colosseumNumberBox,
				this.cathedralNumberBox,
				this.palaceNumberBox
			};
		}

        /// <summary>
        /// Adds all the individual text boxes for each output number of buildings to a list
        /// </summary>
		private void addTextBoxes()
		{
			this.textBoxes = new List<TextBox>{
				this.farmTextBox,
				this.penTextBox,
				this.storageTextBox,
				this.hutTextBox,
				this.waterWheelTextBox,
				this.lumbermillTextBox,
				this.forgeTextBox,
				this.bakeryTextBox,
				this.marketTextBox,
				this.restaurantTextBox,
				this.windmillTextBox,
				this.castleTextBox,
				this.colosseumTextBox,
				this.cathedralTextBox,
				this.palaceTextBox
			};
		}

        /// <summary>
        /// Initializes a list with the different incomes of each building type
        /// </summary>
		private void initializeIncomes()
		{
			this.incomes = new List<int>{
				1,
				5,
				10,
				50,
				100,
				160,
				500,
				1000,
				1600,
				2500,
				3000,
				3500,
				3800,
				4000,
				4500
			};
		}

        /// <summary>
        /// Initializes a list of the different initial costs of each building type
        /// </summary>
		private void initializeCosts()
		{
			this.initialCosts = new List<int>{
				40,
				200,
				400,
				2000,
				5000,
				11000,
				40000,
				100000,
				200000,
				400000,
				750000,
				1000000,
				1500000,
				2500000,
				3000000
			};
		}

        /// <summary>
        /// Can be used to automatically load the default file upon startup, currently commented out
        /// </summary>
		private void loadNA1()
        {
			string filePath = this.openFileDialog1.InitialDirectory + "\\" + this.openFileDialog1.FileName;
			System.IO.StreamReader reader = null;
			try
			{
				reader = new System.IO.StreamReader(@filePath);
				for (int i = 0; i < this.numberBoxes.Count; i++)
				{
					var numberBox = this.numberBoxes[i];
					var textBox = this.textBoxes[i];
					Decimal value = Decimal.Parse(reader.ReadLine());
					numberBox.Value = value;
					textBox.Text = value.ToString();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			finally
			{
				if (reader != null) reader.Dispose();
			}
        }

        /// <summary>
        /// Performs calculations to find the optimal buildings to buy with the given
        /// values in the numeric up down boxes and the gold amount to maximize the
        /// income
        /// </summary>
        /// <param name="sender">
        /// Object which calls this method
        /// </param>
        /// <param name="e">
        /// Arguments associated with this event
        /// </param>
        private void buttonCalculate_Click(object sender, EventArgs e)
        {
			this.buttonCalculate.Visible = false;
			for (int i = 0; i < this.numberBoxes.Count; i++)
			{
				var numberBox = this.numberBoxes[i];
				var textBox = this.textBoxes[i];
				textBox.Text = numberBox.Value.ToString();
			}

			var gold = this.goldNumberBox.Value;
			var efficientBuilding = findMostEfficient();
			decimal cost = findCost(efficientBuilding);
			while (gold > 0 && gold >= cost)
			{
				gold -= cost;
				int count = int.Parse(efficientBuilding.Text);
				count++;
				efficientBuilding.Text = count.ToString();
				efficientBuilding = findMostEfficient();
				cost = findCost(efficientBuilding);
			}
			this.buttonCalculate.Visible = true;
        }

        /// <summary>
        /// Finds the cost of buying the number of buildings of the given textbox
        /// </summary>
        /// <param name="building">
        /// The textbox associated with the building cost to calculate
        /// </param>
        /// <returns>
        /// The amount of gold required to buy the given number of this building
        /// </returns>
		private int findCost(TextBox building)
		{
			int initialCost = this.initialCosts[this.textBoxes.IndexOf(building)];
			double costIncrease = initialCost * 0.1;
			int cost = initialCost;
			int count = int.Parse(building.Text);
			while (count > 0)
			{
				cost += (int)costIncrease;
				count--;
			}
			return cost;
		}

        /// <summary>
        /// Finds the optimal next building to purchase; the building which has the
        /// highest income divided by cost
        /// </summary>
        /// <returns>
        /// The textbox associated with the best next building to buy
        /// </returns>
		private TextBox findMostEfficient()
		{
			TextBox bestBuilding = this.textBoxes[0];

			int income = this.incomes[this.textBoxes.IndexOf(bestBuilding)];
			int costToBuild = findCost(bestBuilding);
			double bestEfficiency = costToBuild / income;
			foreach (var nextBuilding in this.textBoxes)
			{
				int nextCost = findCost(nextBuilding);
				int nextIncome = this.incomes[this.textBoxes.IndexOf(nextBuilding)];
				double nextEfficiency = (double)nextCost / (double)nextIncome;
				if (nextEfficiency < bestEfficiency)
				{
					bestBuilding = nextBuilding;
					bestEfficiency = nextEfficiency;
				}
			}
			return bestBuilding;
		}

        /// <summary>
        /// Called when a key is pressed in a textbox, ensuring only control and digit characters are permitted
        /// </summary>
        /// <param name="sender">
        /// The object calling this method
        /// </param>
        /// <param name="e">
        /// The arguments associated with this event
        /// </param>
		private void textBox_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!char.IsControl(e.KeyChar)
				&& !char.IsDigit(e.KeyChar))
			{
				e.Handled = true;
			}
		}

        /// <summary>
        /// Called when the user clicks the save button
        /// </summary>
        /// <param name="sender">
        /// The object calling this method
        /// </param>
        /// <param name="e">
        /// Arguments associated with this event
        /// </param>
		private void saveToolStripButton_Click(object sender, EventArgs e)
		{
			this.saveFileDialog1.ShowDialog();
		}

        /// <summary>
        /// Called when the Ok button is clicked in the save dialog, saves the current
        /// number of each building to a file
        /// </summary>
        /// <param name="sender">
        /// The object calling this method
        /// </param>
        /// <param name="e">
        /// The arguments associated with this event
        /// </param>
		private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
		{
			string filePath = this.saveFileDialog1.FileName;
			System.IO.StreamWriter writer = null;
			try
			{
				writer = new System.IO.StreamWriter(@filePath);
				foreach (var textBox in this.textBoxes)
				{
					writer.WriteLine(textBox.Text);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			finally
			{
				if (writer != null) writer.Dispose();
			}

		}

        /// <summary>
        /// Called when the user clicks the open button
        /// </summary>
        /// <param name="sender">
        /// The object calling this method
        /// </param>
        /// <param name="e">
        /// Arguments associated with this event
        /// </param>
		private void openToolStripButton_Click(object sender, EventArgs e)
		{
			this.openFileDialog1.ShowDialog();
		}

        /// <summary>
        /// Called when the user clicks Ok in the open dialog, loads all the
        /// building numbers saved in the selected file
        /// </summary>
        /// <param name="sender">
        /// The object calling this method
        /// </param>
        /// <param name="e">
        /// The arguments associated with this event
        /// </param>
		private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
		{
			string filePath = this.openFileDialog1.FileName;
			System.IO.StreamReader reader = null;
			try
			{
				reader = new System.IO.StreamReader(@filePath);
				for (int i = 0; i < this.numberBoxes.Count; i++)
				{
					var numberBox = this.numberBoxes[i];
					var textBox = this.textBoxes[i];
					Decimal value = Decimal.Parse(reader.ReadLine());
					numberBox.Value = value;
					textBox.Text = value.ToString();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			finally
			{
				if (reader != null) reader.Dispose();
			}
		}

        /// <summary>
        /// When the user clicks the New button, it clears all the numbers in the controls
        /// </summary>
        /// <param name="sender">
        /// The object calling this method
        /// </param>
        /// <param name="e">
        /// The arguments associated with this event
        /// </param>
		private void newToolStripButton_Click(object sender, EventArgs e)
		{
			this.penLabel.Focus();
			foreach (var textBox in this.textBoxes)
			{
				textBox.Text = "0";
			}
			foreach (var numberBox in this.numberBoxes)
			{
				numberBox.Value = 0;
				numberBox.Refresh();
			}
			this.goldNumberBox.Value = 0;
		}

        /// <summary>
        /// Called when the user clicks the Help button, displays information about this program
        /// </summary>
        /// <param name="sender">
        /// The object calling this method
        /// </param>
        /// <param name="e">
        /// Arguments associated with this event
        /// </param>
		private void helpToolStripButton_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Monster Warlord Building Calculator\nVersion 1.0.0\nWritten by Dylan Stankievech\nCopyright 2014", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
    }
}
