using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using License.Manager.Wrapper.Model;
using Portable.Licensing;
using Portable.Licensing.Security.Cryptography;
using Portable.Licensing.Validation;

namespace License.Manager.Wrapper
{
    public class PortableLicense
    {
        private string _passPhrase;

        public string PrivateKey { get; set; }
        public string PublicKey { get; set; }

        public string LicKey
        {
            get
            {
                var keys = Guid.NewGuid().ToString().Split('-').Select(s => s.Substring(0, 4).ToUpper());
                return string.Join("-", keys);
            }
        }

        private void CreateKey()
        {
            var keyGenerator = KeyGenerator.Create();
            var keyPair = keyGenerator.GenerateKeyPair();
            PrivateKey = keyPair.ToEncryptedPrivateKeyString(_passPhrase);
            PublicKey = keyPair.ToPublicKeyString();
        }

        public Portable.Licensing.License GenerateLicense(ProdCustomer customer, string passPhrase, string path, int expireLimit)
        {
            _passPhrase = passPhrase;
            CreateKey();
            var license = Portable.Licensing.License.New()
                                 .WithUniqueIdentifier(Guid.NewGuid())
                                 .As(LicenseType.Standard)
                                 .ExpiresAt(DateTime.Now.AddDays(expireLimit))
                                 .WithMaximumUtilization(5)
                                 .LicensedTo(customer.Name, customer.Email)
                                 .CreateAndSignWithPrivateKey(PrivateKey, _passPhrase);

            //File.WriteAllText(path, license.ToString(), Encoding.UTF8);
            return license;
        }

        public bool ValidateLicense(string path, string publicKey, out string validationMessage)
        {
            validationMessage = string.Empty;
            Portable.Licensing.License license;
            using (var streamReader = new StreamReader(path))
            {
                license = Portable.Licensing.License.Load(streamReader);
            }

            var validationFailures = license.Validate()
                                            .ExpirationDate()
                                            .And()
                                            .Signature(publicKey)
                                            .AssertValidLicense();
            var enumerable = validationFailures as IValidationFailure[] ?? validationFailures.ToArray();
            validationMessage = enumerable.Aggregate(validationMessage,
                (current, validationFailure) =>
                    current + (validationFailure.Message + ": " + validationFailure.HowToResolve + "\n"));
            return !enumerable.Any();
        }

        public static string Zip(string value)
        {
            //Transform string into byte[]  
            var byteArray = new byte[value.Length];
            var indexBA = 0;
            foreach (char item in value.ToCharArray())
            {
                byteArray[indexBA++] = (byte)item;
            }

            //Prepare for compress
            var ms = new System.IO.MemoryStream();
            var sw = new System.IO.Compression.GZipStream(ms,
                System.IO.Compression.CompressionMode.Compress);

            //Compress
            sw.Write(byteArray, 0, byteArray.Length);
            //Close, DO NOT FLUSH cause bytes will go missing...
            sw.Close();

            //Transform byte[] zip data to string
            byteArray = ms.ToArray();
            var sB = new System.Text.StringBuilder(byteArray.Length);
            foreach (byte item in byteArray)
            {
                sB.Append((char)item);
            }
            ms.Close();
            sw.Dispose();
            ms.Dispose();
            return sB.ToString();
        }

        public static string UnZip(string value)
        {
            //Transform string into byte[]
            var byteArray = new byte[value.Length];
            var indexBA = 0;
            foreach (char item in value.ToCharArray())
            {
                byteArray[indexBA++] = (byte)item;
            }

            //Prepare for decompress
            var ms = new System.IO.MemoryStream(byteArray);
            var sr = new System.IO.Compression.GZipStream(ms,
                System.IO.Compression.CompressionMode.Decompress);

            //Reset variable to collect uncompressed result
            byteArray = new byte[byteArray.Length];

            //Decompress
            var rByte = sr.Read(byteArray, 0, byteArray.Length);

            //Transform byte[] unzip data to string
            var sB = new System.Text.StringBuilder(rByte);
            //Read the number of bytes GZipStream red and do not a for each bytes in
            //resultByteArray;
            for (int i = 0; i < rByte; i++)
            {
                sB.Append((char)byteArray[i]);
            }
            sr.Close();
            ms.Close();
            sr.Dispose();
            ms.Dispose();
            return sB.ToString();
        }
    }
}
