using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using License.Manager.Db;
using License.Manager.Wrapper;
using License.Manager.Wrapper.Model;

namespace License.Manager.GUI
{
    public partial class Form1 : Form
    {
        private PortableLicense _portableLicense;
        private string _connString;
        public Form1()
        {
            InitializeComponent();
            _connString = ConfigurationManager.ConnectionStrings["LicDbConn"].ConnectionString;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _portableLicense = new PortableLicense();
            _portableLicense.GenerateLicense(new ProdCustomer {Email = "tbh.tilok@live.com", Name = "Tanjeer"},
                Guid.NewGuid().ToString().Substring(0, 8), "licensetest.lic", 2);
            var file = File.ReadAllBytes("licensetest.lic");
            var saved = new DbUtility(_connString).SaveLicense(new LicenseModel
            {
                LicFile = file,
                LicId = _portableLicense.LicKey,
                PublicKey = _portableLicense.PublicKey
            });
            if(saved)
                MessageBox.Show(@"Created and saved!");
            else
                MessageBox.Show(@"Error!");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _portableLicense = new PortableLicense();
            var message = string.Empty;
            var key = File.ReadAllText("key.txt");
            var validate = _portableLicense.ValidateLicense("licensedup.lic", key, out message);
            if (!validate)
                MessageBox.Show(message);
            else
                MessageBox.Show(@"License is valid!");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var license = new DbUtility(_connString).GetLicense(textBox1.Text);
            File.WriteAllBytes("licensedup.lic", license.LicFile);
            File.WriteAllText("key.txt", license.PublicKey);
        }
    }
}
