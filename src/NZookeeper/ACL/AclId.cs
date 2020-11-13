using System;
using System.Security.Cryptography;
using System.Text;

namespace NZookeeper.ACL
{
    public class AclId
    {
        public AclId(string val)
        {
            Value = val;
        }
        public string Value { get; set; }

        public static AclId World()
        {
            return new AclId("anyone");
        }

        public static AclId Ip(string ip)
        {
            return new AclId(ip);
        }

        public static AclId Digest(string username,string password)
        {
            var sha1 = SHA1.Create();
            var sha1Bytes = sha1.ComputeHash(Encoding.UTF8.GetBytes($"{username}:{password}"));
            var base64Psd = Convert.ToBase64String(sha1Bytes);
            return new AclId($"{username}:{base64Psd}");
        }

        public static AclId Super(string username, string password)
        {
            return Digest(username,password);
        }
    }
}