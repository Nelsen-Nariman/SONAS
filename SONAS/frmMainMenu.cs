using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using MySql.Data.MySqlClient;
using static IDM.Fungsi;
using static IDM.InfoToko;
using CrystalDecisions.Shared;

namespace SONAS
{
    public partial class frmMainMenu : Form
    {
        private int lastClickedGroup = 0;
        private int fileCount = 0;
        private string filePath;
        public static IDM.Sector IdmSector = new IDM.Sector();
        private MySqlConnection Scon = null/* TODO Change to default(_) if this is not a reference type */;
        private MySqlCommand Scom = new MySqlCommand();
        private readonly string kdtk = Get_KodeToko();
        private string cFileSO = "SN" + DateTime.Now.ToString("yyMM") + Get_KodeToko().Substring(0, 1);
        private string cFileSODetil = "SND" + DateTime.Now.ToString("yyMM") + Get_KodeToko().Substring(0, 1);
        private string cFileSOEdit = "SNE" + DateTime.Now.ToString("yyMM") + Get_KodeToko().Substring(0, 1);
        private string cFileSOUP = "SNUP" + DateTime.Now.ToString("yyMM") + Get_KodeToko().Substring(0, 1);

        public frmMainMenu()
        {
            InitializeComponent();
        }

        private void frmMainMenu_Load(object sender, EventArgs e)
        {
            this.Text = this.Text + " v." + Application.ProductVersion;

            Scon = IdmSector.GetVersionV2(ModMain.MyKey, Application.StartupPath + @"\cetakUlangSonas.exe", "kasir");
            Scom = new MySqlCommand("", Scon);

            if (Scon.State == ConnectionState.Closed)
                Scon.Open();

            ComboBox1.Items.Clear();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            MySqlConnection Scon = new MySqlConnection();
            MySqlCommand Scom = new MySqlCommand();
            string sql;
            try
            {
                Scon = IdmSector.GetVersionV2(ModMain.MyKey, Application.StartupPath + @"\cetakUlangSonas.exe", "kasir");
                Scom = new MySqlCommand("", Scon);
                //Shell(Application.StartupPath + @"\SO.Net.exe");

                if (Scon.State == ConnectionState.Closed)
                    Scon.Open();

                Scom = new MySqlCommand("", Scon);

                //sql = "SELECT COUNT(*) FROM " + cFileSO + " ";
                //sql += "WHERE SOID='' AND (DRAFT = '' OR DRAFT = NULL);";
                //Scom.CommandText = sql;
                //TraceLog("btnGroup1: " + sql, TipeTracelog.Info);
                //if (!IsTableExists(cFileSO))
                //{
                //    MessageBox.Show("Belum ada data SONAS", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    return;
                //}

                //int count = Convert.ToInt32(Scom.ExecuteScalar());
                //if (count == 0)
                //{
                //    MessageBox.Show("Anda tidak bisa melakukan proses cetak Group 1", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    return;
                //}

                filePath = IDM.InfoToko.Get_PathIDM() + @"\SONAS\Group_1\";
                fileCount = System.IO.Directory.GetFiles(filePath).Length;

                if (fileCount >= 7)
                {
                    lastClickedGroup = 1;
                    ComboBox1.Items.Clear();
                    ComboBox1.Items.AddRange(new object[] {
                        "LPP closing harian Toko Idm.",
                        "Lap rekap perbandingan ke-1 LPP vs file SONAS",
                        "Lap rincian perbandingan ke-1 LPP vs file SONAS",
                        "Register SP sebelum SONAS",
                        "Cetakan SP sebelum SONAS",
                        "Register BPB sebelum SONAS",
                        "Cetakan BPB sebelum SONAS"
                    });
                    ComboBox1.SelectedIndex = 0;
                    ComboBox1.Enabled = true;
                    Button9.Enabled = true;
                }
                else
                {
                    //dsSONAS dsSO = new dsSONAS();
                    //dsSONASNew dsSONew = new dsSONASNew();
                    System.Diagnostics.Process.Start(Application.StartupPath + "\\" + "SO.Net.exe", "-SONAS -GROUP1 -cetakLPPTK");
                    string[] files = System.IO.Directory.GetFiles(filePath, "LPPTK KE 1*.pdf");

                    if (files.Length == 0)
                    {
                        System.Threading.Thread.Sleep(3000);
                        MessageBox.Show("File pdf LPPTK belum tersimpan", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    files = System.IO.Directory.GetFiles(filePath, "LPPTK KE 1*.csv");
                    if (files.Length == 0)
                    {
                        System.Threading.Thread.Sleep(3000);
                        MessageBox.Show("File csv LPPTK belum tersimpan", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }


                    System.Threading.Thread.Sleep(5000);
                    System.Diagnostics.Process.Start(Application.StartupPath + "\\" + "SO.Net.exe", "-SONAS -GROUP1 -cetakRekapLPPvsSONAS");
                    files = System.IO.Directory.GetFiles(filePath, "LAP PERBANDINGAN KE 1*.pdf");

                    if (files.Length == 0)
                    {
                        System.Threading.Thread.Sleep(3000);
                        MessageBox.Show("File LAP PERBANDINGAN KE 1 belum tersimpan", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    System.Threading.Thread.Sleep(5000);
                    System.Diagnostics.Process.Start(Application.StartupPath + "\\" + "SO.Net.exe", "-SONAS -GROUP1 -cetakRincianLPP1vsSONAS");
                    files = System.IO.Directory.GetFiles(filePath, "LAP RINCIAN ITEM SELISIH PERBANDINGAN KE 1*.pdf");

                    if (files.Length == 0)
                    {
                        System.Threading.Thread.Sleep(3000);
                        MessageBox.Show("File LAP RINCIAN ITEM SELISIH PERBANDINGAN KE 1 belum tersimpan", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    System.Threading.Thread.Sleep(5000);
                    System.Diagnostics.Process.Start(Application.StartupPath + "\\" + "SO.Net.exe", "-SONAS -GROUP1 -cetakRegisStrukSebelumSONAS");
                    files = System.IO.Directory.GetFiles(filePath, "REGISTER STRUK SEBELUM SONAS*.pdf");

                    if (files.Length == 0)
                    {
                        System.Threading.Thread.Sleep(3000);
                        MessageBox.Show("File REGISTER STRUK SEBELUM SONAS belum tersimpan", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    files = System.IO.Directory.GetFiles(filePath, "BUKTI STRUK SEBELUM SONAS*.pdf");

                    if (files.Length == 0)
                    {
                        System.Threading.Thread.Sleep(3000);
                        MessageBox.Show("File BUKTI STRUK SEBELUM SONAS belum tersimpan", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }


                    System.Threading.Thread.Sleep(5000);
                    System.Diagnostics.Process.Start(Application.StartupPath + "\\" + "SO.Net.exe", "-SONAS -GROUP1 -cetakRegisBPBSebelumSONAS");
                    files = System.IO.Directory.GetFiles(filePath, "REGISTER BPB SEBELUM SONAS*.pdf");

                    if (files.Length == 0)
                    {
                        System.Threading.Thread.Sleep(3000);
                        MessageBox.Show("File REGISTER BPB SEBELUM SONAS belum tersimpan", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    files = System.IO.Directory.GetFiles(filePath, "BUKTI BPB SEBELUM SONAS*.pdf");

                    if (files.Length == 0)
                    {
                        System.Threading.Thread.Sleep(3000);
                        MessageBox.Show("File BUKTI BPB SEBELUM SONAS belum tersimpan", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    // FrmMain.regisBPB(dsSO, filePath, True)
                    // FrmMain.buktiBPB(dsSO, filePath, True)
                    // FrmMain.regisBPB(dsSO, filePath, False)
                    // FrmMain.regisStruk(dsSO, dsSONew, filePath, True)
                    // FrmMain.buktiStruk(dsSO, dsSONew, filePath, True)
                    // FrmMain.regisStruk(dsSO, dsSONew, filePath, False)

                    lastClickedGroup = 1;
                    ComboBox1.Items.Clear();
                    ComboBox1.Items.AddRange(new object[] {
                        "LPP closing harian Toko Idm.",
                        "Lap rekap perbandingan ke-1 LPP vs file SONAS",
                        "Lap rincian perbandingan ke-1 LPP vs file SONAS",
                        "Register SP sebelum SONAS",
                        "Cetakan SP sebelum SONAS",
                        "Register BPB sebelum SONAS",
                        "Cetakan BPB sebelum SONAS"
                    });
                    ComboBox1.SelectedIndex = 0;
                    ComboBox1.Enabled = true;
                    Button9.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error Group 1", MessageBoxButtons.OK, MessageBoxIcon.Information);
                TraceLog("Group 1 Error: " + ex.Message + ex.StackTrace);
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            MySqlConnection Scon = new MySqlConnection();
            MySqlCommand Scom = new MySqlCommand();
            string sql;

            try
            {
                Scon = IdmSector.GetVersionV2(ModMain.MyKey, Application.StartupPath + @"\cetakUlangSonas.exe", "kasir");
                Scom = new MySqlCommand("", Scon);

                if (Scon.State == ConnectionState.Closed)
                    Scon.Open();

                Scom = new MySqlCommand("", Scon);

                sql = "SELECT COUNT(*) FROM " + cFileSOEdit + " ";
                sql += "WHERE SOID='I' AND DRAFT = '1';";
                Scom.CommandText = sql;
                TraceLog("btnGroup2: " + sql, TipeTracelog.Info);
                if (!IsTableExists(cFileSOEdit))
                {
                    MessageBox.Show("Belum ada data SONAS Edit", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; 
                }

                int count = Convert.ToInt32(Scom.ExecuteScalar());
                if (count == 0)
                {
                    MessageBox.Show("Anda tidak bisa melakukan proses cetak Group 2", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                filePath = IDM.InfoToko.Get_PathIDM() + @"\SONAS\Group_2\";
                fileCount = System.IO.Directory.GetFiles(filePath).Length;

                if (fileCount >= 2)
                {
                    lastClickedGroup = 2;
                    ComboBox1.Items.Clear();
                    ComboBox1.Items.AddRange(new object[] {
                        "Laporan NKL Draft SONAS",
                        "KKIP"
                    });
                    ComboBox1.SelectedIndex = 0;
                    ComboBox1.Enabled = true;
                    Button9.Enabled = true;
                }
                else
                {
                    System.Diagnostics.Process.Start(Application.StartupPath + "\\" + "SO.Net.exe", "-SONAS -GROUP2 -cetakLaporanNKLSementara");
                    string[] files = System.IO.Directory.GetFiles(filePath, "LAP NKL SEMENTARA PRE ADJUST 1*.pdf");

                    if (files.Length == 0)
                    {
                        System.Threading.Thread.Sleep(3000);
                        MessageBox.Show("File LAP NKL SEMENTARA PRE ADJUST 1 belum tersimpan", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    System.Threading.Thread.Sleep(5000);
                    System.Diagnostics.Process.Start(Application.StartupPath + "\\" + "SO.Net.exe", "-SONAS -GROUP2 -cetakLaporanKKIP");
                    files = System.IO.Directory.GetFiles(filePath, "KKIP (KERTAS KERJA ITEM PARETO)*.pdf");

                    if (files.Length == 0)
                    {
                        System.Threading.Thread.Sleep(3000);
                        MessageBox.Show("File KKIP (KERTAS KERJA ITEM PARETO) belum tersimpan", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    lastClickedGroup = 2;
                    ComboBox1.Items.Clear();
                    ComboBox1.Items.AddRange(new object[] {
                        "Laporan NKL Draft SONAS",
                        "KKIP"
                    });
                    ComboBox1.SelectedIndex = 0;
                    ComboBox1.Enabled = true;
                    Button9.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error Group 2", MessageBoxButtons.OK, MessageBoxIcon.Information);
                TraceLog("Group 2 Error: " + ex.Message + ex.StackTrace);
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            MySqlConnection Scon = new MySqlConnection();
            MySqlCommand Scom = new MySqlCommand();
            string sql;

            try
            {
                Scon = IdmSector.GetVersionV2(ModMain.MyKey, Application.StartupPath + @"\cetakUlangSonas.exe", "kasir");
                Scom = new MySqlCommand("", Scon);

                if (Scon.State == ConnectionState.Closed)
                    Scon.Open();

                Scom = new MySqlCommand("", Scon);

                sql = "SELECT COUNT(*) FROM " + cFileSO + " ";
                sql += "WHERE SOID='L' AND DRAFT = '1';";
                Scom.CommandText = sql;
                TraceLog("btnGroup3: " + sql, TipeTracelog.Info);
                if (!IsTableExists(cFileSO))
                {
                    MessageBox.Show("Belum ada data SONAS", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int count = Convert.ToInt32(Scom.ExecuteScalar());
                if (count == 0)
                {
                    MessageBox.Show("Anda tidak bisa melakukan proses cetak Group 3", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                filePath = IDM.InfoToko.Get_PathIDM() + @"\SONAS\Group_3\";
                fileCount = System.IO.Directory.GetFiles(filePath).Length;

                if (fileCount >= 2)
                {
                    lastClickedGroup = 3;
                    ComboBox1.Items.Clear();
                    ComboBox1.Items.AddRange(new object[] {
                        "LHSONAS Harga Jual",
                        "LHSONAS HPP"
                    });
                    ComboBox1.SelectedIndex = 0;
                    ComboBox1.Enabled = true;
                    Button9.Enabled = true;
                }
                else
                {
                    System.Diagnostics.Process.Start(Application.StartupPath + "\\" + "SO.Net.exe", "-SONAS -GROUP3 -cetakLaporanLHSOHargaJual");
                    string[] files = System.IO.Directory.GetFiles(filePath, "LHSO (HARGA JUAL)*.pdf");

                    if (files.Length == 0)
                    {
                        System.Threading.Thread.Sleep(3000);
                        MessageBox.Show("File LHSO (HARGA JUAL) belum tersimpan", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    System.Threading.Thread.Sleep(5000);
                    System.Diagnostics.Process.Start(Application.StartupPath + "\\" + "SO.Net.exe", "-SONAS -GROUP3 -cetakLaporanLHSOHargaHPP");
                    files = System.IO.Directory.GetFiles(filePath, "LHSO (HARGA HPP)*.pdf");

                    if (files.Length == 0)
                    {
                        System.Threading.Thread.Sleep(3000);
                        MessageBox.Show("File LHSO (HARGA HPP) belum tersimpan", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    lastClickedGroup = 3;
                    ComboBox1.Items.Clear();
                    ComboBox1.Items.AddRange(new object[] {
                        "LHSONAS Harga Jual",
                        "LHSONAS HPP"
                    });
                    ComboBox1.SelectedIndex = 0;
                    ComboBox1.Enabled = true;
                    Button9.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error Group 3", MessageBoxButtons.OK, MessageBoxIcon.Information);
                TraceLog("Group 3 Error: " + ex.Message + ex.StackTrace);
            }
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            MySqlConnection Scon = new MySqlConnection();
            MySqlCommand Scom = new MySqlCommand();
            string sql;

            try
            {
                Scon = IdmSector.GetVersionV2(ModMain.MyKey, Application.StartupPath + @"\cetakUlangSonas.exe", "kasir");
                Scom = new MySqlCommand("", Scon);

                if (Scon.State == ConnectionState.Closed)
                    Scon.Open();

                Scom = new MySqlCommand("", Scon);

                sql = "SELECT COUNT(*) FROM " + cFileSO + " ";
                sql += "WHERE SOID='A' AND DRAFT = '1';";
                Scom.CommandText = sql;
                TraceLog("btnGroup4: " + sql, TipeTracelog.Info);
                if (!IsTableExists(cFileSO))
                {
                    MessageBox.Show("Belum ada data SONAS", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int count = Convert.ToInt32(Scom.ExecuteScalar());
                if (count == 0)
                {
                    MessageBox.Show("Anda tidak bisa melakukan proses cetak Group 4", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                filePath = IDM.InfoToko.Get_PathIDM() + @"\SONAS\Group_4\";
                fileCount = System.IO.Directory.GetFiles(filePath).Length;

                if (fileCount >= 6)
                {
                    lastClickedGroup = 4;
                    ComboBox1.Items.Clear();
                    ComboBox1.Items.AddRange(new object[] {
                        "Berita Acara SONAS Harga Jual",
                        "Berita Acara SONAS HPP",
                        "LPP adjustment SONAS Toko Idm.",
                        "Lap rekap perbandingan ke-2 LPP vs file SONAS",
                        "Lap rincian perbandingan ke-2 LPP vs file SONAS",
                        "Proses download file SN"
                    });
                    ComboBox1.SelectedIndex = 0;
                    ComboBox1.Enabled = true;
                    Button9.Enabled = true;
                }
                else
                {
                    System.Diagnostics.Process.Start(Application.StartupPath + "\\" + "SO.Net.exe", "-SONAS -GROUP4 -cetakBASOSONASHargaJual");
                    string[] files = System.IO.Directory.GetFiles(filePath, "BASO SONAS (HARGA JUAL)*.pdf");

                    if (files.Length == 0)
                    {
                        System.Threading.Thread.Sleep(3000);
                        MessageBox.Show("File BASO SONAS (HARGA JUAL) belum tersimpan", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    System.Threading.Thread.Sleep(5000);
                    System.Diagnostics.Process.Start(Application.StartupPath + "\\" + "SO.Net.exe", "-SONAS -GROUP4 -cetakBASOSONASHargaHPP");
                    files = System.IO.Directory.GetFiles(filePath, "BASO SONAS (HARGA HPP)*.pdf");

                    if (files.Length == 0)
                    {
                        System.Threading.Thread.Sleep(3000);
                        MessageBox.Show("File BASO SONAS (HARGA HPP) belum tersimpan", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    System.Threading.Thread.Sleep(5000);
                    System.Diagnostics.Process.Start(Application.StartupPath + "\\" + "SO.Net.exe", "-SONAS -GROUP4 -cetakLPPTK");
                    files = System.IO.Directory.GetFiles(filePath, "LPPTK KE 2*.pdf");

                    if (files.Length == 0)
                    {
                        System.Threading.Thread.Sleep(3000);
                        MessageBox.Show("File pdf LPPTK KE 2 belum tersimpan", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    files = System.IO.Directory.GetFiles(filePath, "LPPTK KE 2*.csv");

                    if (files.Length == 0)
                    {
                        System.Threading.Thread.Sleep(3000);
                        MessageBox.Show("File csv LPPTK KE 2 belum tersimpan", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }


                    System.Threading.Thread.Sleep(5000);
                    System.Diagnostics.Process.Start(Application.StartupPath + "\\" + "SO.Net.exe", "-SONAS -GROUP4 -cetakRekapLPPvsBASOIC");
                    files = System.IO.Directory.GetFiles(filePath, "LAP PERBANDINGAN KE 2*.pdf");

                    if (files.Length == 0)
                    {
                        System.Threading.Thread.Sleep(3000);
                        MessageBox.Show("File LAP PERBANDINGAN KE 2 belum tersimpan", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    System.Threading.Thread.Sleep(5000);
                    System.Diagnostics.Process.Start(Application.StartupPath + "\\" + "SO.Net.exe", "-SONAS -GROUP4 -cetakRincianLPP2vsBASOIC");
                    files = System.IO.Directory.GetFiles(filePath, "LAP RINCIAN ITEM SELISIH PERBANDINGAN KE 2*.pdf");

                    if (files.Length == 0)
                    {
                        System.Threading.Thread.Sleep(3000);
                        MessageBox.Show("File LAP RINCIAN ITEM SELISIH PERBANDINGAN KE 2 belum tersimpan", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    lastClickedGroup = 4;
                    ComboBox1.Items.Clear();
                    ComboBox1.Items.AddRange(new object[] {
                        "Berita Acara SONAS Harga Jual",
                        "Berita Acara SONAS HPP",
                        "LPP adjustment SONAS Toko Idm.",
                        "Lap rekap perbandingan ke-2 LPP vs file SONAS",
                        "Lap rincian perbandingan ke-2 LPP vs file SONAS",
                        "Proses download file SN"
                    });
                    ComboBox1.SelectedIndex = 0;
                    ComboBox1.Enabled = true;
                    Button9.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error Group 4", MessageBoxButtons.OK, MessageBoxIcon.Information); TraceLog("Group 4 Error: " + ex.Message + ex.StackTrace);
            }
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            MySqlConnection Scon = new MySqlConnection();
            MySqlCommand Scom = new MySqlCommand();
            string sql;

            try
            {
                Scon = IdmSector.GetVersionV2(ModMain.MyKey, Application.StartupPath + @"\cetakUlangSonas.exe", "kasir");
                Scom = new MySqlCommand("", Scon);

                if (Scon.State == ConnectionState.Closed)
                    Scon.Open();

                Scom = new MySqlCommand("", Scon);

                sql = "SELECT COUNT(*) FROM " + cFileSOUP + " ";
                sql += "(WHERE SOID='' OR SOID = NULL) AND (DRAFT = '' OR DRAFT = NULL);";
                Scom.CommandText = sql;
                TraceLog("btnGroup5: " + sql, TipeTracelog.Info);
                if (!IsTableExists(cFileSOUP))
                {
                    MessageBox.Show("Belum ada data SNUP", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int count = Convert.ToInt32(Scom.ExecuteScalar());
                if (count == 0)
                {
                    MessageBox.Show("Anda tidak bisa melakukan proses cetak Group 5", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                filePath = IDM.InfoToko.Get_PathIDM() + @"\SONAS\Group_5\";
                fileCount = System.IO.Directory.GetFiles(filePath).Length;

                if (fileCount >= 1)
                {
                    lastClickedGroup = 5;
                    ComboBox1.Items.Clear();
                    ComboBox1.Items.AddRange(new object[] {
                        "KKUP"
                    });
                    ComboBox1.SelectedIndex = 0;
                    ComboBox1.Enabled = true;
                    Button9.Enabled = true;
                }
                else
                {
                    System.Diagnostics.Process.Start(Application.StartupPath + "\\" + "SO.Net.exe", "-SONAS -GROUP5 -cetakFileKKUP");
                    string[] files = System.IO.Directory.GetFiles(filePath, "KKUP (KERTAS KERJA UJI PETIK)*.pdf");

                    if (files.Length == 0)
                    {
                        System.Threading.Thread.Sleep(3000);
                        MessageBox.Show("File pdf KKUP (KERTAS KERJA UJI PETIK) belum tersimpan", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    files = System.IO.Directory.GetFiles(filePath, "KKUP (KERTAS KERJA UJI PETIK)*.csv");
                    if (files.Length == 0)
                    {
                        System.Threading.Thread.Sleep(3000);
                        MessageBox.Show("File csv KKUP (KERTAS KERJA UJI PETIK) belum tersimpan", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    lastClickedGroup = 5;
                    ComboBox1.Items.Clear();
                    ComboBox1.Items.AddRange(new object[] {
                        "KKUP"
                    });
                    ComboBox1.SelectedIndex = 0;
                    ComboBox1.Enabled = true;
                    Button9.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error Group 5", MessageBoxButtons.OK, MessageBoxIcon.Information);
                TraceLog("Group 5 Error: " + ex.Message + ex.StackTrace);
            }
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            MySqlConnection Scon = new MySqlConnection();
            MySqlCommand Scom = new MySqlCommand();
            string sql;

            try
            {
                Scon = IdmSector.GetVersionV2(ModMain.MyKey, Application.StartupPath + @"\cetakUlangSonas.exe", "kasir");
                Scom = new MySqlCommand("", Scon);

                if (Scon.State == ConnectionState.Closed)
                    Scon.Open();

                Scom = new MySqlCommand("", Scon);

                sql = "SELECT COUNT(*) FROM " + cFileSOUP + " ";
                sql += "WHERE SOID='L' AND DRAFT = '1';";
                Scom.CommandText = sql;
                TraceLog("btnGroup6: " + sql, TipeTracelog.Info);
                if (!IsTableExists(cFileSOUP))
                {
                    MessageBox.Show("Belum ada data SNUP", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int count = Convert.ToInt32(Scom.ExecuteScalar());
                if (count == 0)
                {
                    MessageBox.Show("Anda tidak bisa melakukan proses cetak Group 6", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                filePath = IDM.InfoToko.Get_PathIDM() + @"\SONAS\Group_6\";
                fileCount = System.IO.Directory.GetFiles(filePath).Length;

                if (fileCount >= 2)
                {
                    lastClickedGroup = 6;
                    ComboBox1.Items.Clear();
                    ComboBox1.Items.AddRange(new object[] {
                        "LHSONAS Uji Petik Harga Jual",
                        "LHSONAS Uji Petik HPP"
                    });
                    ComboBox1.SelectedIndex = 0;
                    ComboBox1.Enabled = true;
                    Button9.Enabled = true;
                }
                else
                {
                    System.Diagnostics.Process.Start(Application.StartupPath + "\\" + "SO.Net.exe", "-SONAS -GROUP6 -cetakFileLHSOUjiPetikHargaJual");
                    string[] files = System.IO.Directory.GetFiles(filePath, "LHSO UJI PETIK (HARGA JUAL)*.pdf");

                    if (files.Length == 0)
                    {
                        System.Threading.Thread.Sleep(3000);
                        MessageBox.Show("File LHSO UJI PETIK (HARGA JUAL) belum tersimpan", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    System.Threading.Thread.Sleep(5000);
                    System.Diagnostics.Process.Start(Application.StartupPath + "\\" + "SO.Net.exe", "-SONAS -GROUP6 -cetakFileLHSOUjiPetikHargaHPP");
                    files = System.IO.Directory.GetFiles(filePath, "LHSO UJI PETIK (HARGA HPP)*.pdf");

                    if (files.Length == 0)
                    {
                        System.Threading.Thread.Sleep(3000);
                        MessageBox.Show("File LHSO UJI PETIK (HARGA HPP) belum tersimpan", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    lastClickedGroup = 6;
                    ComboBox1.Items.Clear();
                    ComboBox1.Items.AddRange(new object[] {
                        "LHSONAS Uji Petik Harga Jual",
                        "LHSONAS Uji Petik HPP"
                    });
                    ComboBox1.SelectedIndex = 0;
                    ComboBox1.Enabled = true;
                    Button9.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error Group 6", MessageBoxButtons.OK, MessageBoxIcon.Information);
                TraceLog("Group 6 Error: " + ex.Message + ex.StackTrace);
            }
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            MySqlConnection Scon = new MySqlConnection();
            MySqlCommand Scom = new MySqlCommand();
            string sql;

            try
            {
                Scon = IdmSector.GetVersionV2(ModMain.MyKey, Application.StartupPath + @"\cetakUlangSonas.exe", "kasir");
                Scom = new MySqlCommand("", Scon);

                if (Scon.State == ConnectionState.Closed)
                    Scon.Open();

                Scom = new MySqlCommand("", Scon);

                sql = "SELECT COUNT(*) FROM " + cFileSOUP + " ";
                sql += "WHERE SOID='A' AND DRAFT = '1';";
                Scom.CommandText = sql;
                TraceLog("btnGroup7: " + sql, TipeTracelog.Info);
                if (!IsTableExists(cFileSOUP))
                {
                    MessageBox.Show("Belum ada data SNUP", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int count = Convert.ToInt32(Scom.ExecuteScalar());
                if (count == 0)
                {
                    MessageBox.Show("Anda tidak bisa melakukan proses cetak Group 7", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                filePath = IDM.InfoToko.Get_PathIDM() + @"\SONAS\Group_7\";
                fileCount = System.IO.Directory.GetFiles(filePath).Length;

                if (fileCount >= 5)
                {
                    lastClickedGroup = 7;
                    ComboBox1.Items.Clear();
                    ComboBox1.Items.AddRange(new object[] {
                        "BASONAS Uji Petik Harga Jual",
                        "BASONAS Uji Petik HPP",
                        "LPP adjustment SONAS Uji Petik Toko Idm.",
                        "Lap rekap perbandingan ke-3 LPP vs file SONAS",
                        "Lap rincian perbandingan ke-3 LPP vs file SONAS"
                    });
                    ComboBox1.SelectedIndex = 0;
                    ComboBox1.Enabled = true;
                    Button9.Enabled = true;
                }
                else
                {
                    System.Diagnostics.Process.Start(Application.StartupPath + "\\" + "SO.Net.exe", "-SONAS -GROUP7 -cetakBASOUjiPetikHargaJual");
                    string[] files = System.IO.Directory.GetFiles(filePath, "BASO UJI PETIK (HARGA JUAL)*.pdf");

                    if (files.Length == 0)
                    {
                        System.Threading.Thread.Sleep(3000);
                        MessageBox.Show("File BASO UJI PETIK (HARGA JUAL) belum tersimpan", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    System.Threading.Thread.Sleep(5000);
                    System.Diagnostics.Process.Start(Application.StartupPath + "\\" + "SO.Net.exe", "-SONAS -GROUP7 -cetakBASOUjiPetikHargaHPP");
                    files = System.IO.Directory.GetFiles(filePath, "BASO UJI PETIK (HARGA HPP)*.pdf");

                    if (files.Length == 0)
                    {
                        System.Threading.Thread.Sleep(3000);
                        MessageBox.Show("File BASO UJI PETIK (HARGA HPP) belum tersimpan", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    System.Threading.Thread.Sleep(5000);
                    System.Diagnostics.Process.Start(Application.StartupPath + "\\" + "SO.Net.exe", "-SONAS -GROUP7 -cetakLPPTK");
                    files = System.IO.Directory.GetFiles(filePath, "LPPTK KE 3 (SOUP)*.pdf");

                    if (files.Length == 0)
                    {
                        System.Threading.Thread.Sleep(3000);
                        MessageBox.Show("File pdf LPPTK KE 3 (SOUP) belum tersimpan", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    files = System.IO.Directory.GetFiles(filePath, "LPPTK KE 3 (SOUP)*.csv");

                    if (files.Length == 0)
                    {
                        System.Threading.Thread.Sleep(3000);
                        MessageBox.Show("File csv LPPTK KE 3 (SOUP) belum tersimpan", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }


                    System.Threading.Thread.Sleep(5000);
                    System.Diagnostics.Process.Start(Application.StartupPath + "\\" + "SO.Net.exe", "-SONAS -GROUP7 -cetakRekapLPPvsBASOIC");
                    files = System.IO.Directory.GetFiles(filePath, "LAP PERBANDINGAN KE 3 (SOUP)*.pdf");

                    if (files.Length == 0)
                    {
                        System.Threading.Thread.Sleep(3000);
                        MessageBox.Show("File LAP PERBANDINGAN KE 3 (SOUP) belum tersimpan", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    System.Threading.Thread.Sleep(5000);
                    System.Diagnostics.Process.Start(Application.StartupPath + "\\" + "SO.Net.exe", "-SONAS -GROUP7 -cetakRincianLPP2vsBASOIC");
                    files = System.IO.Directory.GetFiles(filePath, "LAP RINCIAN ITEM SELISIH PERBANDINGAN KE 3 (SOUP)*.pdf");

                    if (files.Length == 0)
                    {
                        System.Threading.Thread.Sleep(3000);
                        MessageBox.Show("File LAP RINCIAN ITEM SELISIH PERBANDINGAN KE 3 (SOUP) belum tersimpan", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    lastClickedGroup = 7;
                    ComboBox1.Items.Clear();
                    ComboBox1.Items.AddRange(new object[] {
                        "BASONAS Uji Petik Harga Jual",
                        "BASONAS Uji Petik HPP",
                        "LPP adjustment SONAS Uji Petik Toko Idm.",
                        "Lap rekap perbandingan ke-3 LPP vs file SONAS",
                        "Lap rincian perbandingan ke-3 LPP vs file SONAS"
                    });
                    ComboBox1.SelectedIndex = 0;
                    ComboBox1.Enabled = true;
                    Button9.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error Group 7", MessageBoxButtons.OK, MessageBoxIcon.Information);
                TraceLog("Group 7 Error: " + ex.Message + ex.StackTrace);
            }
        }

        private void Button8_Click(object sender, EventArgs e)
        {
            MySqlConnection Scon = new MySqlConnection();
            MySqlCommand Scom = new MySqlCommand();
            string sql;

            try
            {
                Scon = IdmSector.GetVersionV2(ModMain.MyKey, Application.StartupPath + @"\cetakUlangSonas.exe", "kasir");
                Scom = new MySqlCommand("", Scon);

                if (Scon.State == ConnectionState.Closed)
                    Scon.Open();

                Scom = new MySqlCommand("", Scon);

                sql = "SELECT COUNT(*) FROM " + cFileSOUP + " ";
                sql += "WHERE SOID='A' AND DRAFT = '1';";
                Scom.CommandText = sql;
                TraceLog("btnGroup7: " + sql, TipeTracelog.Info);
                if (!IsTableExists(cFileSOUP))
                {
                    MessageBox.Show("Belum ada data SNUP", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int count = Convert.ToInt32(Scom.ExecuteScalar());
                if (count == 0)
                {
                    MessageBox.Show("Anda tidak bisa melakukan proses cetak Group 8", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                filePath = IDM.InfoToko.Get_PathIDM() + @"\SONAS\Group_8\";
                fileCount = System.IO.Directory.GetFiles(filePath).Length;

                if (fileCount >= 4)
                {
                    lastClickedGroup = 8;
                    ComboBox1.Items.Clear();
                    ComboBox1.Items.AddRange(new object[] {
                        "Register SP setelah SONAS",
                        "Cetakan SP setelah SONAS",
                        "Register BPB setelah SONAS",
                        "Cetakan BPB setelah SONAS"
                    });
                    ComboBox1.SelectedIndex = 0;
                    ComboBox1.Enabled = true;
                    Button9.Enabled = true;
                }
                else
                {
                    System.Diagnostics.Process.Start(Application.StartupPath + "\\" + "SO.Net.exe", "-SONAS -GROUP8 -cetakRegisStrukSetelahSONAS");
                    string[] files = System.IO.Directory.GetFiles(filePath, "REGISTER STRUK SETELAH SONAS*.pdf");

                    if (files.Length == 0)
                    {
                        System.Threading.Thread.Sleep(3000);
                        MessageBox.Show("File REGISTER STRUK SETELAH SONAS belum tersimpan", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    files = System.IO.Directory.GetFiles(filePath, "BUKTI STRUK SETELAH SONAS*.pdf");

                    if (files.Length == 0)
                    {
                        System.Threading.Thread.Sleep(3000);
                        MessageBox.Show("File BUKTI STRUK SETELAH SONAS belum tersimpan", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }


                    System.Threading.Thread.Sleep(5000);
                    System.Diagnostics.Process.Start(Application.StartupPath + "\\" + "SO.Net.exe", "-SONAS -GROUP8 -cetakRegisBPBSetelahSONAS");
                    files = System.IO.Directory.GetFiles(filePath, "REGISTER BPB SETELAH SONAS*.pdf");

                    if (files.Length == 0)
                    {
                        System.Threading.Thread.Sleep(3000);
                        MessageBox.Show("File REGISTER BPB SETELAH SONAS belum tersimpan", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    files = System.IO.Directory.GetFiles(filePath, "BUKTI BPB SETELAH SONAS*.pdf");

                    if (files.Length == 0)
                    {
                        System.Threading.Thread.Sleep(3000);
                        MessageBox.Show("File BUKTI BPB SETELAH SONAS belum tersimpan", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    lastClickedGroup = 8;
                    ComboBox1.Items.Clear();
                    ComboBox1.Items.AddRange(new object[] {
                        "Register SP setelah SONAS",
                        "Cetakan SP setelah SONAS",
                        "Register BPB setelah SONAS",
                        "Cetakan BPB setelah SONAS"
                    });
                    ComboBox1.SelectedIndex = 0;
                    ComboBox1.Enabled = true;
                    Button9.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error Group 8", MessageBoxButtons.OK, MessageBoxIcon.Information);
                TraceLog("Group 8 Error: " + ex.Message + ex.StackTrace);
            }
        }


        private void Button9_Click(object sender, EventArgs e)
        {
            switch (lastClickedGroup)
            {
                case 1:
                    {
                        if (ComboBox1.SelectedItem.ToString() == "LPP closing harian Toko Idm.")
                        {
                        }
                        else if (ComboBox1.SelectedItem.ToString() == "Lap rekap perbandingan ke-1 LPP vs file SONAS")
                        {
                        }
                        else if (ComboBox1.SelectedItem.ToString() == "Lap rincian perbandingan ke-1 LPP vs file SONAS")
                        {
                        }
                        else if (ComboBox1.SelectedItem.ToString() == "Register SP sebelum SONAS")
                        {
                        }
                        else if (ComboBox1.SelectedItem.ToString() == "Cetakan SP sebelum SONAS")
                        {
                        }
                        else if (ComboBox1.SelectedItem.ToString() == "Register BPB sebelum SONAS")
                        {
                        }
                        else if (ComboBox1.SelectedItem.ToString() == "Cetakan BPB sebelum SONAS")
                        {
                        }

                        break;
                    }

                case 2:
                    {
                        if (ComboBox1.SelectedItem.ToString() == "Laporan NKL Draft SONAS")
                        {
                        }
                        else if (ComboBox1.SelectedItem.ToString() == "KKIP")
                        {
                        }

                        break;
                    }

                case 3:
                    {
                        if (ComboBox1.SelectedItem.ToString() == "LHSONAS Harga Jual")
                        {
                        }
                        else if (ComboBox1.SelectedItem.ToString() == "LHSONAS HPP")
                        {
                        }

                        break;
                    }

                case 4:
                    {
                        if (ComboBox1.SelectedItem.ToString() == "Berita Acara SONAS Harga Jual")
                        {
                        }
                        else if (ComboBox1.SelectedItem.ToString() == "Berita Acara SONAS HPP")
                        {
                        }
                        else if (ComboBox1.SelectedItem.ToString() == "LPP adjustment SONAS Toko Idm.")
                        {
                        }
                        else if (ComboBox1.SelectedItem.ToString() == "Lap rekap perbandingan ke-2 LPP vs file SONAS")
                        {
                        }
                        else if (ComboBox1.SelectedItem.ToString() == "Lap rincian perbandingan ke-2 LPP vs file SONAS")
                        {
                        }
                        else if (ComboBox1.SelectedItem.ToString() == "Proses download file SN")
                        {
                        }

                        break;
                    }

                case 5:
                    {
                        if (ComboBox1.SelectedItem.ToString() == "KKUP")
                        {
                        }

                        break;
                    }

                case 6:
                    {
                        if (ComboBox1.SelectedItem.ToString() == "LHSONAS Uji Petik Harga Jual")
                        {
                        }
                        else if (ComboBox1.SelectedItem.ToString() == "LHSONAS Uji Petik HPP")
                        {
                        }

                        break;
                    }

                case 7:
                    {
                        if (ComboBox1.SelectedItem.ToString() == "BASONAS Uji Petik Harga Jual")
                        {
                        }
                        else if (ComboBox1.SelectedItem.ToString() == "BASONAS Uji Petik HPP")
                        {
                        }
                        else if (ComboBox1.SelectedItem.ToString() == "LPP adjustment SONAS Uji Petik Toko Idm.")
                        {
                        }
                        else if (ComboBox1.SelectedItem.ToString() == "Lap rekap perbandingan ke-3 LPP vs file SONAS")
                        {
                        }
                        else if (ComboBox1.SelectedItem.ToString() == "Lap rincian perbandingan ke-3 LPP vs file SONAS")
                        {
                        }

                        break;
                    }

                case 8:
                    {
                        if (ComboBox1.SelectedItem.ToString() == "Register SP setelah SONAS")
                        {
                        }
                        else if (ComboBox1.SelectedItem.ToString() == "Cetakan SP setelah SONAS")
                        {
                        }
                        else if (ComboBox1.SelectedItem.ToString() == "Register BPB setelah SONAS")
                        {
                        }
                        else if (ComboBox1.SelectedItem.ToString() == "Cetakan BPB setelah SONAS")
                        {
                        }

                        break;
                    }
            }
        }
    }
}
