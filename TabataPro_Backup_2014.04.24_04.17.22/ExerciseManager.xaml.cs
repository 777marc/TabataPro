using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using TabataPro.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Telerik.Windows.Controls;

namespace TabataPro
{
    public partial class ExerciseManager : PhoneApplicationPage
    {

        private TPDB.exerciseDataContext exerciseDB;
        private ObservableCollection<TPDB.Exercise> _exercises;
        public ObservableCollection<TPDB.Exercise> Exercises
        {
            get
            {
                return _exercises;
            }
            set
            {
                if (_exercises != value)
                {
                    _exercises = value;
                    NotifyPropertyChanged("Exercises");
                }
            }
        }

        public ExerciseManager()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();

            // Connect to the database and instantiate data context.
            exerciseDB = new TPDB.exerciseDataContext(TPDB.exerciseDataContext.DBConnectionString);

            // Data context and observable collection are children of the main page.
            this.DataContext = this;

            loadExercises();
        }

        private void loadExercises()
        {
            var exercises = from TPDB.Exercise ex in exerciseDB.Exercises
                            select ex;

            Exercises = new ObservableCollection<TPDB.Exercise>(exercises);

            if (Exercises.Count == 0)
            {
                initializeExerciseList();

                exercises = from TPDB.Exercise ex in exerciseDB.Exercises
                            select ex;

                Exercises = new ObservableCollection<TPDB.Exercise>(exercises);
            }

            exerciseList.ItemsSource = Exercises;
        }

        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();

            // Create a new button and set the text value to the localized string from AppResources.
            ApplicationBarIconButton appBarButton3 = new ApplicationBarIconButton(new Uri("/Assets/AppBar/back.png", UriKind.Relative));
            appBarButton3.Text = "Back";
            appBarButton3.Click += new EventHandler(go_home);
            ApplicationBar.Buttons.Add(appBarButton3);

            ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/add.png", UriKind.Relative));
            appBarButton.Text = "New Exercise"; //AppResources.AppBarButtonText;
            appBarButton.Click += new EventHandler(create_new_exercise);
            ApplicationBar.Buttons.Add(appBarButton);


        }

        private void create_new_exercise(object sender, EventArgs e)
        {
            RadInputPrompt.Show("Exercise Name", closedHandler: (args) =>
            {
                string exerciseName = args.Text;

                if (exerciseName == "" | exerciseName == null)
                {
                    return;
                }

                TPDB.Exercise newExercise = new TPDB.Exercise();

                newExercise.ExerciseName = exerciseName;

                // Add a to-do item to the observable collection.
                Exercises.Add(newExercise);

                // Add a to-do item to the local database.
                exerciseDB.Exercises.InsertOnSubmit(newExercise);
                exerciseDB.SubmitChanges();

            });
        }

        private void deleteExerciseButton_Click(object sender, RoutedEventArgs e)
        {
            // Cast parameter as a button.
            var button = sender as Button;

            if (button != null)
            {
                TPDB.Exercise exerciseToDelete = button.DataContext as TPDB.Exercise;
                Exercises.Remove(exerciseToDelete);
                exerciseDB.Exercises.DeleteOnSubmit(exerciseToDelete);
                exerciseDB.SubmitChanges();
            }
        }

        private void go_home(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/PivMain.xaml", UriKind.Relative));
        }

        private void initializeExerciseList()
        {
            string[] exArr = new string[] { "Push-ups", "Sit-ups", "Hindu Squat", "Jumping Lunges"
                                            ,"Jump Rope", "Box Jumps", "Hop Overs","Mountain Climbers"
                                            ,"Handstand Pushups", "Running","Burpees" };

            foreach (string ex in exArr)
            {
                TPDB.Exercise newExercise = new TPDB.Exercise();
                newExercise.ExerciseName = ex;
                Exercises.Add(newExercise);
                exerciseDB.Exercises.InsertOnSubmit(newExercise);
            }
            exerciseDB.SubmitChanges();
        }



        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify the app that a property has changed.
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion



    }
}