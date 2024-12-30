using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using MySql.Data.MySqlClient;

namespace Cadastro_Cliente
{
     class funcoes
    {
        public static void msgErro(string Msg)
        {
            MessageBox.Show(Msg, "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static bool msgPergunta(string MsgPergunta)
        {
            
           if( MessageBox.Show(MsgPergunta, "Atenção", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                return true;
           else
                return false;
        }

        public static void PriMaiuscula(Control ctl)
        {
            TextInfo textInfo = new CultureInfo("pt-BR", false).TextInfo;

            string t = ctl.Text;

            t = textInfo.ToTitleCase(t);

            t = t.Replace(" Das ", " das ")
                .Replace(" Da ", " da ")
                .Replace(" Dos ", " dos ")
                .Replace(" Do ", " do ")
                .Replace(" De ", " de ");

           ctl.Text = t;

            if (ctl is TextBox txt)
            {
                txt.SelectionStart = txt.Text.Length;
            } 
            else if (ctl is ComboBox cmb)
            {
                cmb.SelectionStart = cmb.Text.Length;
            }
        }

        public static DataTable BuscaSQL(string ComandoSql)
        {
            DataTable dt = new DataTable();

            using (MySqlConnection con = new MySqlConnection(@"Server=127.0.0.1; Port=3306; Database=base; User=root; Password=;"))
            {
                con.Open();
                using (MySqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = ComandoSql;

                    using (MySqlDataAdapter mySql = new MySqlDataAdapter(cmd))
                    {
                        mySql.Fill(dt);
                    }
                }
            }

            return dt;
        }

        public static void CarregarComboBox (ComboBox cmb, string tabela, string campo)
        {
            cmb.DataSource = funcoes.BuscaSQL($"SELECT DISTINCT {campo} FROM {tabela} WHERE {campo} <> ''");
            cmb.DisplayMember = campo;
            cmb.SelectedIndex = -1;
        }

        public static void SalvarImagemPequena(string ArquivoOriginal, string NovaFoto, int Largura, int Altura, bool onlyResizeIfWider)
        {
            Image TamanhoImagem = Image.FromFile(ArquivoOriginal);

            TamanhoImagem.RotateFlip(RotateFlipType.Rotate180FlipNone);
            TamanhoImagem.RotateFlip(RotateFlipType.Rotate180FlipNone);

            if (onlyResizeIfWider)
            {
                if (TamanhoImagem.Width <= Largura)
                {
                    Largura = TamanhoImagem.Width;
                }
            }

            int newHeight = TamanhoImagem.Height * Largura / TamanhoImagem.Width;

            if (newHeight > Altura)
            {
                Largura = TamanhoImagem.Width * Altura / TamanhoImagem.Height;
                newHeight = Altura;
            }

            Image NovaImagem = TamanhoImagem.GetThumbnailImage(Largura, newHeight, null, IntPtr.Zero);

            TamanhoImagem.Dispose();

            NovaImagem.Save(NovaFoto);
        }

        public static void ImprimirPDF(ReportViewer report, string nomeArquivo, ReportParameterCollection p = null)
        {
            if (p != null)
                report.LocalReport.SetParameters(p);

            report.Refresh();
            report.RefreshReport();

            try
            {
                Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string filenameExtension;

                byte[] bytes = report.LocalReport.Render(
                "PDF", null, out mimeType, out encoding, out filenameExtension,
                out streamids, out warnings);
                using (FileStream fs = new FileStream(nomeArquivo + ".pdf", FileMode.Create))
                {
                    fs.Write(bytes, 0, bytes.Length);
                }

                System.Diagnostics.Process.Start(nomeArquivo + ".pdf");
            }
            catch (Exception)
            {
                funcoes.msgErro("Cadastro Clientes");
            }
        }
    }
}
