using System;

namespace NZookeeper.ACL
{
    [Flags]
    public enum AclPerm
    {
        /// <summary>
        /// read permission
        /// </summary>
        Read = 1 << 0,
        /// <summary>
        /// write permission
        /// </summary>
        Write = 1 << 1,
        /// <summary>
        /// create permission
        /// </summary>
        Create = 1 << 2,
        /// <summary>
        /// delete permission
        /// </summary>
        Delete = 1 << 3,
        /// <summary>
        /// admin permission
        /// </summary>
        Admin = 1 << 4,
        /// <summary>
        /// All permissions
        /// </summary>
        All = Read | Write | Create | Delete | Admin
    }
}