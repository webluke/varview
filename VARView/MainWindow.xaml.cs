using MahApps.Metro.Controls;
using Microsoft.Win32;
using System.IO;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using ControlzEx.Theming;
using System.Windows.Media;

namespace VARView;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : MetroWindow
{

    public MainWindow()
    {
        InitializeComponent();
        ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncWithAppMode;
        ThemeManager.Current.SyncTheme();
    }

    private void LoadVAR_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        OpenFileDialog dlg = new OpenFileDialog();
        dlg.Filter = "VAR Sheets (*.rtf, *.doc)|*.rtf;*.doc|All files (*.*)|*.*";
        if (dlg.ShowDialog() == true)
        {
            try
            {
                using FileStream fileStream = new FileStream(dlg.FileName, FileMode.Open);
                TextRange range = new TextRange(VARRTF.Document.ContentStart, VARRTF.Document.ContentEnd);
                range.Load(fileStream, DataFormats.Rtf);
            }
            catch (IOException)
            {
                MessageBox.Show("File is in use by another program. Please close the file and try again."); return;
            }
        }

        string debitGroup = "";
        string autoClose = "";
        foreach (var table in VARRTF.Document.Blocks.OfType<Table>())
        {
            table.Foreground = Brushes.Black;

            if (table.Columns.Count == 2)
            {
                foreach (var r in table.RowGroups)
                {
                    foreach (var row in r.Rows)
                    {
                        string s = new TextRange(row.Cells[0].ContentStart, row.Cells[0].ContentEnd).Text;
                        int start = s.IndexOf("-") - 1;
                        if (start > 1)
                        {
                            debitGroup += s.Substring(start, 1);
                        }
                    }
                }
                foreach (var r in table.RowGroups)
                {
                    foreach (var row in r.Rows)
                    {
                        string s = new TextRange(row.Cells[1].ContentStart, row.Cells[1].ContentEnd).Text;
                        int start = s.IndexOf("-") - 1;
                        if (start > 1)
                        {
                            debitGroup += s.Substring(start, 1);
                        }
                    }
                }

            }
            if (table.Columns.Count == 6)
            {
                string s = new TextRange(table.RowGroups[0].Rows[0].Cells[0].ContentStart, table.RowGroups[0].Rows[0].Cells[0].ContentEnd).Text;
                if (s.Length > 4)
                {
                    autoClose += s.Substring(s.IndexOf("1.  ") + 4, 4);
                }
            }
        }
        VARDebitGroups.Text = debitGroup;
        VARAutoClose.Text = autoClose;




