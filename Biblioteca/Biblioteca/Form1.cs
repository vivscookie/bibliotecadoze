using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Biblioteca
{
    public partial class Form1 : Form
    {
        TextBox txtPesquisa;
        DataGridView gridLivros;

        public Form1()
        {
            InitializeComponent();
            MontarTela();
            Carregar();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        void MontarTela()
        {
            Text = "Sistema de Biblioteca";
            ClientSize = new System.Drawing.Size(840, 520);
            StartPosition = FormStartPosition.CenterScreen;

            txtPesquisa = Ui.T(125, 15, 380);
            var btnPesquisar = Ui.B("Pesquisar", 515, 14, 85, (s, e) => Carregar());
            AcceptButton = btnPesquisar; // Enter também pesquisa

            gridLivros = Ui.Grid(15, 50, 585, 455);

            var btnEmprestimo = Ui.B("Realizar Empréstimo", 620, 60, 200, AbrirEmprestimo);
            var btnLivro = Ui.B("Cadastrar livro", 620, 100, 200, AbrirLivro);
            var btnCliente = Ui.B("Cadastrar cliente", 620, 140, 200, AbrirCliente);
            btnEmprestimo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnLivro.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCliente.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            Controls.AddRange(new Control[] {
                Ui.L("Pesquisar Livro:", 15, 15), txtPesquisa, btnPesquisar,
                gridLivros,
                btnEmprestimo, btnLivro, btnCliente });
        }

        void Carregar()
        {
            try
            {
                gridLivros.DataSource = Db.Consultar(
                    @"SELECT l.Id, l.Titulo, l.Autor, l.Editora, l.Quantidade,
                             l.Quantidade - (SELECT COUNT(*) FROM Emprestimos e
                                             WHERE e.LivroId = l.Id AND e.DataDevolucao IS NULL) AS Disponiveis
                      FROM Livros l
                      WHERE l.Titulo LIKE @p OR l.Autor LIKE @p
                      ORDER BY l.Titulo",
                    new SqlParameter("@p", "%" + txtPesquisa.Text.Trim() + "%"));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao acessar o banco: " + ex.Message);
            }
        }

        void AbrirEmprestimo(object sender, EventArgs e)
        {
            using (var f = new FormEmprestimo()) f.ShowDialog(this);
            Carregar();
        }

        void AbrirLivro(object sender, EventArgs e)
        {
            using (var f = new FormLivro()) f.ShowDialog(this);
            Carregar();
        }

        void AbrirCliente(object sender, EventArgs e)
        {
            using (var f = new FormCliente()) f.ShowDialog(this);
            Carregar();
        }
    }
}
