using System.ComponentModel;

namespace Flappy_Birds_WFA.Utils
{
    class Achievements : INotifyPropertyChanged
    {
        public static Achievements Instance = new Achievements();

        private int highscore;

        public int Highscore
        {
            get => highscore;
            set {
                if (highscore == value) return; // Prevent unnecessary updates

                highscore = value;
                OnPropertyChanged(nameof(Highscore));
            }
        }

        public void TryUpdateHighscore(int score)
        {
            if (score > Highscore)
                Highscore = score;
        }

        public void Reset()
        {
            Highscore = 0;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
