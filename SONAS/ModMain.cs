using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using static IDM.InfoToko;

static class ModMain
{
    private static IDM.Sector a = new IDM.Sector();
    public static string MyKey = "13E32F1995ADC74D23A0AC151E7991ED";
    private static bool isSector = true;
    public static MySqlConnection MasterCon;
    public static void Main(string[] CmdArgs)
    {
        Process[] appProc;
        string strModName, strProcName;
        strModName = Process.GetCurrentProcess().MainModule.ModuleName;
        strProcName = System.IO.Path.GetFileNameWithoutExtension(strModName);
        appProc = Process.GetProcessesByName(strProcName);
        if (appProc.Length > 1)
            return;
        System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
        int counter = 0;
    ulangi:
        ;
        if (isSector)
            MasterCon = a.GetVersionByApp("cetakUlangSonas.exe", MyKey, "kasir");
        else
            MasterCon = new MySqlConnection(Get_KoneksiSQL());
        if (MasterCon == null)
        {
            counter += 1;
            if (counter > 3)
            {
                MessageBox.Show("Key IDM Sector Salah!!");
                return;
            }
            goto ulangi;
        }

        // Dim Frm As New FrmMain
        //frmMainMenu Frm = new frmMainMenu();
        //Frm.ShowDialog();
    }
}
