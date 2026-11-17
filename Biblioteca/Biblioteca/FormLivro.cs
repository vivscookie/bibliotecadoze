using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Biblioteca
{
    public class FormLivro : Form
    {
        TextBox txtTitulo, txtAutor, txtEditora;
        NumericUpDown numQtd;
        DataGridView grid;

        public FormLivro()
        {
            Text = "Cadastrar livro";
            ClientSize = new System.Drawing.Size(640, 480);
            StartPosition = FormStartPosition.CenterParent;

            txtTitulo = Ui.T(90, 15, 300);
            txtAutor = Ui.T(90, 45, 300);
            txtEditora = Ui.T(90, 75, 300);
            numQtd = new NumericUpDown { Left = 90, Top = 105, Width = 60, Minimum = 1, Maximum = 999, Value = 1 };
            grid = Ui.Grid(10, 145, 620, 325);

            Controls.AddRange(new Control[] {
                Ui.L("Título:", 10, 15), txtTitulo,
                Ui.L("Autor:", 10, 45), txtAutor,
                Ui.L("Editora:", 10, 75), txtEditora,
                Ui.L("Quantidade:", 10, 105), numQtd,
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
                    "SELECT Id, Titulo, Autor, Editora, Quantidade FROM Livros ORDER BY Titulo");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao acessar o banco: " + ex.Message);
            }
        }

        void Salvar(object sender, EventArgs e)
        {
            if (txtTitulo.Text.Trim() == "" || txtAutor.Text.Trim() == "")
            {
                MessageBox.Show("Informe o título e o autor.");
                return;
            }

            try
            {
                Db.Executar(
                    "INSERT INTO Livros (Titulo, Autor, Editora, Quantidade) VALUES (@t, @a, @e, @q)",
                    new SqlParameter("@t", txtTitulo.Text.Trim()),
                    new SqlParameter("@a", txtAutor.Text.Trim()),
                    new SqlParameter("@e", txtEditora.Text.Trim()),
                    new SqlParameter("@q", (int)numQtd.Value));

                txtTitulo.Clear();
                txtAutor.Clear();
                txtEditora.Clear();
                numQtd.Value = 1;
                Carregar();
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
                MessageBox.Show("Selecione um livro na lista.");
                return;
            }

            int id = Convert.ToInt32(grid.CurrentRow.Cells["Id"].Value);

            if (MessageBox.Show("Excluir o livro selecionado?", "Confirmação",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                Db.Executar("DELETE FROM Livros WHERE Id = @id", new SqlParameter("@id", id));
                Carregar();
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                MessageBox.Show("Não é possível excluir: o livro possui empréstimos registrados.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao excluir: " + ex.Message);
            }
        }
    }
}

