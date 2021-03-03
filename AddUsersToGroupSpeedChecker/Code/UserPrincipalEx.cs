/********************** Module - UserPrincipal Extension *****************\
* Module Name:  UserPrincipalEx.cs
* Project:      AD Bulk Users
* Date:         11th May, 2015
* Copyright (c) Dovestones Software      
* Credit:       http://www.codeproject.com/Articles/118122/How-to-use-AD-Attributes-not-represented-in-UserPr
* 
* Provide way for modifying AD Attributes not represented in UserPrincipal.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\**************************************************************************/

using System.DirectoryServices.AccountManagement;

namespace AddUsersToGroupSpeedChecker.Code
{
    /// <summary>
    /// Extension of UserPrincipal to provide way for modifying AD Attributes not represented in UserPrincipal.
    /// </summary>
    [DirectoryRdnPrefix("CN")]
    [DirectoryObjectClass("user")]
    public class UserPrincipalEx : UserPrincipal
    {
        #region Constructors

        public UserPrincipalEx(PrincipalContext context)
            : base(context)
        {
        }

        public UserPrincipalEx(PrincipalContext context,
            string samAccountName,
            string password,
            bool enabled)
            : base(context, samAccountName, password, enabled)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get or sets nullable int value that specifies whether user must change password at next logon.
        /// </summary>
        [DirectoryProperty("pwdLastSet")]
        public int? MustChangePassword
        {
            get
            {
                if (ExtensionGet("pwdLastSet").Length != 1)
                    return null;
                return (int)ExtensionGet("pwdLastSet")[0];
            }
            set { ExtensionSet("pwdLastSet", value); }
        }

        /// <summary>
        /// Set nullable int value that specifies NORMAL_ACCOUNT.
        /// </summary>
        [DirectoryProperty("userAccountControl")]
        public int? SetNormalAccount
        {
            set
            {
                ExtensionSet("userAccountControl", value);
            }
        }

        #endregion

        #region Methods

        public new static UserPrincipalEx FindByIdentity(PrincipalContext context, string identityValue)
        {
            return (UserPrincipalEx)FindByIdentityWithType(context, typeof(UserPrincipalEx), identityValue);
        }

        public new static UserPrincipalEx FindByIdentity(PrincipalContext context, IdentityType identityType,
            string identityValue)
        {
            return
                (UserPrincipalEx)FindByIdentityWithType(context, typeof(UserPrincipalEx), identityType, identityValue);
        }

        #endregion
    }
}
