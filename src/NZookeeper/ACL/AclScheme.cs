using System.ComponentModel;

namespace NZookeeper.ACL
{
    public enum AclScheme
    {
        [Description("world")]
        World = 0,

        [Description("auth")]
        Auth,

        [Description("digest")]
        Digest,

        [Description("ip")]
        Ip
    }
}