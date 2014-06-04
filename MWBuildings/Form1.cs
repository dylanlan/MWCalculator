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

		private void textBox_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!char.IsControl(e.KeyChar)
				&& !char.IsDigit(e.KeyChar))
			{
				e.Handled = true;
			}
		}

		private void saveToolStripButton_Click(object sender, EventArgs e)
		{
			this.saveFileDialog1.ShowDialog();
		}

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

		private void openToolStripButton_Click(object sender, EventArgs e)
		{
			this.openFileDialog1.ShowDialog();
		}

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

		private void helpToolStripButton_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Monster Warlord Building Calculator\nVersion 1.0.0\nWritten by Dylan Stankievech\nCopyright 2014", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
    }
}
