using Dino_Game_WFA.Utils;
using System.Net.Http.Headers;

namespace Dino_Game_WFA
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
            Initialize();

            // Properties
            this.Text = "Dinosaur Game - Main Menu";

            // Events
            this.BackColor = Color.DarkSlateGray;
            this.FormClosed += (s, e) => Terminate();
            this.KeyDown += HandleKeyDown;
        }

        TableLayoutPanel tableLayoutPanel = new TableLayoutPanel();

        Button startButton = new Button();
        Label highScoreLabel = new Label();
        Label titleLabel = new Label();

        private void Initialize()
        {
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Parent = this;
            tableLayoutPanel.RowCount = 3;
            tableLayoutPanel.ColumnCount = 1;

            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 60F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            titleLabel.Dock = DockStyle.Fill;
            titleLabel.Text = "Dinosaur Game";
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.ForeColor = Color.WhiteSmoke;
            titleLabel.Font = new Font("Arial", 24, FontStyle.Bold);
            tableLayoutPanel.Controls.Add(titleLabel, 0, 0);

            highScoreLabel.Dock = DockStyle.Fill;
            highScoreLabel.ForeColor = Color.WhiteSmoke;
            highScoreLabel.Font = new Font("Arial", 16, FontStyle.Regular);
            highScoreLabel.TextAlign = ContentAlignment.MiddleCenter;
            tableLayoutPanel.Controls.Add(highScoreLabel, 0, 1);

            startButton.Anchor = AnchorStyles.None;
            startButton.Text = "Start Game";
            startButton.AutoSize = true;
            startButton.Padding = new Padding(20);
            startButton.BackColor = Color.Yellow;
            startButton.Font = new Font("Arial", 14, FontStyle.Bold);
            startButton.Click += (s, e) => StartGame();
            tableLayoutPanel.Controls.Add(startButton, 0, 2);

            Binding highscoreBinding = new Binding("Text", Achievements.Instance, "Highscore", true, DataSourceUpdateMode.OnPropertyChanged);

            highscoreBinding.Format += (sender, e) =>
            {
                e.Value = $"Highscore: {e.Value}";
            };

            highScoreLabel.DataBindings.Add(highscoreBinding);
        }

        private void StartGame()
        {
            this.Hide();
            new GameWindow().Show();
        }

        /// <summary>
        /// Is supposed to be implemented in a FormClosedEvent to navigate back to menu from other forms when they're closed.
        /// </summary>
        /// <param name="sender">Sending Form1</param>
        /// <param name="e">Handler</param>
        public static void NavigateToMenuHandler(object? sender, FormClosedEventArgs e)
        {
            var closingForm = sender as Form;
            if (closingForm == null) return;

            // Get all open forms EXCEPT the one that just closed
            var otherForms = Application.OpenForms
                .OfType<Form>()
                .Where(f => f != closingForm && !f.IsDisposed && f is not MainWindow)
                .ToArray();

            // Close all others
            foreach (var form in otherForms)
            {
                form.Close();
            }

            // Show main menu (create only if not already open)
            var menu = Application.OpenForms.OfType<MainWindow>().FirstOrDefault()
                       ?? new MainWindow();

            menu.Show();
            menu.BringToFront();
        }

        private void Terminate()
        {
            Application.Exit();
        }

        private void HandleKeyDown(object? sender, KeyEventArgs e)
        {
            // Handle key down events here

            if (e.KeyCode == Keys.Escape)
                this.Close();
        }
    }
}
