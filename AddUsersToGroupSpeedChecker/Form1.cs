using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using AddUsersToGroupSpeedChecker.Code;

namespace AddUsersToGroupSpeedChecker
{
    public partial class Form1 : Form
    {
        private string _domain, _domainController, _adminUser, _adminPass, _userSamAccount, _groupDN, _groupName;
        private Stopwatch _timer;
        private bool _showAmLbl;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnUserByAcctMgmt_Click(object sender, EventArgs e)
        {
            ReadConfigValues();

            // Get group name
            _timer = Stopwatch.StartNew();
            lblStatus.Text = string.Format("Adding user '{0}' to group '{1}' by AM.", _userSamAccount, GetGroupName(_groupDN));

            btnUserByAcctMgmt.Enabled = btnAddUserByAdServices.Enabled = lblTimeByAcctMgmt.Enabled = lblTimeByAdServices.Enabled = false;
            _showAmLbl = true;
            bgWorker.RunWorkerAsync();
        }

        private void btnAddUserByAdServices_Click(object sender, EventArgs e)
        {
            ReadConfigValues();
            _timer = Stopwatch.StartNew();
            lblStatus.Text = string.Format("Adding user '{0}' to group '{1}' by ADS.", _userSamAccount, GetGroupName(_groupDN));

            btnUserByAcctMgmt.Enabled = btnAddUserByAdServices.Enabled = lblTimeByAcctMgmt.Enabled = lblTimeByAdServices.Enabled = false;
            _showAmLbl = false;
            bgWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Read values from App.config file.
        /// </summary>
        private void ReadConfigValues()
        {
            _domain = ConfigurationManager.AppSettings["Domain"];
            _domainController = ConfigurationManager.AppSettings["DomainController"];
            _adminUser = ConfigurationManager.AppSettings["AdminUser"];
            _adminPass = ConfigurationManager.AppSettings["AdminPass"];
            _userSamAccount = ConfigurationManager.AppSettings["SamAccountOfUserToAdd"];
            _groupDN = ConfigurationManager.AppSettings["GroupDN"];
        }

        /// <summary>
        /// Get group name as per group's distinguishedName.
        /// </summary>
        /// <param name="groupDN">The distinguished name of group.</param>
        /// <returns>Return group name or empty value.</returns>
        private string GetGroupName(string groupDN)
        {
            var match = Regex.Match(groupDN.Trim(), "^CN=(?<COMMON_NAME>.+?),(?<CONTAINER>(?:OU|CN|DC)=.+)",
                           RegexOptions.Singleline | RegexOptions.IgnoreCase);

            if (match.Success || match.Groups["COMMON_NAME"].Success)
                _groupName = match.Groups["COMMON_NAME"].Value;

            return _groupName;
        }

        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (_showAmLbl)
            {
                var user = ADUtilities.FindUser(IdentityType.SamAccountName, _userSamAccount, _domainController, _adminUser, _adminPass);

                ADUtilities.AddUserToGroup(user, _groupDN, _domainController, _adminUser, _adminPass, true);
            }
            else
            {
                ADUtilities.AddUserToGroup(_domain, _domainController, _adminUser, _adminPass, _groupDN, _userSamAccount, true);
            }
        }

        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _timer.Stop();
            btnUserByAcctMgmt.Enabled = btnAddUserByAdServices.Enabled = lblTimeByAcctMgmt.Enabled = lblTimeByAdServices.Enabled = true;

            if (e.Error == null)
            {
                if (_showAmLbl)
                {
                    lblTimeByAcctMgmt.Visible = _showAmLbl;
                    lblTimeByAcctMgmt.Text = _timer.Elapsed.Seconds > 60 ? string.Format("Time passed: {0} minutes", _timer.Elapsed.Minutes) : string.Format("Time passed: {0} seconds", _timer.Elapsed.Seconds);
                    lblStatus.Text = string.Format("User '{0}' was added to group '{1}' by AM successfully!", _userSamAccount, _groupName);
                }
                else
                {
                    lblTimeByAdServices.Visible = !_showAmLbl;
                    lblTimeByAdServices.Text = _timer.Elapsed.Seconds > 60 ? string.Format("Time passed: {0} minutes", _timer.Elapsed.Minutes) : string.Format("Time passed: {0} seconds", _timer.Elapsed.Seconds);
                    lblStatus.Text = string.Format("User '{0}' was added to group '{1}' by ADS successfully!", _userSamAccount, _groupName);
                }

            }
            else
            {
                MessageBox.Show("Error occured-\n\n" + e.Error);
            }
        }
    }
}
