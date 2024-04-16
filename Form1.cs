using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using CsvHelper;

namespace AC3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ReadCSV();
        }

        private List<Comarca> Comarques { get; set; }
        private int pageSize = 10;
        private int currentPage = 1;

        private void ReadCSV()
        {
            this.Comarques = new List<Comarca>();
            using (StreamReader reader = new StreamReader("consumAigua.csv"))
            {
                using (CsvReader csvreader = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csvreader.Read();
                    csvreader.ReadHeader();
                    while (csvreader.Read())
                    {
                        this.Comarques.Add(
                            new Comarca
                            {
                                Year = csvreader.GetField<int>("Any"),
                                Code = csvreader.GetField<int>("Codi comarca"),
                                Name = csvreader.GetField<string>("Comarca"),
                                Population = csvreader.GetField<int>("Població"),
                                DomesticExpense = csvreader.GetField<int>("Domèstic xarxa"),
                                EconomicalActivitiesExpense = csvreader.GetField<int>("Activitats econòmiques i fonts pròpies"),
                                Total = csvreader.GetField<int>("Total"),
                                IndividualExpense = csvreader.GetField<decimal>("Consum domèstic per càpita")
                            }
                        );
                    }
                }
            }
            PaginateData();
            CreateXML();
            ComboBoxYearMinValue();
            ComboBoxComarcaMinValue();
        }

        private void PaginateData()
        {
            int startIndex = (currentPage - 1) * pageSize;
            int endIndex = Math.Min(startIndex + pageSize - 1, this.Comarques.Count - 1);
            List<Comarca> currentPageData = this.Comarques.GetRange(startIndex, endIndex - startIndex + 1);
            dataGridView1.DataSource = currentPageData;
            rightArrow.Enabled = endIndex < this.Comarques.Count - 1;
            leftArrow.Enabled = currentPage > 1;
        }

        private void CreateXML()
        {
            Dictionary<int, string> comarquesDict = new Dictionary<int, string>();
            foreach (Comarca comarca in this.Comarques)
            {
                comarquesDict.TryAdd(comarca.Code, comarca.Name);
            }
            XmlDocument xml = new XmlDocument();
            if (File.Exists("consumAigua.xml"))
                File.Delete("consumAigua.xml");

            XDocument xmlDoc = new XDocument(
                new XElement(
                    "Consum",
                    from comarca in comarquesDict
                    select new XElement(
                        "Comarca",
                        new XElement("Codi", comarca.Key),
                        new XElement("Nom", comarca.Value)
                    )
                )
            );
            string xmlFilePath = "consumAigua.xml";
            xmlDoc.Save(xmlFilePath);
        }

        private void ComboBoxYearMinValue()
        {
            for (int i = this.Comarques.Select(x => x.Year).Distinct().Min(); i <= 2050; i++)
            {
                comboBoxYear.Items.Add(i);
            }
        }

        private void ComboBoxComarcaMinValue()
        {
            string xmlFilePath = "consumAigua.xml";
            XDocument xmlDoc = XDocument.Load(xmlFilePath);
            var comarques =
                from comarca in xmlDoc.Descendants("Comarca")
                select new
                {
                    Code = (int)comarca.Element("Codi"),
                    Name = (string)comarca.Element("Nom")
                };
            foreach (var comarca in comarques)
            {
                comboBoxComarca.Items.Add(comarca.Name);
            }
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex;
            if (row != -1)
            {
                DataGridViewRow selectedRow = dataGridView1.Rows[row];
                Comarca comarca = new Comarca
                {
                    Year = Convert.ToInt32(selectedRow.Cells["Any"].Value),
                    Code = Convert.ToInt32(selectedRow.Cells["Codi Comarca"].Value),
                    Name = selectedRow.Cells["Comarca"].Value.ToString(),
                    Population = Convert.ToInt32(selectedRow.Cells["Població"].Value),
                    DomesticExpense = Convert.ToInt32(selectedRow.Cells["Domèstic xarxa"].Value),
                    EconomicalActivitiesExpense = Convert.ToInt32(selectedRow.Cells["Activitats econòmiques i fonts pròpies"].Value),
                    Total = Convert.ToInt32(selectedRow.Cells["Total"].Value),
                    IndividualExpense = Convert.ToDecimal(selectedRow.Cells["Consum domèstic per càpita"].Value)
                };
                IsBigCity(comarca);
                PrintDomesticAverageExpense(comarca);
                Consumes(comarca);
            }
        }

        private void IsBigCity(Comarca comarca)
        {
            labelGT.Text = comarca.Population > 200000 ? "Sí" : "No";
        }

        private void PrintDomesticAverageExpense(Comarca comarca)
        {
            var avgByComarca = this
                .Comarques.Where(c => c.Code == comarca.Code)
                .GroupBy(c => c.Code)
                .Select(g => new
                {
                    Code = g.Key,
                    g.First().Name,
                    AvgDomesticExpense = g.Average(c => c.DomesticExpense).ToString("0")
                });
            labelAvg.Text = avgByComarca.Max(c => c.AvgDomesticExpense);
        }

        private void Consumes(Comarca comarca)
        {
            int maxYear = this.Comarques.Max(c => c.Year);
            var filteredComarques = this.Comarques.Where(c => c.Year == maxYear);
            var mostExpensive = filteredComarques.Max(c => c.IndividualExpense);
            var lessExpensive = filteredComarques.Min(c => c.IndividualExpense);
            labelHighest.Text = comarca.IndividualExpense == mostExpensive ? "Sí" : "No";
            labelLowest.Text = comarca.IndividualExpense == lessExpensive ? "Sí" : "No";
            //En cas de que el consum sigui el més alt i el més baix alhora, es mostrarà "No" a les dues. Ex: és l'únic registre d'aquest any.
        }

        private void GuardarCSV(ComboBox comboBoxYear, ComboBox comboBoxComarca, TextBox textBoxPopulation, TextBox textBoxDomNet, TextBox textBoxEconomical, TextBox textBoxTotal, TextBox textBoxDomCap)
        {
            var newComarca = new Comarca
            {
                Year = Convert.ToInt32(comboBoxYear.SelectedItem),
                Code = this.Comarques.Find(c => c.Name == comboBoxComarca.SelectedItem.ToString()).Code,
                Name = comboBoxComarca.SelectedItem.ToString(),
                Population = Convert.ToInt32(textBoxPopulation.Text),
                DomesticExpense = Convert.ToInt32(textBoxDomNet.Text),
                EconomicalActivitiesExpense = Convert.ToInt32(textBoxEconomical.Text),
                Total = Convert.ToInt32(textBoxTotal.Text),
                IndividualExpense = decimal.Parse(textBoxDomCap.Text)
            };
            using var stream = File.Open("consumAigua.csv", FileMode.Append);
            using var writer = new StreamWriter(stream);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            writer.WriteLine();
            csv.WriteRecord(newComarca);
            MessageBox.Show("Dades guardades correctament", "Guardat", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Comarques.Clear();
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            Regex integerPattern = new Regex(@"^[0-9]{1,7}$");
            Regex decimalPattern = new Regex(@"^[0-9]+(?:[\,]\d+)?$");

            if (comboBoxYear.SelectedItem == null)
                MessageBox.Show("Selecciona un any", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (comboBoxComarca.SelectedItem == null)
                MessageBox.Show("Selecciona una comarca", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (!integerPattern.IsMatch(textBoxPopulation.Text))
                MessageBox.Show("La població ha de ser un número positiu i enter", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (!integerPattern.IsMatch(textBoxDomNet.Text))
                MessageBox.Show("El consum domèstic ha de ser un número positiu i enter", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (!integerPattern.IsMatch(textBoxEconomical.Text))
                MessageBox.Show("El consum d'activitats econòmiques ha de ser un número positiu i enter", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (!integerPattern.IsMatch(textBoxTotal.Text))
                MessageBox.Show("El total ha de ser un número positiu i enter", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (!decimalPattern.IsMatch(textBoxDomCap.Text))
                MessageBox.Show("El consum per càpita ha de ser un número positiu (decimals amb coma)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                GuardarCSV(comboBoxYear, comboBoxComarca, textBoxPopulation, textBoxDomNet, textBoxEconomical, textBoxTotal, textBoxDomCap);
                ReadCSV();
                CleanInputs();
            }
        }

        private void CleanInputs()
        {
            comboBoxYear.SelectedItem = null;
            comboBoxComarca.SelectedItem = null;
            textBoxPopulation.Text = "";
            textBoxDomNet.Text = "";
            textBoxEconomical.Text = "";
            textBoxTotal.Text = "";
            textBoxDomCap.Text = "";
        }

        private void Clean_Click(object sender, EventArgs e)
        {
            CleanInputs();
        }

        private void leftArrow_Click(object sender, EventArgs e)
        {
            currentPage--;
            ReadCSV();
        }

        private void rightArrow_Click(object sender, EventArgs e)
        {
            currentPage++;
            ReadCSV();
        }
    }
}
