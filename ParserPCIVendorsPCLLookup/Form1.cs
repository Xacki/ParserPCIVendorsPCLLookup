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
        public class PciLookUp
        {
            public string id { get; set; }
            public string desc { get; set; }
            public string venID { get; set; }
            public string venDesc { get; set; }

        }

        /*public class PciVendor
        {
            public string venID { get; set; }
            public string venDesc { get; set; }

        }*/

        public async void  SelectFromDeviceHunt(ListBox lb,ProgressBar pb)
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

            var numbersAndWords = vendorId.Zip(vendorName, (first, second) => $"{first.ToUpper().Trim()} {second}");

            pb.Maximum = numbersAndWords.Count();
            pb.Visible = true;

            foreach (var str in numbersAndWords)
            {
                lb.Items.Add(str);
                pb.Value++;
            }
            pb.Visible = false;
        }

        void ProcessParse(ListBox lb, ProgressBar pb)
        {
            string line = "";
            using (WebClient wc  = new WebClient()) line = wc.DownloadString("https://www.pcilookup.com/api.php?action=search&vendor=&device=&_=1662980845774");

            List<PciLookUp> json = JsonConvert.DeserializeObject<List<PciLookUp>>(line);

            List<string> vendors = new List<string>();

            foreach (PciLookUp obj in json)
            {
                vendors.Add(obj.venID+" "+obj.venDesc);
            }

            vendors = vendors.Distinct().ToList();

            label1.Text = vendors.Count.ToString();
            progressBar2.Visible = true;
            progressBar2.Maximum = json.Count();
            foreach (var item in vendors)
            {
                lb.Items.Add($"{item}");
                foreach (var obj in json)
                {
                    if ((obj.venID + " " + obj.venDesc) == item)
                    {
                        lb.Items.Add($"{obj.id.ToUpper()} {obj.desc}");
                        
                    }
                }
                progressBar2.Value++;
                lb.Items.Add("");
            }
            progressBar2.Visible = false;

        }

        public async Task SelectFromPciLookup(ListBox lb, ProgressBar pb)
        {
            await Task.Run(() => ProcessParse(lb,pb));
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await Task.Run(() => SelectFromDeviceHunt(listBox1,progressBar1));

        }
      
        private async void button2_Click(object sender, EventArgs e)
        {
            await Task.Run(() => SelectFromPciLookup(listBox2, progressBar2));

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
