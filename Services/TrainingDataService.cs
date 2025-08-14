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
            // Add sample training sessions with exercises
            var morningRun = new TrainingSession
            {
                Title = "Morning Run",
                Type = "Running",
                Description = "Easy pace morning jog in the park",
                Date = DateTime.Today.AddDays(-2),
                Duration = TimeSpan.FromMinutes(30),
                Intensity = 6
            };

            // Add a running exercise to the morning run
            var runningExercise = new Exercise
            {
                Name = "5K Run",
                Description = "Steady pace run through the neighborhood",
                ExerciseType = ExerciseType.Running,
                Duration = TimeSpan.FromMinutes(25),
                Distance = 5.0
            };
            runningExercise.RunningSets.Add(new RunningSet
            {
                SetNumber = 1,
                Duration = TimeSpan.FromMinutes(25),
                Distance = 5.0
            });
            morningRun.Exercises.Add(runningExercise);

            var strengthTraining = new TrainingSession
            {
                Title = "Strength Training",
                Type = "Strength Training",
                Description = "Upper body workout focusing on chest and arms",
                Date = DateTime.Today.AddDays(-1),
                Duration = TimeSpan.FromMinutes(45),
                Intensity = 8
            };

            // Add strength exercises
            var benchPress = new Exercise
            {
                Name = "Bench Press",
                Description = "Chest exercise with barbell",
                ExerciseType = ExerciseType.Strength,
                Sets = 3,
                Reps = 10,
                Weight = 80,
                Unit = "kg"
            };
            benchPress.ExerciseSets.Add(new ExerciseSet { SetNumber = 1, Reps = 10, Weight = 70, Unit = "kg" });
            benchPress.ExerciseSets.Add(new ExerciseSet { SetNumber = 2, Reps = 10, Weight = 80, Unit = "kg" });
            benchPress.ExerciseSets.Add(new ExerciseSet { SetNumber = 3, Reps = 8, Weight = 85, Unit = "kg" });
            strengthTraining.Exercises.Add(benchPress);

            var squats = new Exercise
            {
                Name = "Squat",
                Description = "Leg exercise with barbell",
                ExerciseType = ExerciseType.Strength,
                Sets = 3,
                Reps = 12,
                Weight = 100,
                Unit = "kg"
            };
            squats.ExerciseSets.Add(new ExerciseSet { SetNumber = 1, Reps = 12, Weight = 90, Unit = "kg" });
            squats.ExerciseSets.Add(new ExerciseSet { SetNumber = 2, Reps = 12, Weight = 100, Unit = "kg" });
            squats.ExerciseSets.Add(new ExerciseSet { SetNumber = 3, Reps = 10, Weight = 105, Unit = "kg" });
            strengthTraining.Exercises.Add(squats);

            var yogaSession = new TrainingSession
            {
                Title = "Yoga Session",
                Type = "Yoga",
                Description = "Relaxing evening yoga flow",
                Date = DateTime.Today,
                Duration = TimeSpan.FromMinutes(60),
                Intensity = 4
            };

            // Add a sprinting session with sprint exercises
            var sprintTraining = new TrainingSession
            {
                Title = "Sprint Training",
                Type = "Sprint Training",
                Description = "High intensity sprint workout",
                Date = DateTime.Today.AddDays(-3),
                Duration = TimeSpan.FromMinutes(45),
                Intensity = 9
            };

            var sprint100m = new Exercise
            {
                Name = "100m Sprint",
                Description = "Maximum effort 100m sprints",
                ExerciseType = ExerciseType.Sprinting,
                Distance = 100,
                SprintSeconds = 12,
                SprintHundredths = 50
            };
            sprint100m.RunningSets.Add(new RunningSet { SetNumber = 1, Distance = 100, SprintSeconds = 13, SprintHundredths = 20 });
            sprint100m.RunningSets.Add(new RunningSet { SetNumber = 2, Distance = 100, SprintSeconds = 12, SprintHundredths = 80 });
            sprint100m.RunningSets.Add(new RunningSet { SetNumber = 3, Distance = 100, SprintSeconds = 12, SprintHundredths = 50 });
            sprintTraining.Exercises.Add(sprint100m);

            var sledSprint = new Exercise
            {
                Name = "Sled Sprint",
                Description = "Weighted sled sprints",
                ExerciseType = ExerciseType.SledSprint,
                Distance = 20,
                Weight = 40,
                Unit = "kg",
                SprintSeconds = 8,
                SprintHundredths = 0
            };
            sledSprint.RunningSets.Add(new RunningSet { SetNumber = 1, Distance = 20, Weight = 40, SprintSeconds = 8, SprintHundredths = 50 });
            sledSprint.RunningSets.Add(new RunningSet { SetNumber = 2, Distance = 20, Weight = 40, SprintSeconds = 8, SprintHundredths = 20 });
            sledSprint.RunningSets.Add(new RunningSet { SetNumber = 3, Distance = 20, Weight = 40, SprintSeconds = 7, SprintHundredths = 90 });
            sprintTraining.Exercises.Add(sledSprint);

            // Add all sessions to the collection
            TrainingSessions.Add(morningRun);
            TrainingSessions.Add(strengthTraining);
            TrainingSessions.Add(yogaSession);
            TrainingSessions.Add(sprintTraining);
        }
    }
}