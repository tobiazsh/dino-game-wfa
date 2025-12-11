using Flappy_Birds_WFA.GameObjects;
using System.ComponentModel;
using System.Diagnostics;

namespace Flappy_Birds_WFA.Utils
{
    public class Game : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public static Game Instance = new Game();

        // Components
        private Floor _floor1, _floor2;
        private Bird _bird;
        private List<PipePair> _pipes = new List<PipePair>();

        private int _nextPipe = 0;

        private bool _isGameOver = false;

        // Constants
        private const float SCROLL = 200f; // Floor movement speed

        private const int PIPE_SPAWN_MIN_INTERVAL = 250; // Minimum interval between pipes
        private const int PIPE_SPAWN_MAX_INTERVAL = 600; // Maximum interval between pipes
        private const int PIPE_MIN_GAP = 120; // Minimum gap between pipes
        private const int PIPE_MAX_GAP = 450; // Maximum gap between pipes
        private const int PIPE_MIN_WIDTH = 50; // Minimum pipe width
        private const int PIPE_MAX_WIDTH = 150; // Maximum pipe width

        public Game()
        {
            _floor1 = new Floor().SetBounds(Globals.GameWindowWidth, Floor.TEXTURE.Size.Height);
            _floor2 = new Floor().SetBounds(Globals.GameWindowWidth, Floor.TEXTURE.Size.Height);
            _bird = new Bird(Globals.GameWindowHeight / 2);
        }

        public void Initialize(Form parent)
        {
            _floor1.SetBounds(parent.ClientSize.Width, Floor.TEXTURE.Size.Height)
                  .SetPosition(0, parent.ClientSize.Height - _floor1.Height);

            _floor2.SetBounds(parent.ClientSize.Width, Floor.TEXTURE.Size.Height)
                  .SetPosition(_floor1.Width, parent.ClientSize.Height - _floor2.Height);

            _bird.SetPosition(
                    parent.ClientSize.Width / 4 - Bird.TEXTURE.Size.Width / 2,
                    parent.ClientSize.Height / 2 - Bird.TEXTURE.Size.Height / 2)
                .SetBounds(Bird.TEXTURE.Size.Width, Bird.TEXTURE.Size.Height);
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
                ConsoleWriter.DebugLine($"Score updated to: {_score}");
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
                ConsoleWriter.DebugLine($"Game Halted State changed to: {_isHalted}");
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
                ConsoleWriter.DebugLine($"Game Over State changed to: {_isGameOver}");
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
            _pipes.ForEach(pipePair => pipePair.Draw(e));
            _floor1.Draw(e);
            _floor2.Draw(e);
            _bird.Draw(e);
        }

        public void UpdateState(Form sender, float dt)
        {
            ScrollPipes(sender, dt);
            CheckScored();
            ScrollFloors(dt);
            UpdateBird(dt);
        }

        private void ScrollFloors(float dt)
        {
            _floor1.Scroll(SCROLL * dt);
            _floor2.Scroll(SCROLL * dt);

            // Check floor out of bounds and reposition
            if (_floor1.X + _floor1.Width <= 0)
            {
                _floor1.SetPosition(_floor2.X + _floor2.Width, _floor1.Y);
            }

            // Check floor out of bounds and reposition
            if (_floor2.X + _floor2.Width <= 0)
            {
                _floor2.SetPosition(_floor1.X + _floor1.Width, _floor2.Y);
            }
        }

        private void ScrollPipes(Form sender, float dt)
        {
            if (_nextPipe == 0)
            {
                _pipes.Add(PipePair.GenerateRandom(
                    PIPE_MIN_GAP, 
                    PIPE_MAX_GAP, 
                    PIPE_MIN_WIDTH, 
                    PIPE_MAX_WIDTH, 
                    sender.ClientSize.Height, sender.ClientSize.Width));

                _nextPipe = new Random().Next(PIPE_SPAWN_MIN_INTERVAL, PIPE_SPAWN_MAX_INTERVAL);
                ConsoleWriter.DebugLine($"Spawned new pipe. Next pipe in {_nextPipe} renders.");
            }

            _nextPipe--;

            _pipes.ForEach(pipePair => pipePair.Scroll(SCROLL * dt));

            _pipes.Where(pipePair => pipePair.IsCompletelyOutOfBounds(0, true))
                 .ToList()
                 .ForEach(pipePair =>
                 {
                     _pipes.Remove(pipePair);
                     ConsoleWriter.DebugLine("Removed out of bounds pipe.");
                 }); // Remove out of bounds pipes

            if (_pipes.Any(pipePair => pipePair.IntersectsWith(_bird)))
            {
                ConsoleWriter.DebugLine("Collision detected!");
                GameOver();
            }
        }

        private void GameOver()
        {
            IsGameOver = true;
            IsHalted = true;
            Achievements.Instance.Highscore = Math.Max(Achievements.Instance.Highscore, Score);
        }

        private void UpdateBird(float dt)
        {
            _bird.Calculate(_floor1.Y - _bird.Height, 0, dt);
        }

        private void CheckScored()
        {
            foreach (var pipePair in _pipes)
            {
                if (!pipePair.HasBeenScored && pipePair.CheckScore(_bird))
                {
                    Score++;
                    pipePair.HasBeenScored = true;
                }
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Jump()
        {
            _bird.Jump();
        }

        public void Reset()
        {
            _pipes.Clear();
            _nextPipe = 0;

            _score = 0;
            _isGameOver = false;
            _isHalted = true;

            // Recreate entities so they start fresh. Positions will be set in Initialize(parent) afterwards.
            _bird = new Bird(Globals.GameWindowHeight / 2);
            _floor1 = new Floor();
            _floor2 = new Floor();

            // Notify bindings about property changes so UI updates correctly
            OnPropertyChanged(nameof(Score));
            OnPropertyChanged(nameof(IsGameOver));
            OnPropertyChanged(nameof(IsHalted));

            ConsoleWriter.DebugLine("Game reset.");
        }
    }
}
