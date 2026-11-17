using System;
using System.Windows.Forms;

namespace Biblioteca
{
    // Pequenos helpers para criar controles por código (sem usar o designer)
    public static class Ui
    {
        public static Label L(string texto, int x, int y)
        {
            return new Label { Text = texto, Left = x, Top = y + 3, AutoSize = true };
        }

        public static TextBox T(int x, int y, int largura)
        {
            return new TextBox { Left = x, Top = y, Width = largura };
        }

        public static Button B(string texto, int x, int y, int largura, EventHandler click)
        {
            var b = new Button { Text = texto, Left = x, Top = y, Width = largura };
            b.Click += click;
            return b;
        }

        public static DataGridView Grid(int x, int y, int largura, int altura)
        {
            return new DataGridView
            {
                Left = x,
                Top = y,
                Width = largura,
                Height = altura,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
        }
    }
}
