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
using MySql.Data.MySqlClient;

namespace Cadastro_Cliente
{
    public partial class FMenuClientes : Form
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

                if (File.Exists(pastaFotos + lin.Cells["id"].Value.ToString() + ".png"))
                {
                    lin.Cells["foto"].Value = Image.FromFile(pastaFotos + lin.Cells["id"].Value.ToString() + ".png");
                }
                else
                {
                    lin.Cells["foto"].Value = Properties.Resources.Monkey;
                }
            }

            dtgListaCliente.ClearSelection();
            btnAlterarCliente.Enabled = false;
        }

        private void dtgListaCliente_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            btnAlterarCliente.Enabled = true;
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
            ReoganizarGrid();

            Rodape();
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
    }
}
