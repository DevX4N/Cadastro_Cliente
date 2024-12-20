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
using System.Globalization;
using System.IO;
using System.Collections;
using System.Net.Http;
using Newtonsoft.Json;


namespace Cadastro_Cliente
{

    public partial class FCadClient : Form
    {
        public FCadClient()
        {
            InitializeComponent();
        }

        string strConexao = "Server=127.0.0.1; Port=3306; Database=base; User=root; Password=;";
        string pastaFotos = AppDomain.CurrentDomain.BaseDirectory + "//fotos//";

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
            using (MySqlConnection con = new MySqlConnection(strConexao))
            {
                con.Open();
                using (MySqlCommand cmd = con.CreateCommand())
                {
                    if (txtID.Text == "")
                    {
                        cmd.CommandText = "INSERT INTO tclientes (nome, documento, genero, rg, estado_civil, data_nasc, cep, endereco, numero, bairro, cidade, estado, celular, email, obs, situacao) " +
                            "VALUES (@nome, @documento, @genero, @rg, @estado_civil, @data_nasc, @cep, @endereco, @numero, @bairro, @cidade, @estado, @celular, @email, @obs, @situacao)";
                    }
                    else
                    {
                        cmd.CommandText = "UPDATE tclientes SET nome = @nome, documento = @documento, genero = @genero, rg = @rg, estado_civil = @estado_civil, data_nasc = @data_nasc, cep = @cep, " +
                            "endereco = @endereco, numero = @numero, bairro = @bairro, cidade = @cidade, estado = @estado, celular = @celular, email = @email, obs = @obs, situacao = @situacao WHERE id = @id";
                        cmd.Parameters.AddWithValue("@id", txtID.Text); 
                    }


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

                    if (txtDataNasc.Text == "  /  /")
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

                    Console.WriteLine("Query: " + cmd.CommandText);
                    foreach (MySqlParameter param in cmd.Parameters)
                    {
                        Console.WriteLine($"Parameter: {param.ParameterName}, Value: {param.Value}");
                    }

                    cmd.ExecuteNonQuery();



                    if (txtID.Text == "")
                    {
                        cmd.CommandText = "SELECT @@IDENTITY";
                        txtID.Text = cmd.ExecuteScalar().ToString();
                    }

                  
                }
                MessageBox.Show("Tudo certo!");
            }
        }

