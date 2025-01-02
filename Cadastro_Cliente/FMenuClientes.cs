using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using MySql.Data.MySqlClient;

namespace Cadastro_Cliente
{
    public partial class FMenuClientes : CustomForm
    {
        public FMenuClientes()
        {
            InitializeComponent();
        }

        string pastaFotos = AppDomain.CurrentDomain.BaseDirectory + "//fotos//";

        private void btnNovoCliente_Click(object sender, EventArgs e)
        {
            FCadClient fCadClient = new FCadClient();
            fCadClient.ShowDialog();
        }

        private void FMenuClientes_Load(object sender, EventArgs e)
        {
            BuscarClientes();
        }

        private void dtgListaCliente_Sorted(object sender, EventArgs e)
        {
            ReoganizarGrid();
        }



        private void ReoganizarGrid()
        {
            foreach (DataGridViewRow lin in dtgListaCliente.Rows)
            {
                if (lin.Cells["situacao"].Value.ToString() == "Inativo")
                {
                    lin.DefaultCellStyle.ForeColor = Color.Crimson;
                }

                //if (File.Exists(pastaFotos + lin.Cells["id"].Value.ToString() + ".png"))
                //{
                //    lin.Cells["foto"].Value = Image.FromFile(pastaFotos + lin.Cells["id"].Value.ToString() + ".png");
                //}
                //else
                //{
                //    lin.Cells["foto"].Value = Properties.Resources.Monkey;
                //}
            }

            dtgListaCliente.ClearSelection();
            btnAlterarCliente.Enabled = false;
            btnFichaCliente.Enabled = false;
        }

        private void dtgListaCliente_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            btnAlterarCliente.Enabled = true;
            btnFichaCliente.Enabled = true;
        }

        private void btnAlterarCliente_Click(object sender, EventArgs e)
        {
            FCadClient fCadClient = new FCadClient();
            fCadClient.txtID.Text = dtgListaCliente.CurrentRow.Cells["id"].Value.ToString();
            fCadClient.ShowDialog();

            BuscarClientes();
        }

        private void BuscarClientes()
        {
            dtgListaCliente.DataSource = funcoes.BuscaSQL("SELECT * FROM tclientes WHERE 1 " + GerarCriterios());
            if ((dtgListaCliente.RowCount * 30) + 50 > 514)
                dtgListaCliente.Height = 514;
            else
                dtgListaCliente.Height = (dtgListaCliente.RowCount * 30) + 50;

            ReoganizarGrid();

            Rodape();

            if (dtgListaCliente.RowCount == 0)
                lblAvisoEncontrado.Visible = true;
            else
                lblAvisoEncontrado.Visible = false;
        }


        private string GerarCriterios()
        {
            string c = "";
            if (pesqCod.Text != string.Empty)
            {
                c += " AND id = " + pesqCod.Text;
            }

            if (pesqNome.Text != string.Empty)
            {
                c += $" AND (nome LIKE '%{pesqNome.Text}%' OR documento LIKE '%{pesqNome.Text}%')";
            }

            if (pesqGenero.Text != string.Empty)
            {
                c += $" AND genero = '{pesqGenero.Text}'";
            }

            if (pesqEstadoCivil.Text != string.Empty)
            {
                c += $" AND estado_civil = '{pesqEstadoCivil.Text}'";
            }

            if (pesqEndereco.Text != string.Empty)
            {
                string e = pesqEndereco.Text;
                c += $" AND (cep LIKE '%{e}%' OR cidade LIKE '%{e}%' OR endereco LIKE '%{e}%' OR bairro LIKE '%{e}%'  OR estado LIKE '%{e}%'  OR numero LIKE '%{e}%')";
            }

            try
            {
                DateTime data = Convert.ToDateTime(pesqNasc.Text);
                c += $" AND data_nasc = '{data:yyyy-MM-dd}'";
            }
            catch (Exception)
            {
            }

            if (pesqAtivo.Checked == true)
            {
                c += " AND situacao = 'Ativo'";
            }
            else if (pesqInativo.Checked == true)
            {
                c += " AND situacao = 'Inativo'";
            }

            return c;
        }

        private void pesqNome_TextChanged(object sender, EventArgs e)
        {
            BuscarClientes();
        }

        private void pesqCod_TextChanged(object sender, EventArgs e)
        {
            BuscarClientes();
        }

