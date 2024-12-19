using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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

    }
}
