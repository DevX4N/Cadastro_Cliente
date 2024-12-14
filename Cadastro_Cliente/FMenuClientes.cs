using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
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

        private void btnNovoCliente_Click(object sender, EventArgs e)
        {
            FCadClient fCadClient = new FCadClient();
            fCadClient.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string caminhoArquivo = @"C:\Users\Asus\Downloads\Clientes.json";

            string conteudoJson = System.IO.File.ReadAllText(caminhoArquivo);

            dynamic objetoJson = Newtonsoft.Json.JsonConvert.DeserializeObject(conteudoJson);


            string comandoSql = "INSERT INTO tclientes (nome, documento, genero, rg, estado_civil, data_nasc, cep, endereco, numero, bairro, cidade, estado, celular, email, obs, situacao) " +
                            "VALUES (@nome, @documento, @genero, @rg, @estado_civil, @data_nasc, @cep, @endereco, @numero, @bairro, @cidade, @estado, @celular, @email, @obs, @situacao)";

            using (MySqlConnection Conexao = new MySqlConnection(@"Server=127.0.0.1; Port=3306; Database=base; User=root; Password=;"))
            {
                Conexao.Open();

                foreach (var item in objetoJson)
                {
                    using (MySqlCommand cmd = Conexao.CreateCommand())
                    {
                        cmd.CommandText = comandoSql;

                        cmd.Parameters.AddWithValue("@nome", "teste");
                        cmd.Parameters.AddWithValue("@documento", "documento");
                        cmd.Parameters.AddWithValue("@genero", "Masculino");
                        cmd.Parameters.AddWithValue("@rg", "1");
                        cmd.Parameters.AddWithValue("@estado_civil", "Solteiro");
                        cmd.Parameters.AddWithValue("@data_nasc", "01/01/2024");
                        cmd.Parameters.AddWithValue("@cep", "89707091");
                        cmd.Parameters.AddWithValue("@endereco", "rua");
                        cmd.Parameters.AddWithValue("@bairro", "bairro");
                        cmd.Parameters.AddWithValue("@cidade", "cidade");
                        cmd.Parameters.AddWithValue("@estado", "estado");
                        cmd.Parameters.AddWithValue("@celular", "celular");
                        cmd.Parameters.AddWithValue("@email", "email");
                        cmd.Parameters.AddWithValue("@obs", "obs");
                        cmd.Parameters.AddWithValue("@situacao", "situacao");
                        cmd.Parameters.AddWithValue("@numero", "numero");

                        cmd.ExecuteNonQuery();

                    }
                }
            }
        }
    }
}
