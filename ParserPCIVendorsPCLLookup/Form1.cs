using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParserPCIVendorsPCLLookup
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public class JsonObject
        {
            public string desc { get; set; }
            public string id { get; set; }
            public string venDesc { get; set; }
            public string venID { get; set; }

        }
        private async void button1_Click(object sender, EventArgs e)
        {
            string qurySelectCell;
            IHtmlCollection<IElement> htmlElement;

            IConfiguration config = Configuration.Default.WithDefaultLoader();
            string address = "https://devicehunt.com/all-pci-vendors";
            IBrowsingContext context = BrowsingContext.New(config);
            IDocument document = await context.OpenAsync(address);

            qurySelectCell = "tr td:nth-child(1)";
            htmlElement = document.QuerySelectorAll(qurySelectCell);

            IEnumerable<string> vendorId = htmlElement.Select(m => m.TextContent);

            qurySelectCell = "tr td:nth-child(2)";
            htmlElement = document.QuerySelectorAll(qurySelectCell);

            IEnumerable<string> vendorName = htmlElement.Select(m => m.TextContent);

            var numbersAndWords = vendorId.Zip(vendorName, (first, second) => first.ToUpper()+" "+ second);

            progressBar1.Maximum = numbersAndWords.Count();
            progressBar1.Visible = true;

            foreach (var str in numbersAndWords)
            {
                listBox1.Items.Add(str);
                progressBar1.Value++;
            }
            progressBar1.Visible = false;

        }
      
        private void button2_Click(object sender, EventArgs e)
        {
            string line = "";

             using (WebClient wc  = new WebClient())
                line = wc.DownloadString("https://www.pcilookup.com/api.php?action=search&vendor=&device=&_=1662980845774");

            List<JsonObject> json = JsonConvert.DeserializeObject<List<JsonObject>>(line);
            progressBar2.Maximum = json.Count;
            progressBar2.Visible = true;

            foreach (JsonObject val in json)
            {
                listBox2.Items.Add($"{val.venID} {val.venDesc} {val.id} {val.desc}");
                progressBar2.Value++;
            }
            progressBar2.Visible = false;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter("C:\\1.txt"))
            {
                for (int i = 0; i < listBox1.Items.Count; i++)
                    sw.WriteLine(listBox1.Items[i].ToString());
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter("C:\\2.txt"))
            {
                for (int i = 0; i < listBox2.Items.Count; i++)
                    sw.WriteLine(listBox2.Items[i].ToString());
            }
        }
    }
}
