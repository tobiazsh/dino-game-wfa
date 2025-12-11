using Dino_Game_WFA.Utils;

namespace Dino_Game_WFA
{
    public partial class GameWindow : Form
    {
        public GameWindow()
        {
            InitializeComponent();
            Initialize();

            // Properties
            this.Height = Globals.GameWindowHeight;
            this.Width = Globals.GameWindowWidth;
            this.DoubleBuffered = true; // Reduce flickering
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedDialog; // Prevent resizing
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.SkyBlue;
            this.Text = "Dinosaur Game - Game Window";

            Game.Instance.Initialize(this);

            // Events
            this.KeyDown += HandleKeyDown;
            this.FormClosed += MainWindow.NavigateToMenuHandler;
        }

        // Components
        Label haltedInfoLabel = new Label();
        Label gameOverLabel = new Label();

        private void Initialize()
        {
            haltedInfoLabel.Text = $"Game is halted. Press any key to continue and {Keys.Pause.ToString()} to halt again!";
            haltedInfoLabel.DataBindings.Add("Visible", Game.Instance, "IsHalted", true, DataSourceUpdateMode.OnPropertyChanged, true, "");
            haltedInfoLabel.Font = Globals.TitleFont;
            haltedInfoLabel.AutoSize = true;
            haltedInfoLabel.Parent = this;

            // Game over label
            var scoreBinding = new Binding("Text", Game.Instance, "Score", true, DataSourceUpdateMode.OnPropertyChanged, 0);

            scoreBinding.Format += (sender, e) =>
            {
                e.Value = $"Game Over! Your Score: {e.Value}. Press any key to restart.";
            };

            scoreBinding.BindingComplete += (s, e) =>
            {
                if (e.BindingCompleteState == BindingCompleteState.Success)
                    RecenterGameOverLabel();
            };

            gameOverLabel.DataBindings.Add(scoreBinding);

            var visibilityBinding = new Binding("Visible", Game.Instance, "IsGameOver", true, DataSourceUpdateMode.OnPropertyChanged, false, "");

            visibilityBinding.BindingComplete += (s, e) =>
            {
                if (e.BindingCompleteState == BindingCompleteState.Success)
                {
                    if (gameOverLabel.Visible)
                        RecenterGameOverLabel();
                }
            };

            gameOverLabel.DataBindings.Add(visibilityBinding);
            gameOverLabel.Font = Globals.TitleFont;
            gameOverLabel.AutoSize = true;
            gameOverLabel.Location = new Point((this.ClientSize.Width - gameOverLabel.Width) / 2, (this.ClientSize.Height - gameOverLabel.Height) / 2);
            gameOverLabel.Parent = this;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Game.Instance.GameLoop(this, e);
            base.OnPaint(e);
        }

        private void RecenterGameOverLabel()
        {
            gameOverLabel.PerformLayout();
            int x = Math.Max(0, (this.ClientSize.Width - gameOverLabel.Width) / 2);
            int y = Math.Max(0, (this.ClientSize.Height - gameOverLabel.Height) / 2);
            gameOverLabel.Location = new Point(x, y);
            gameOverLabel.BringToFront();
            this.Invalidate();
        }

        private void HandleKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Pause)
            {
                Game.Instance.IsHalted = true; // Pause the game on Pause key
                return;
            }

            if (!Game.Instance.IsHalted && e.KeyCode == Keys.Space)
            {
                // Handle Jump
            }

            if (Game.Instance.IsHalted && !Game.Instance.IsGameOver)
            {
                Game.Instance.IsHalted = false; // Start the game with any key if it was halted
                return;
            }

            if (Game.Instance.IsGameOver) // If game over, any key restarts
            {
                Game.Instance.Reset();
                Game.Instance.Initialize(this);
                this.Invalidate(); // Redraw the game window
            }
        }
    }
}
