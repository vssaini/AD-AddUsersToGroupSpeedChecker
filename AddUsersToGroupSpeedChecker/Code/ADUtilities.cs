using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Text.RegularExpressions;

namespace AddUsersToGroupSpeedChecker.Code
{
    class ADUtilities
    {
        //Group Constants (uint)
        //ADS_GROUP_TYPE_BUILTIN_LOCAL_GROUP    = 0x00000001;
        //ADS_GROUP_TYPE_GLOBAL_GROUP           = 0x00000002;
        //ADS_GROUP_TYPE_DOMAIN_LOCAL_GROUP     = 0x00000004;
        //ADS_GROUP_TYPE_SECURITY_ENABLED       = 0x80000000;
        //ADS_GROUP_TYPE_UNIVERSAL_GROUP        = 0x00000008;

        [Flags]
        public enum GroupType : uint
        {
            LocalDistribution = 0x00000004,
            LocalSecurity = 0x00000004 | 0x80000000,
            GlobalDistribution = 0x00000002,
            GlobalSecurity = 0x00000002 | 0x80000000,
            UniversalDistribution = 0x00000008,
            UniversalSecurity = 0x00000008 | 0x80000000
        }

        /// <summary>
        /// Finds a user by its identity value
        /// </summary>
        /// <param name="identityType">The identity type</param>
        /// <param name="identityValue">The identity value</param>
        /// <param name="domainController">The domain controller</param>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <returns>The found user</returns>
        public static UserPrincipalEx FindUser(IdentityType identityType, string identityValue, string domainController, string username, string password)
        {
            return FindUser(identityType, identityValue, null, domainController, username, password);
        }

        /// <summary>
        /// Finds a user by its identity value
        /// </summary>
        /// <param name="identityType">The identity type</param>
        /// <param name="identityValue">The identity value</param>
        /// <param name="container">The container (OU) in which to search</param>
        /// <param name="domainController">The domain controller</param>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <returns>The found user</returns>
        public static UserPrincipalEx FindUser(IdentityType identityType, string identityValue, string container, string domainController, string username, string password)
        {
            var principalContext = new PrincipalContext(ContextType.Domain, domainController, container, username, password);
            var userPrincipal = new UserPrincipalEx(principalContext) { SamAccountName = identityValue };
            var searcher = new PrincipalSearcher(userPrincipal);
            userPrincipal = searcher.FindOne() as UserPrincipalEx;
            return userPrincipal;

            // Create the principal context (Below code discarded for slow performance issue)
            //var userPrincial = UserPrincipalEx.FindByIdentity(principalContext, identityType, identityValue);
        }

        /// <summary>
        /// Adds a user to a group and creates the group if required
        /// </summary>
        /// <param name="user">The user to add to the group</param>
        /// <param name="groupDN">The group's DN</param>
        /// <param name="domainController">The domain controller</param>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <param name="createGroup">Indicates whether to create the group if it does not exist</param>
        public static void AddUserToGroup(UserPrincipal user, string groupDN, string domainController, string username, string password, bool createGroup)
        {
            // Exit if adding to Domain Users
            if (groupDN.Contains("CN=Domain Users")) return;

            // Get the group with the specified DN
            var domainContext = new PrincipalContext(ContextType.Domain, domainController, username, password);
            var group = GroupPrincipal.FindByIdentity(domainContext, IdentityType.DistinguishedName, groupDN);

            PrincipalContext ouContext = null;

            // If the group is not found
            if (group == null)
            {
                // If we should create non existing groups
                if (createGroup)
                {
                    try
                    {
                        // Get the group's common-name (cn) and container from the DN
                        var match = Regex.Match(groupDN.Trim(), "^CN=(?<COMMON_NAME>.+?),(?<CONTAINER>(?:OU|CN|DC)=.+)",
                            RegexOptions.Singleline | RegexOptions.IgnoreCase);

                        if (!match.Success || !match.Groups["COMMON_NAME"].Success || !match.Groups["CONTAINER"].Success)
                            throw new Exception(string.Format("Invalid DN '{0}' format.", groupDN));

                        var groupCommonName = match.Groups["COMMON_NAME"].Value;
                        var groupContainer = match.Groups["CONTAINER"].Value;

                        // Make sure that the group container exists before creating the group
                        try
                        {
                            var ouPath = string.Format("LDAP://{0}", groupContainer);
                            if (!DirectoryEntry.Exists(ouPath))
                                throw new Exception(string.Format("Container '{0}' doesn't exist.", groupContainer));
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(string.Format("Container '{0}' doesn't exist.", groupContainer), ex);
                        }

                        // Create new group
                        ouContext = new PrincipalContext(ContextType.Domain, domainController, groupContainer,
                            username, password);
                        group = new GroupPrincipal(ouContext, groupCommonName);
                        group.Save();

                    }
                    catch (Exception ex)
                    {
                        // ReSharper disable once PossibleIntendedRethrow
                        throw ex;
                    }
                }
                else // we should not create groups
                {
                    throw new Exception(string.Format("Group '{0}' not found.", groupDN));
                }
            }
            // Add the user to the group
            if (!group.Members.Contains(user))
            {
                group.Members.Add(user);
                group.Save();
            }

            // Dispose to preserve resources
            domainContext.Dispose();
            group.Dispose();
            if (ouContext != null) ouContext.Dispose();
        }

