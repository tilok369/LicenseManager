using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using License.Manager.Wrapper;
using License.Manager.Wrapper.Model;

namespace License.Manager.GUI
{
    public partial class Form1 : Form
    {
        private PortableLicense _portableLicense;
        public Form1()
        {
            InitializeComponent();
            _portableLicense = new PortableLicense("test");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _portableLicense.GenerateLicense(new ProdCustomer{Email = "tbh.tilok@live.com", Name = "Tanjeer"}, "licensetest.lic", 2);
            File.WriteAllText("publickey.txt", PortableLicense.Zip(_portableLicense.PublicKey));
            MessageBox.Show(@"Created!");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var message = string.Empty;
            var key = File.ReadAllText("publickey.txt");
            var validate = _portableLicense.ValidateLicense("licensetest.lic", PortableLicense.UnZip(key), out message);
            if (!validate)
                MessageBox.Show(message);
            else
                MessageBox.Show(@"License is valid!");
        }
    }
}
