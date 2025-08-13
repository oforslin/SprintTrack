using System.Collections.ObjectModel;
using SprintTrack.Models;

namespace SprintTrack.Services
{
    public interface ITrainingDataService
    {
        ObservableCollection<TrainingSession> TrainingSessions { get; }
        void AddTrainingSession(TrainingSession session);
        void RemoveTrainingSession(TrainingSession session);
        void UpdateTrainingSession(TrainingSession session);
    }

    public class TrainingDataService : ITrainingDataService
    {
        public ObservableCollection<TrainingSession> TrainingSessions { get; } = new ObservableCollection<TrainingSession>();

        public TrainingDataService()
        {
            // Add some sample data
            InitializeSampleData();
        }

        public void AddTrainingSession(TrainingSession session)
        {
            TrainingSessions.Add(session);
        }

        public void RemoveTrainingSession(TrainingSession session)
        {
            TrainingSessions.Remove(session);
        }

        public void UpdateTrainingSession(TrainingSession session)
        {
            // Find and update existing session
            var existing = TrainingSessions.FirstOrDefault(s => s.Id == session.Id);
            if (existing != null)
            {
                var index = TrainingSessions.IndexOf(existing);
                TrainingSessions[index] = session;
            }
        }

        private void InitializeSampleData()
        {
            // Add some sample training sessions
            TrainingSessions.Add(new TrainingSession
            {
                Title = "Morning Run",
                Type = "Running",
                Description = "Easy pace morning jog in the park",
                Date = DateTime.Today.AddDays(-2),
                Duration = TimeSpan.FromMinutes(30),
                Intensity = 6
            });

            TrainingSessions.Add(new TrainingSession
            {
                Title = "Strength Training",
                Type = "Strength Training",
                Description = "Upper body workout focusing on chest and arms",
                Date = DateTime.Today.AddDays(-1),
                Duration = TimeSpan.FromMinutes(45),
                Intensity = 8
            });

            TrainingSessions.Add(new TrainingSession
            {
                Title = "Yoga Session",
                Type = "Yoga",
                Description = "Relaxing evening yoga flow",
                Date = DateTime.Today,
                Duration = TimeSpan.FromMinutes(60),
                Intensity = 4
            });
        }
    }
}