        /// <summary>
        /// Add user to group.
        /// </summary>
        /// <param name="domain">The domain</param>
        /// <param name="domainController">The domain controller</param>
        /// <param name="adminUser">The admin user for domain</param>
        /// <param name="adminPass">The admin pass for domain</param>
        /// <param name="groupDN">The group distinguished name</param>
        /// <param name="samUser">The sAMAccountName of user that need to be added to group</param>
        /// <param name="createGroup">Indicates whether to create the group if it does not exist</param>
        public static void AddUserToGroup(string domain, string domainController, string adminUser, string adminPass, string groupDN, string samUser, bool createGroup)
        {
            // Exit if adding to Domain Users
            if (groupDN.Contains("CN=Domain Users")) return;

            // Get the group's common-name (cn) and container from the DN
            var match = Regex.Match(groupDN.Trim(), "^CN=(?<COMMON_NAME>.+?),(?<CONTAINER>(?:OU|CN|DC)=.+)",
                RegexOptions.Singleline | RegexOptions.IgnoreCase);

            if (!match.Success || !match.Groups["COMMON_NAME"].Success || !match.Groups["CONTAINER"].Success)
                throw new Exception(string.Format("Invalid DN '{0}' format.", groupDN));

            var groupName = match.Groups["COMMON_NAME"].Value;
            var groupContainer = match.Groups["CONTAINER"].Value;

            // Get root domain entry
            var context = new DirectoryContext(DirectoryContextType.Domain, domain,adminUser,adminPass);
            var objDomain = Domain.GetDomain(context);
            var rootDe = objDomain.GetDirectoryEntry();
            
            DirectoryEntry group;
            var path = string.Format("LDAP://{0}", groupDN);
            if (DirectoryEntry.Exists(path))
            {
                using (var ds = new DirectorySearcher(rootDe))
                {
                    ds.SizeLimit = 1; // Get me  only one record
                    ds.SearchScope = SearchScope.Subtree; // Search in base and sub-tree
                    ds.Filter = string.Format("(&(objectClass=group)(name={0}))", groupName);
                    var result = ds.FindOne();
                    group = result != null ? result.GetDirectoryEntry() : null;
                }
            }
            else
            {
                // If we should create non existing groups
                if (createGroup)
                {
                    try
                    {
                        // Make sure that the group container exists before creating the group
                        DirectoryEntry destOuEntry;
                        try
                        {
                            TestDestinationOU(groupContainer, rootDe, out destOuEntry);
                            if (destOuEntry == null)
                                throw new Exception(string.Format("Container '{0}' doesn't exist.", groupContainer));
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(string.Format("Container '{0}' doesn't exist.", groupContainer), ex);
                        }

                        // CREATE GROUP IN CONTAINER
                        group = destOuEntry.Children.Add(string.Format("CN={0}", groupName), "group");

                        // Use of unchecked to prevent compiler error 'overflow in constant value'
                        //int type;
                        //unchecked
                        //{
                        //    //this is the default if not specified
                        //    type = (int)GroupType.GlobalSecurity;
                        //}

                        group.Properties["sAMAccountName"].Add(groupName);
                        //groupEntry.Properties["groupType"].Add(type);
                        group.CommitChanges();
                    }
                    catch (Exception ex)
                    {
                        // ReSharper disable once PossibleIntendedRethrow
                        throw ex;
                    }
                }
                else // we should not create groups
                {
                    throw new Exception(string.Format("Group '{0}' not found.", groupDN));
                }
            }

            // Add the user to group
            using (rootDe)
            {
                AddSamToGroup(rootDe, group, samUser);
            }
        }

        /// <summary>
        /// Checks if the destination OU exists
        /// </summary>
        /// <param name="destinationOU">The destination OU</param>
        /// <param name="rootDe">The entry to domain root</param>
        /// <param name="destOuDe">The destination OU entry object.</param>
        private static void TestDestinationOU(string destinationOU, DirectoryEntry rootDe, out DirectoryEntry destOuDe)
        {
            using (var ds = new DirectorySearcher(rootDe))
            {
                ds.SizeLimit = 1; // Get me  only one record
                ds.SearchScope = SearchScope.Subtree; // Search in base and sub-tree
                ds.Filter = string.Format("(&(objectCategory=organizationalUnit)(distinguishedName={0}))", destinationOU);
                var result = ds.FindOne();
                destOuDe = result != null ? result.GetDirectoryEntry() : null;
            }
        }

        /// <summary>
        /// Add's an object to a group
        /// </summary>
        /// <param name="rootDe">The root directory entry.</param>
        /// <param name="groupDe">The DirectoryEntry of the group</param>
        /// <param name="samUser">The sAMAccountName of the user.</param>
        private static void AddSamToGroup(DirectoryEntry rootDe, DirectoryEntry groupDe, string samUser)
        {
            // Add user to group
            using (var userSearcher = new DirectorySearcher(rootDe))
            {
                userSearcher.SizeLimit = 1; // Get me  only one record
                userSearcher.SearchScope = SearchScope.Subtree; // Search in base and sub-tree if not found
                userSearcher.Filter = string.Format("(&(objectCategory=user)(SAMAccountName={0}))", samUser);
                var result = userSearcher.FindOne();

                if (result != null)
                {
                    using (var userDe = result.GetDirectoryEntry()) // User
                    {
                        using (groupDe)
                        {
                            //var objectDN = (string)userDe.Properties["distinguishedName"].Value;

                            // If the object is not a member of the group
                            //if (!groupDe.Properties["member"].Contains(objectDN))
                            //{
                            //    // Add the object DN to the group's "member" property
                            //    groupDe.Properties["member"].Add(objectDN);
                            //    groupDe.CommitChanges();
                            //}

                            var isMember = (bool)groupDe.Invoke("IsMember", userDe.Path);
                            if (!isMember) groupDe.Invoke("Add", userDe.Path);
                        }
                    }
                }
            }
        }
    }
}
