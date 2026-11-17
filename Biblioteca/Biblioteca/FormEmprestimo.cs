using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Biblioteca
{
    public class FormEmprestimo : Form
    {
        ComboBox cmbLivro, cmbCliente;
        DataGridView grid;

        public FormEmprestimo()
        {
            Text = "Realizar empréstimo";
            ClientSize = new System.Drawing.Size(700, 480);
            StartPosition = FormStartPosition.CenterParent;

            cmbLivro = new ComboBox { Left = 80, Top = 15, Width = 320, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbCliente = new ComboBox { Left = 80, Top = 45, Width = 320, DropDownStyle = ComboBoxStyle.DropDownList };
            grid = Ui.Grid(10, 95, 680, 375);

            Controls.AddRange(new Control[] {
                Ui.L("Livro:", 10, 15), cmbLivro,
                Ui.L("Cliente:", 10, 45), cmbCliente,
                Ui.B("Emprestar", 430, 14, 100, Emprestar),
                Ui.B("Devolver", 430, 44, 100, Devolver),
                Ui.L("Para devolver, selecione o empréstimo na lista e clique em Devolver.", 10, 70),
                grid });

            Carregar();
        }

        void Carregar()
        {
            try
            {
                cmbLivro.DisplayMember = "Titulo";
                cmbLivro.ValueMember = "Id";
                cmbLivro.DataSource = Db.Consultar("SELECT Id, Titulo FROM Livros ORDER BY Titulo");

                cmbCliente.DisplayMember = "Descricao";
                cmbCliente.ValueMember = "Cpf";
                cmbCliente.DataSource = Db.Consultar(
                    "SELECT Cpf, Nome + ' - ' + Cpf AS Descricao FROM Clientes ORDER BY Nome");

                grid.DataSource = Db.Consultar(
                    @"SELECT e.Id, l.Titulo AS Livro, c.Nome AS Cliente,
                             e.DataEmprestimo, e.DataDevolucao
                      FROM Emprestimos e
                      JOIN Livros l ON l.Id = e.LivroId
                      JOIN Clientes c ON c.Cpf = e.ClienteCpf
                      ORDER BY e.DataEmprestimo DESC");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao acessar o banco: " + ex.Message);
            }
        }

        void Emprestar(object sender, EventArgs e)
        {
            if (cmbLivro.SelectedValue == null || cmbCliente.SelectedValue == null)
            {
                MessageBox.Show("Selecione um livro e um cliente.");
                return;
            }

            try
            {
                int livroId = Convert.ToInt32(cmbLivro.SelectedValue);
                string cpf = cmbCliente.SelectedValue.ToString();

                int disponiveis = Convert.ToInt32(Db.Escalar(
                    @"SELECT l.Quantidade - (SELECT COUNT(*) FROM Emprestimos
                                             WHERE LivroId = l.Id AND DataDevolucao IS NULL)
                      FROM Livros l WHERE l.Id = @id",
                    new SqlParameter("@id", livroId)));

                if (disponiveis <= 0)
                {
                    MessageBox.Show("Não há exemplares disponíveis deste livro.");
                    return;
                }

                Db.Executar("INSERT INTO Emprestimos (LivroId, ClienteCpf) VALUES (@l, @c)",
                    new SqlParameter("@l", livroId),
                    new SqlParameter("@c", cpf));

                MessageBox.Show("Empréstimo realizado.");
                Carregar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao emprestar: " + ex.Message);
            }
        }

        void Devolver(object sender, EventArgs e)
        {
            if (grid.CurrentRow == null)
            {
                MessageBox.Show("Selecione um empréstimo na lista.");
                return;
            }

            try
            {
                int id = Convert.ToInt32(grid.CurrentRow.Cells["Id"].Value);

                int n = Db.Executar(
                    "UPDATE Emprestimos SET DataDevolucao = GETDATE() WHERE Id = @id AND DataDevolucao IS NULL",
                    new SqlParameter("@id", id));

                if (n == 0)
                    MessageBox.Show("Este empréstimo já foi devolvido.");
                else
                    MessageBox.Show("Devolução registrada.");

                Carregar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao devolver: " + ex.Message);
            }
        }
    }
}
