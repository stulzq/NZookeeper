namespace NZookeeper.ACL
{
    public class Acl
    {
        public Acl()
        {
            
        }
        public Acl(AclPerm perm, AclScheme scheme,AclId id)
        {
            Perm = perm;
            Scheme = scheme;
            Id = id;
        }
        public AclPerm Perm { get; set; }

        public AclScheme Scheme { get; set; }

        public AclId Id { get; set; }
    }
}