        VARV.Text = FindVARValue("V Number:     ", 9, VARRTF);
        VARMerchantName.Text = FindVARValue("Merchant Name:     ", 50, VARRTF);
        VARMerchantNumber.Text = FindVARValue("Merchant Number:  ", 12, VARRTF);
        VARBin.Text = FindVARValue("BIN:  ", 6, VARRTF);
        VARAgent.Text = FindVARValue("Agent:   ", 6, VARRTF);
        VARChain.Text = FindVARValue("Chain:  ", 6, VARRTF);
        VARTerminalNumber.Text = FindVARValue("Terminal#:  ", 4, VARRTF);
        VARStoreNumber.Text = FindVARValue("Store Number: ", 4, VARRTF);
        VARAddress.Text = FindVARValue("Street Address:", "City:     ", VARRTF);
        VARCity.Text = FindVARValue("City:     ", "State:  ", VARRTF);
        VARState.SelectedValue = FindVARValue("State:  ", 2, VARRTF);
        VARZip.Text = FindVARValue("Postal Code: ", 5, VARRTF);
        VARContactName.Text = FindVARValue("Contact Name:", "Country: ", VARRTF);
        VARPhoneNumber.Text = Regex.Replace(FindVARValue("Phone Number:", 14, VARRTF).Replace("(", "").Replace(")", "").Replace("-", ""), @"(\d{3})(\d{3})(\d{4})", "$1-$2$3").ToString();
        VARTimeZone.SelectedValue = FindVARValue("Time Zone Code: ", "Time Zone Differential:  ", VARRTF);
        VARMCC.Text = FindVARValue("VISA MCC:   ", 4, VARRTF);
        VARAcceptedCards.Text = FindVARValue("Card Type Accepted:", "EDC Primary:", VARRTF);
        VAREDCPrimary.Text = FindVARValue("EDC Primary:", "EDC Secondary:", VARRTF);
        VAREDCSecondary.Text = FindVARValue("EDC Secondary:", "Auth Primary:", VARRTF);
        VARAuthPrimary.Text = FindVARValue("Auth Primary:", "Auth Secondary:", VARRTF);
        VARAuthSecondary.Text = FindVARValue("Auth Secondary:", "Security Code:", VARRTF);
        VARABA.Text = FindVARValue("Merchant ABA Number: ", 12, VARRTF);
        VARSettlementAgent.Text = FindVARValue("Merchant Settlement Agent (FIID): ", 4, VARRTF);
        VARReimbursementAttribute.Text = FindVARValue("Reimbursement Attribute:  ", 1, VARRTF);
        VARComments.Text = FindVARValue("Comments(320 Characters Maximum):", "Please note: ", VARRTF);
        VARFCSID.Text = FindVARValue("EBT FCSID: ", "Networks and Sharing Groups:", VARRTF);
        VARHCPOSID.Text = FindVARValue("HC POS ID:", "Host Capture Auto Close Times:", VARRTF);
        //VARAutoClose.Text = FindVARValue("Host Capture Auto Close Times:", 4, VARRTF);

        ClearVAR.IsEnabled = true;
    }

    private string FindVARValue(string searchText, int valueLength, RichTextBox richTextBox)
    {
        string result;

        TextRange documentRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
        string documentText = documentRange.Text;

        int startIndex = documentText.IndexOf(searchText) + searchText.Length;
        if (startIndex != -1)
        {
            int endIndex = startIndex + valueLength;
            string foundSubstring = documentText.Substring(startIndex, endIndex - startIndex).TrimEnd().TrimStart();

            result = foundSubstring;
        }
        else
        {
            result = "";
        }


        return result;
    }

    private string FindVARValue(string searchText, string endText, RichTextBox richTextBox)
    {
        string result;

        TextRange documentRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
        string documentText = documentRange.Text;

        int startIndex = documentText.IndexOf(searchText) + searchText.Length;
        int endIndex = documentText.IndexOf(endText);
        if (startIndex != -1)
        {
            string foundSubstring = documentText.Substring(startIndex, endIndex - startIndex).TrimEnd().TrimStart();

            result = foundSubstring;
        }
        else
        {
            result = "";
        }


        return result;
    }

    private void OpenHelp(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("For help or feedback please Teams Chat Luke Schaefer", "Help", MessageBoxButton.OK, MessageBoxImage.Question);
    }

    private void ClearVAR_Click(object sender, RoutedEventArgs e)
    {
        VARV.Clear();
        VARABA.Clear();
        VARAcceptedCards.Clear();
        VARAddress.Clear();
        VARAgent.Clear();
        VARAuthPrimary.Clear();
        VARAuthSecondary.Clear();
        VARAutoClose.Clear();
        VARBin.Clear();
        VARChain.Clear();
        VARCity.Clear();
        VARComments.Clear();
        VARContactName.Clear();
        VARDebitGroups.Clear();
        VAREDCPrimary.Clear();
        VAREDCSecondary.Clear();
        VARFCSID.Clear();
        VARHCPOSID.Clear();
        VARMCC.Clear();
        VARMerchantName.Clear();
        VARMerchantNumber.Clear();
        VARPhoneNumber.Clear();
        VARState.SelectedItem = null;
        VARStoreNumber.Clear();
        VARTerminalNumber.Clear();
        VARTimeZone.SelectedItem = null;
        VARZip.Clear();
        VARSettlementAgent.Clear();
        VARReimbursementAttribute.Clear();

        VARRTF.Document.Blocks.Clear();

        ClearVAR.IsEnabled = false;
    }
}