        private bool Validacoes()
        {
            if (txtNomeCliente.Text == "")
            {
                funcoes.msgErro("Campo Nome é obrigatório");
                txtNomeCliente.Focus();
                return true;
            }

            if (rdoCPF.Checked == false && rdoCNPJ.Checked == false)
            {
                funcoes.msgErro("Marque um tipo de Documentação \r CPF ou CNPJ");
                return true;
            }

            if (txtCnpjCpf.Text == "")
            {
                if (rdoCPF.Checked == true)
                    funcoes.msgErro("Digite o seu CPF!");
                else
                    funcoes.msgErro("Digite o seu CNPJ!");

                txtCnpjCpf.Focus();
                return true;
            }

            if (rdoMasculino.Checked == false && rdoFeminino.Checked == false && rdoOutros.Checked == false)
            {
                funcoes.msgErro("Marque um Gênero");
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
                    funcoes.msgErro("Data de Nascimento inválida");
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
            if (funcoes.msgPergunta("Deseja limpar todos os campos?") == false)
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
            cmbEndereco.SelectedIndex = 0;
            txtNumero.Text = "";
            cmbBairro.SelectedIndex = 0;
            cmbCidade.SelectedIndex = 0;
            cmbEstado.SelectedIndex = -1;
            txtCelular.Text = "";
            txtEmail.Text = "";
            txtObs.Text = "";


            btnSalvar.Text = "Salvar";
            pctImgCliente.Image = Properties.Resources.Monkey;
        }

        private void btnFechar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void rdoCPF_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoCPF.Checked == true)
            {
                txtCnpjCpf.Mask = "000,000,000-00";
                txtCnpjCpf.Focus();
            }
        }

        private void rdoCNPJ_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoCNPJ.Checked == true)
            {
                txtCnpjCpf.Mask = "00,000,000/0000-00";
                txtCnpjCpf.Focus();
            }
        }

        private void rdoMasculino_CheckedChanged(object sender, EventArgs e)
        {
            txtrg.Focus();
        }

        private void rdoFeminino_CheckedChanged(object sender, EventArgs e)
        {
            txtrg.Focus();
        }

        private void rdoOutros_CheckedChanged(object sender, EventArgs e)
        {
            txtrg.Focus();
        }

        private void txtDataNasc_Validating(object sender, CancelEventArgs e)
        {
            if (txtDataNasc.Text == "  /  /")
                return;
            try
            {
                txtDataNasc.Text = Convert.ToDateTime(txtDataNasc.Text).ToString();
            }
            catch (Exception)
            {
                funcoes.msgErro("Data de Nascimento inválida");
                e.Cancel = true;
            }
        }

        private void cmbEstadoCivil_Validating(object sender, CancelEventArgs e)
        {
            if (cmbEstadoCivil.Text == "")
            {
                return;
            }

            if (cmbEstadoCivil.SelectedIndex == -1)
            {
                funcoes.msgErro("Selecione um item da lista");
            }
        }

        private void cmbEstado_Validating(object sender, CancelEventArgs e)
        {
            if (cmbEstado.Text == "")
            {
                return;
            }

            if (cmbEstado.SelectedIndex == -1)
            {
                funcoes.msgErro("Selecione um item da lista");
            }
        }

        private void txtNomeCliente_TextChanged(object sender, EventArgs e)
        {
            funcoes.PriMaiuscula(txtNomeCliente);
        }

        private void txtCep_Validating(object sender, CancelEventArgs e)
        {
            if (txtCep.Text.Length == 0)
                return;
            if (txtCep.Text.Replace(" ", "").Length < 8)
            {
                funcoes.msgErro("Informe um CEP válido");
                e.Cancel = true;
            }

            lblAviso.Visible = true;
            Application.DoEvents();

            cmbEndereco.Text = string.Empty;
            cmbBairro.Text = string.Empty;
            cmbCidade.Text = string.Empty;
            cmbEstado.Text = string.Empty;

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage resposta = client.GetAsync($"https://viacep.com.br/ws/{txtCep.Text}/json/").Result;

                    dynamic json = JsonConvert.DeserializeObject(resposta.Content.ReadAsStringAsync().Result);

                    if (json.erro == null)
                    {
                        cmbEndereco.Text = json.logradouro.ToString();
                        cmbBairro.Text = json.bairro.ToString();
                        cmbCidade.Text = json.localidade.ToString();
                        cmbEstado.Text = json.uf.ToString();

                        foreach (var item in cmbEstado.Items)
                        {
                            if (item.ToString().Contains($"({ cmbEstado.Text})"))
                            {
                                cmbEstado.Text = item.ToString();
                                break;
                            }
                        }
                    }
                    else
                    {
                        txtCep.Focus();
                        funcoes.msgErro("CEP Não Localizado...");
                    }
                }
                catch (Exception)
                {
                    funcoes.msgErro("Não foi possivel realizar a consulta \rVerifique sua conexão com a internet");
                }

              
            }

            lblAviso.Visible = false;
        }

        private void txtCnpjCpf_Validating(object sender, CancelEventArgs e)
        {
            if (txtCnpjCpf.Text == "")
                return;

            if (rdoCPF.Checked == true && txtCnpjCpf.Text.Replace(" ", "").Length < 11)
            {
                funcoes.msgErro("Informe um CPF válido");
                e.Cancel = true;
            }
            if (rdoCNPJ.Checked == true && txtCnpjCpf.Text.Replace(" ", "").Length < 14)
            {
                funcoes.msgErro("Informe um CNPJ válido");
                e.Cancel = true;
            }
        }

        private void txtCelular_Validating(object sender, CancelEventArgs e)
        {
            if (txtCelular.Text == "")
                return;

            if (txtCelular.Text.Replace(" ", "").Length < 11)
            {
                funcoes.msgErro("Informe um Número válido");
                e.Cancel = true;
            }
        }

        byte[] imgBytes;

        private void btnAddFoto_Click(object sender, EventArgs e)
        {
            if (txtID.Text == "")
            {
                funcoes.msgErro("Salve os dados do cliente primeiro");
                txtNomeCliente.Focus();
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Arquivo de imagem(*.bmp;*.jpg;*.gif;*.jpeg;*.png)|*.bmp;*.jpg;*.gif;*.jpeg;*.png";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                
                //File.Copy(openFileDialog.FileName, pastaFotos + "Cliente" + txtID.Text + ".png");

                funcoes.SalvarImagemPequena(openFileDialog.FileName, AppDomain.CurrentDomain.BaseDirectory + "/FotoTemp.png", pctImgCliente.Width, pctImgCliente.Height, false);

                pctImgCliente.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "/FotoTemp.png");
                imgBytes = File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "/FotoTemp.png");

                using (MySqlConnection con = new MySqlConnection(strConexao))
                {
                    con.Open();
                    using (MySqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "UPDATE tclientes set foto = @foto WHERE id = @id";
                        cmd.Parameters.AddWithValue("@foto", imgBytes);
                        cmd.Parameters.AddWithValue("id", txtID.Text);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private void btnRemoveFoto_Click(object sender, EventArgs e)
        {
            if (txtID.Text == "")
            {
                funcoes.msgErro("Não há foto para ser removida");
                return;
            }

            if (File.Exists(pastaFotos + "Cliente" + txtID.Text + ".png") == false)
            {
                funcoes.msgErro("Não há foto para remover");
                return;
            }

            if (funcoes.msgPergunta("Deseja realmente remover a foto?") == false)
                return;
            pctImgCliente.Image = Properties.Resources.Monkey;

            File.Delete(pastaFotos + txtID.Text + ".png");
        }

        private void FCadClient_Load(object sender, EventArgs e)
        {
            funcoes.CarregarComboBox(cmbEndereco, "tclientes", "endereco");
            funcoes.CarregarComboBox(cmbCidade, "tclientes", "cidade");
            funcoes.CarregarComboBox(cmbBairro, "tclientes", "bairro");

            if (txtID.Text == "")
                return;

            btnSalvar.Text = "Atualizar";
            DataTable dt = funcoes.BuscaSQL("SELECT * FROM tclientes WHERE id =" + txtID.Text);
            txtNomeCliente.Text = dt.Rows[0]["nome"].ToString();
            txtNumero.Text = dt.Rows[0]["numero"].ToString();
            txtrg.Text = dt.Rows[0]["rg"].ToString();
            txtCep.Text = dt.Rows[0]["cep"].ToString();
            cmbBairro.Text = dt.Rows[0]["bairro"].ToString();
            cmbCidade.Text = dt.Rows[0]["cidade"].ToString();
            txtEmail.Text = dt.Rows[0]["email"].ToString();
            txtObs.Text = dt.Rows[0]["obs"].ToString();
            cmbEstado.Text = dt.Rows[0]["estado"].ToString();
            cmbEstadoCivil.Text = dt.Rows[0]["estado_civil"].ToString();
            cmbEndereco.Text = dt.Rows[0]["endereco"].ToString();
            txtCelular.Text = dt.Rows[0]["celular"].ToString();
            txtDataNasc.Text = dt.Rows[0]["data_nasc"].ToString();

            if (dt.Rows[0]["documento"].ToString().Length == 11)
            {
                rdoCPF.Checked = true;
            }
            else
            {
                rdoCNPJ.Checked = true;
            }

            txtCnpjCpf.Text = dt.Rows[0]["documento"].ToString();

            if (dt.Rows[0]["genero"].ToString() == "Masculino")
            {
                rdoMasculino.Checked = true;
            }
            else if (dt.Rows[0]["genero"].ToString() == "Feminino")
            {
                rdoFeminino.Checked = true;
            }
            else
            {
                rdoOutros.Checked = true;
            }

            if (dt.Rows[0]["situacao"].ToString() == "Ativo")
            {
                chkSituacao.Checked = true;
            }
            else
            {
                chkSituacao.Checked = false;
            }

            if (File.Exists(pastaFotos + "Cliente" + txtID.Text + ".png"))
            {
                pctImgCliente.LoadAsync(pastaFotos + "Cliente" + txtID.Text + ".png");
            }
            else
            {
                pctImgCliente.Image = Properties.Resources.Monkey;
            }
        }
    }
}