        private void pescTodos_CheckedChanged(object sender, EventArgs e)
        {
            if (pescTodos.Checked == true)
            {
                BuscarClientes();
            }
        }

        private void pesqAtivo_CheckedChanged(object sender, EventArgs e)
        {
            if (pesqAtivo.Checked == true)
            {
                BuscarClientes();
            }
        }

        private void pesqInativo_CheckedChanged(object sender, EventArgs e)
        {
            if (pesqInativo.Checked == true)
            {
                BuscarClientes();
            }
        }

        private void Rodape()
        {
            lblTotalLocalizado.Text = "Total localizado : " + dtgListaCliente.RowCount;

            int contador = 0;

            foreach (DataGridViewRow lin in dtgListaCliente.Rows)
            {
                if (lin.Cells["situacao"].Value.ToString() == "Inativo")
                {
                    contador++;
                }
            }

            lblTotalInativo.Text = "Total Inativos : " + contador.ToString();
            lblTotalAtivo.Text = "Total Ativos " + (dtgListaCliente.RowCount - contador).ToString();
        }

        private void dtgListaCliente_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;

            DataGridViewRow lin = dtgListaCliente.Rows[e.RowIndex];

            lin.DefaultCellStyle.BackColor = Color.SkyBlue;

            if (lin.Cells["FotoBD"].Value == DBNull.Value)
                lin.Cells["foto"].Value = Properties.Resources.Monkey;
            else
                lin.Cells["foto"].Value = lin.Cells["FotoBD"].Value;

        }

        private void dtgListaCliente_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;

            dtgListaCliente.Rows[e.RowIndex].DefaultCellStyle.BackColor = e.RowIndex % 2 == 0 ? Color.White : Color.AliceBlue;
            dtgListaCliente.Rows[e.RowIndex].Cells["foto"].Value = null;

        }

        private void btnRelatorioCliente_Click(object sender, EventArgs e)
        {
  
            DataTable dt = funcoes.BuscaSQL("SELECT * FROM tclientes");

            DS.DadosClienteDataTable dtgCli = new DS.DadosClienteDataTable();
            dtgCli.Merge(dt);

       
            ReportDataSource reportDataSource = new ReportDataSource("DataSet1", dtgCli as DataTable);
            RelatorioClientes.LocalReport.DataSources.Clear();
            RelatorioClientes.LocalReport.DataSources.Add(reportDataSource);

            funcoes.ImprimirPDF(RelatorioClientes, "RelatorioCliente");
        }

        private void btnFichaCliente_Click(object sender, EventArgs e)
        {
            string id = dtgListaCliente.CurrentRow.Cells["id"].Value.ToString();
            DataTable dt = funcoes.BuscaSQL("SELECT * FROM tclientes WHERE id = " + id);

            DS.DadosClienteDataTable dtgCli = new DS.DadosClienteDataTable();
            dtgCli.Merge(dt);

            if (dtgCli.Rows[0]["foto"] != DBNull.Value)
                dtgCli.Rows[0]["foto"] = dt.Rows[0]["foto"];
            else
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    Properties.Resources.Monkey.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    dtgCli.Rows[0]["foto"] = ms.ToArray();
                }
            }

            ReportDataSource reportDataSource = new ReportDataSource("DataSet1" , dtgCli as DataTable);
            reportFicha.LocalReport.DataSources.Clear();
            reportFicha.LocalReport.DataSources.Add(reportDataSource);

            funcoes.ImprimirPDF(reportFicha, "FichaCadastral");
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Ativar KeyPreview para capturar teclas pressionadas
            KeyPreview = true;

            // Associar evento KeyDown
            KeyDown += FMenuClientes_KeyDown;
        }

        private void FMenuClientes_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Handled) return;
            switch (e.KeyCode)
            {
                case Keys.F4:
                    e.Handled = true;
                    btnNovoCliente.PerformClick();
                    break;
                case Keys.F5:
                    e.Handled = true;
                    btnAlterarCliente.PerformClick();
                    break;
                case Keys.F6:
                    e.Handled = true;
                    btnRelatorioCliente.PerformClick();
                    break;
                case Keys.F7:
                    e.Handled = true;
                    btnFichaCliente.PerformClick();
                    break;
                case Keys.Escape:
                    e.Handled = true;
                    Close();
                    break;
            }
        }
    }
}







