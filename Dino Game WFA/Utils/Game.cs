using Dino_Game_WFA.GameObjects;
using Dino_Game_WFA.Resource;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace Dino_Game_WFA.Utils
{
    public class Game : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public static Game Instance = new Game();

        private static readonly int FLOOR_HEIGHT = 160;

        // Components
        private Dino _dino;
        private Rectangle _floor = new Rectangle(0, Globals.GameWindowHeight - FLOOR_HEIGHT, Globals.GameWindowWidth, FLOOR_HEIGHT);
        private List<Obstacle> _obstacles = new List<Obstacle>();

        private int _nextObstacle = 0;

        private bool _isGameOver = false;

        // Constants
        private const float SCROLL = 200f; // Movement speed

        private const int OBSTACLE_SPAWN_MIN_INTERVAL = 250; // Minimum interval between pipes
        private const int OBSTACLE_SPAWN_MAX_INTERVAL = 600; // Maximum interval between pipes

        public Game()
        {
            _dino = new Dino(Globals.GameWindowHeight / 2);
        }

        public void Initialize(Form parent)
        {
            _dino.SetPosition(
                    parent.ClientSize.Width / 4 - Dino.TEXTURE_RUNNING.Size.Width / 2,
                    parent.ClientSize.Height / 2 - Dino.TEXTURE_RUNNING.Size.Height / 2)
                .SetBounds(Dino.TEXTURE_RUNNING.Size.Width, Dino.TEXTURE_RUNNING.Size.Height);
        }

        // Private Fields

        private int _score;
        private bool _isHalted = true; // Make game not immediately start
        private readonly Stopwatch _gameTimer = Stopwatch.StartNew();

        // Properties
        public int Score
        {
            get => _score;
            set
            {
                if (_score == value) return;

                _score = value;
                OnPropertyChanged(nameof(Score));
            }
        }

        public bool IsHalted
        {
            get => _isHalted;
            set
            {
                if (_isHalted == value) return;

                _isHalted = value;
                OnPropertyChanged(nameof(IsHalted));
            }
        }

        public bool IsGameOver
        {
            get => _isGameOver;
            set
            {
                if (_isGameOver == value) return;
                _isGameOver = value;
                OnPropertyChanged(nameof(IsGameOver));
            }
        }

        public void GameLoop(Form sender, PaintEventArgs e)
        {
            float dt = (float)_gameTimer.Elapsed.TotalSeconds;
            _gameTimer.Restart();

            if (!IsHalted)
            {
                UpdateState(sender, dt); // Allow update state only if not halted
                sender.Invalidate(); // Inform about update
            }

            Render(e); // Always render regardless of halted state
        }

        public void Render(PaintEventArgs e)
        {
            _obstacles.ForEach(pipePair => pipePair.Draw(e));
            DrawFloor(e.Graphics);
            _dino.Draw(e);
        }

        public void UpdateState(Form sender, float dt)
        {
            ScrollObstacles(sender, dt);
            CheckScored();
            UpdateDino(dt);
        }

        private void DrawFloor(Graphics gr)
        {
            gr.DrawRectangle(Pens.DarkSlateGray, _floor);
        }

        private void ScrollObstacles(Form sender, float dt)
        {
            if (_nextObstacle == 0)
            {
                int obstacleType = new Random().Next(0, 2);
                _obstacles.Add(new Obstacle(obstacleType == 1 ? Obstacle.ObstacleType.OBS1 : Obstacle.ObstacleType.OBS2));

                _nextObstacle = new Random().Next(OBSTACLE_SPAWN_MIN_INTERVAL, OBSTACLE_SPAWN_MAX_INTERVAL);
            }

            _nextObstacle--;

            _obstacles.ForEach(obstacle => obstacle.Scroll(SCROLL * dt));

            _obstacles.Where(obstacle => obstacle.IsCompletelyOutOfBounds(0, true))
                 .ToList()
                 .ForEach(obstacle =>
                 {
                     _obstacles.Remove(obstacle);
                 }); // Remove out of bounds pipes

            if (_obstacles.Any(obstacle => obstacle.IntersectsWith(_dino)))
            {
                GameOver();
            }
        }

        private void GameOver()
        {
            IsGameOver = true;
            IsHalted = true;
            Achievements.Instance.Highscore = Math.Max(Achievements.Instance.Highscore, Score);
        }

        private void UpdateDino(float dt)
        {
            _dino.Calculate(Globals.GameWindowHeight - FLOOR_HEIGHT + Dino.TEXTURE_RUNNING.Size.Height, Globals.GameWindowHeight / 2, dt);
        }

        private void CheckScored()
        {
            foreach (var obstacle in _obstacles)
            {
                if (obstacle.CheckScored(_dino.X))
                    Score++;
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Jump()
        {
            _dino.Jump();
        }

        public void Reset()
        {
            _obstacles.Clear();
            _nextObstacle = 0;

            _score = 0;
            _isGameOver = false;
            _isHalted = true;

            // Recreate entities so they start fresh. Positions will be set in Initialize(parent) afterwards.
            _dino = new Dino(Globals.GameWindowHeight - FLOOR_HEIGHT + Dino.TEXTURE_RUNNING.Size.Height);

            // Notify bindings about property changes so UI updates correctly
            OnPropertyChanged(nameof(Score));
            OnPropertyChanged(nameof(IsGameOver));
            OnPropertyChanged(nameof(IsHalted));

            // ConsoleWriter.DebugLine("Game reset.");
        }
    }
}
