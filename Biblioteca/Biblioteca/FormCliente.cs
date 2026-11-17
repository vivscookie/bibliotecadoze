using System;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace Biblioteca
{
    public class FormCliente : Form
    {
        TextBox txtCpf, txtNome, txtEndereco, txtTelefone, txtEmail;
        DataGridView grid;

        public FormCliente()
        {
            Text = "Cadastrar cliente";
            ClientSize = new System.Drawing.Size(700, 500);
            StartPosition = FormStartPosition.CenterParent;

            txtCpf = Ui.T(90, 15, 150);
            txtCpf.MaxLength = 14; // aceita com máscara (000.000.000-00); só os números são salvos
            txtNome = Ui.T(90, 45, 300);
            txtEndereco = Ui.T(90, 75, 300);
            txtTelefone = Ui.T(90, 105, 150);
            txtEmail = Ui.T(90, 135, 300);
            grid = Ui.Grid(10, 175, 680, 315);

            Controls.AddRange(new Control[] {
                Ui.L("CPF:", 10, 15), txtCpf,
                Ui.L("Nome:", 10, 45), txtNome,
                Ui.L("Endereço:", 10, 75), txtEndereco,
                Ui.L("Telefone:", 10, 105), txtTelefone,
                Ui.L("E-mail:", 10, 135), txtEmail,
                Ui.B("Salvar", 420, 14, 100, Salvar),
                Ui.B("Excluir", 420, 44, 100, Excluir),
                grid });

            Carregar();
        }

        void Carregar()
        {
            try
            {
                grid.DataSource = Db.Consultar(
                    "SELECT Cpf, Nome, Endereco, Telefone, Email FROM Clientes ORDER BY Nome");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao acessar o banco: " + ex.Message);
            }
        }

        void Salvar(object sender, EventArgs e)
        {
            // guarda só os dígitos do CPF
            string cpf = new string(txtCpf.Text.Where(char.IsDigit).ToArray());

            if (cpf.Length != 11)
            {
                MessageBox.Show("O CPF deve ter 11 números.");
                return;
            }
            if (txtNome.Text.Trim() == "")
            {
                MessageBox.Show("Informe o nome.");
                return;
            }

            try
            {
                Db.Executar(
                    "INSERT INTO Clientes (Cpf, Nome, Endereco, Telefone, Email) VALUES (@c, @n, @e, @t, @m)",
                    new SqlParameter("@c", cpf),
                    new SqlParameter("@n", txtNome.Text.Trim()),
                    new SqlParameter("@e", txtEndereco.Text.Trim()),
                    new SqlParameter("@t", txtTelefone.Text.Trim()),
                    new SqlParameter("@m", txtEmail.Text.Trim()));

                txtCpf.Clear();
                txtNome.Clear();
                txtEndereco.Clear();
                txtTelefone.Clear();
                txtEmail.Clear();
                Carregar();
            }
            catch (SqlException ex) when (ex.Number == 2627)
            {
                MessageBox.Show("Já existe um cliente com este CPF.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar: " + ex.Message);
            }
        }

        void Excluir(object sender, EventArgs e)
        {
            if (grid.CurrentRow == null)
            {
                MessageBox.Show("Selecione um cliente na lista.");
                return;
            }

            string cpf = grid.CurrentRow.Cells["Cpf"].Value.ToString();

            if (MessageBox.Show("Excluir o cliente selecionado?", "Confirmação",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                Db.Executar("DELETE FROM Clientes WHERE Cpf = @c", new SqlParameter("@c", cpf));
                Carregar();
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                MessageBox.Show("Não é possível excluir: o cliente possui empréstimos registrados.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao excluir: " + ex.Message);
            }
        }
    }
}