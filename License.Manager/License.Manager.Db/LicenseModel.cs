using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace License.Manager.Db
{
    public class LicenseModel
    {
        public string LicId { get; set; }
        public string PublicKey { get; set; }
        public byte[] LicFile { get; set; }
    }
}
