using Dino_Game_WFA.GameObjects;
using System.ComponentModel;
using System.Diagnostics;

namespace Dino_Game_WFA.Utils
{

    // "OBST" = "Obstacle"!!
    public class Game : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public static Game Instance = new Game();

        // Components
        private Dino _dino = new Dino(0, 1f); // Will be properly initialized in Initialize()
        private List<Obstacle> _obstacles = new();

        private int _nextObst = 0;

        private bool _isGameOver = false;

        private float _scale = 1f;

        // Constants
        private const float SCROLL = 200f; // Floor movement speed

        private const int OBST_SPAWN_MIN_INTERVAL = 250; // Minimum interval between pipes
        private const int OBST_SPAWN_MAX_INTERVAL = 600; // Maximum interval between pipes
        
        private float FLOOR_H = 250; // Default floor height, will be set in Initialize()
        private float _parentH;
        private float _parentW;

        public void Initialize(Form parent, float floorH, float scale)
        {
            _parentH = parent.ClientSize.Height;
            _parentW = parent.ClientSize.Width;

            this._scale = scale;

            FLOOR_H = floorH;

            float startingY = FLOOR_H - (Dino.TEXTURE_RUNNING.Size.Height * _scale);
            _dino = new Dino(startingY, _scale);

            _dino.SetPosition(
                    _parentW / 4 - Dino.TEXTURE_RUNNING.Size.Width * _scale / 2, // Assume Dino is running since it will be the case most of the time
                    startingY)
                .SetBounds(Dino.TEXTURE_RUNNING.Size.Width * _scale, Dino.TEXTURE_RUNNING.Size.Height * _scale);
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
            RenderFloor(e);
            _obstacles.ForEach(obst => obst.Draw(e));
            _dino.Draw(e);
        }

        private void RenderFloor(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;

            // Draw Floor
            graphics.FillRectangle(Brushes.SaddleBrown, 0, FLOOR_H, _parentW, _parentH - FLOOR_H);
        }

        public void UpdateState(Form sender, float dt)
        {
            ScrollObstacles(sender, dt);
            CheckScored();
            UpdateDino(dt);
        }

        private void ScrollObstacles(Form sender, float dt)
        {
            if (_nextObst == 0)
            {
                _obstacles.Add(Obstacle.CreateRandomObstacle(_parentW, FLOOR_H, _scale));

                _nextObst = new Random().Next(OBST_SPAWN_MIN_INTERVAL, OBST_SPAWN_MAX_INTERVAL);
            }

            _nextObst--;

            _obstacles.ForEach(obstacle => obstacle.Scroll(SCROLL * dt));

            _obstacles.Where(obstacle => obstacle.IsCompletelyOutOfBounds(0, true))
                 .ToList()
                 .ForEach(obstacle =>
                 {
                     _obstacles.Remove(obstacle);
                 }); // Remove out of bounds pipes

            if (_obstacles.Any(obstacle => obstacle.IntersectsWith(_dino)))
            {
                _dino.Deadify();
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
            _dino.Calculate(FLOOR_H - _dino.Height, 0, dt);
        }

        private void CheckScored()
        {
            foreach (var obstacle in _obstacles)
            {
                obstacle.CheckScored(_dino, () =>
                {
                    Score++;
                });
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Jump(bool allowMultiple)
        {
            _dino.Jump(allowMultiple);
        }

        public void Reset()
        {
            _obstacles.Clear();
            _nextObst = 0;

            _score = 0;
            _isGameOver = false;
            _isHalted = true;

            // Recreate entities so they start fresh. Positions will be set in Initialize(parent) afterwards.
            _dino = new Dino(_parentH / 2, _scale);

            // Notify bindings about property changes so UI updates correctly
            OnPropertyChanged(nameof(Score));
            OnPropertyChanged(nameof(IsGameOver));
            OnPropertyChanged(nameof(IsHalted));
        }
    }
}
