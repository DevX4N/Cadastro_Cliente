using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using MySql.Data.MySqlClient;
using System.Runtime.InteropServices.WindowsRuntime;


namespace Cadastro_Cliente
{
    public partial class FCadClient : Form
    {
        public FCadClient()
        {
            InitializeComponent();
        }

        private void FCadClient_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendKeys.Send("{TAB}");
                e.SuppressKeyPress = true;
            }
        }

        private void SalvarCliente()
        {
            using (MySqlConnection con = new MySqlConnection("Server=127.0.0.1; Port=3306; Database=base; User=root; Password=;"))
            {
                con.Open();
                using (MySqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO tclientes (nome, documento, genero, rg, estado_civil, data_nasc, cep, endereco, numero, bairro, cidade, estado, celular, email, obs, situacao) " +
                        " VALUES (@nome, @documento, @genero, @rg, @estado_civil, @data_nasc, @cep, @endereco, @numero, @bairro, @cidade, @estado, @celular, @email, @obs, @situacao)";

                    cmd.Parameters.AddWithValue("@nome", txtNomeCliente.Text);
                    cmd.Parameters.AddWithValue("@documento", txtCnpjCpf.Text);

                    if (rdoMasculino.Checked == true)
                        cmd.Parameters.AddWithValue("@genero", "Masculino");
                    else if (rdoFeminino.Checked == true)
                        cmd.Parameters.AddWithValue("@genero", "Feminino");
                    else if (rdoOutros.Checked == true)
                        cmd.Parameters.AddWithValue("@genero", "Outros");

                    cmd.Parameters.AddWithValue("@rg", txtrg.Text);
                    cmd.Parameters.AddWithValue("@estado_civil", cmbEstadoCivil.Text);

                    if(txtDataNasc.Text == "  /  /")
                         cmd.Parameters.AddWithValue("@data_nasc", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@data_nasc", Convert.ToDateTime(txtDataNasc.Text));

                    cmd.Parameters.AddWithValue("@cep", txtCep.Text);
                    cmd.Parameters.AddWithValue("@endereco", cmbEndereco.Text);
                    cmd.Parameters.AddWithValue("@numero", txtNumero.Text);
                    cmd.Parameters.AddWithValue("@bairro", cmbBairro.Text);
                    cmd.Parameters.AddWithValue("@cidade", cmbCidade.Text);
                    cmd.Parameters.AddWithValue("@estado", cmbEstado.Text);
                    cmd.Parameters.AddWithValue("@celular", txtCelular.Text);
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@obs", txtObs.Text);
                    cmd.Parameters.AddWithValue("@situacao", (chkSituacao.Checked == true ? "Ativo" : "Inativo"));
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "SELECT @@IDENTITY";
                    txtID.Text = cmd.ExecuteScalar().ToString();
                }
                MessageBox.Show("Tudo certo!");
            }
        }

        private bool Validacoes()
        {
            if(txtNomeCliente.Text == "")
            {
                MessageBox.Show("Campo Nome é obrigatório", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtNomeCliente.Focus();
                return true;
            }

            if(rdoCPF.Checked == false && rdoCNPJ.Checked == false)
            {
                MessageBox.Show("Marque um tipo de Documentação \r CPF ou CNPJ", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }

            if (txtCnpjCpf.Text == "")
            {
                if(rdoCPF.Checked == true)
                     MessageBox.Show("Digite o seu CPF!", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("Digite o seu CNPJ!", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Information);

                txtCnpjCpf.Focus();
                return true;
            }

            if(rdoMasculino.Checked == false && rdoFeminino.Checked == false && rdoOutros.Checked == false)
            {
                MessageBox.Show("Marque um Gênero", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }

            if (txtDataNasc.Text != "  /  /")
            {
                try
                {
                    Convert.ToDateTime(txtDataNasc.Text);
                }
                catch (Exception)
                {
                    MessageBox.Show("Data de Nascimento inválida", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtDataNasc.Focus();
                    return true;
                }
            }

            return false;
        }


        private void btnSalvar_Click(object sender, EventArgs e)
        {
            if (Validacoes() == true)
                return;

            SalvarCliente();
        }

        private void btnNovo_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Deseja limpar todos os campos?", "Atenção", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No)
                return;

            txtID.Text = "";
            txtNomeCliente.Text = "";
            rdoCNPJ.Checked = false;
            rdoCPF.Checked = false;
            txtCnpjCpf.Text = "";
            rdoMasculino.Checked = false;  
            rdoFeminino.Checked = false;
            rdoOutros.Checked = false;
            txtrg.Text = "";
            cmbEstadoCivil.SelectedIndex = -1;
            txtDataNasc.Text = "";
            txtCep.Text = "";
            cmbEndereco.SelectedIndex = -1;
            txtNumero.Text = "";
            cmbBairro.SelectedIndex = -1;
            cmbCidade.SelectedIndex = -1;
            cmbEstado.SelectedIndex = -1;
            txtCelular.Text = "";
            txtEmail.Text = "";
            txtObs.Text = "";
        }

        private void btnFechar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
