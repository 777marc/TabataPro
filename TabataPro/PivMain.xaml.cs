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
using TabataPro.DAL;
using System.Windows.Threading;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Telerik.Windows.Controls;

namespace TabataPro
{
    public partial class PivMain : PhoneApplicationPage
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

        MarketplaceDetailTask _marketPlaceDetailTask = new MarketplaceDetailTask();
        TPParams _params = new TPParams();
        DispatcherTimer _newTimer = new DispatcherTimer();
        bool _working = true;
        int _interval = 0;
        int _restInterval = 10;
        int _workInterval = 20;
        int _startTime = 240;
        int _elapsedtime = 0;
        int _seconds = 0;
        int _currentExerciseId = 0;
        
        public PivMain()
        {
            InitializeComponent();
            _newTimer.Interval = TimeSpan.FromSeconds(1);
            _newTimer.Tick += OnTimerTick;

            // Connect to the database and instantiate data context.
            exerciseDB = new TPDB.exerciseDataContext(TPDB.exerciseDataContext.DBConnectionString);
            // Data context and observable collection are children of the main page.
            this.DataContext = this;

            loadExercises();

            // rate reminder
            RadRateApplicationReminder radRAReminder = new RadRateApplicationReminder();
            radRAReminder.RecurrencePerUsageCount = 3;
            radRAReminder.SkipFurtherRemindersOnYesPressed = true;
            radRAReminder.Notify();

            BuildLocalizedApplicationBar();

        }

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBarIconButton btnStart = new ApplicationBarIconButton(new Uri("/Assets/AppBar/play.png", UriKind.Relative));
            btnStart.Text = "start";
            btnStart.Click += new EventHandler(start);
            ApplicationBar.Buttons.Add(btnStart);

            ApplicationBarIconButton btnPause = new ApplicationBarIconButton(new Uri("/Assets/AppBar/pause.png", UriKind.Relative));
            btnPause.Text = "pause";
            btnPause.Click += new EventHandler(pause);
            ApplicationBar.Buttons.Add(btnPause);
            
            ApplicationBarIconButton btnStop = new ApplicationBarIconButton(new Uri("/Assets/AppBar/stop.png", UriKind.Relative));
            btnStop.Text = "stop";
            btnStop.Click += new EventHandler(stop);
            ApplicationBar.Buttons.Add(btnStop);

        }

        private void pause(object sender, EventArgs e)
        {
            startTimer();
            setButtonState("Pause");
        }

        private void start(object sender, EventArgs e)
        {
            startTimer();
            setButtonState("Start");
        }

        private void stop(object sender, EventArgs e)
        {
            Reset();
            setButtonState("Stop");
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

            rlExerciseTypes.ItemsSource = Exercises;
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

        //private void btnStartStop_Click(object sender, RoutedEventArgs e)
        //{
        //    startTimer();
        //}

        private void startTimer()
        {

            if (_newTimer.IsEnabled)
            {
                _newTimer.Stop();
                App.ButtonState = "Stop";
            }
            else
            {
                _newTimer.Start();
                App.ButtonState = "Start";
            }

            setState(_working);

        }

        private void setState(bool _working)
        {
            if (_working)
            {
                txtBlkState.Text = "Excercise";
                _interval = _workInterval;
                txtBlkCurrentExercise.Text = getExcercise();
                if (App.ButtonState != "Stop")
                {
                    playAlert(txtBlkCurrentExercise.Text);
                }
            }
            else
            {
                txtBlkState.Text = "Rest";
                _interval = _restInterval;
                if (App.ButtonState != "Stop")
                {
                    playAlert("Rest");
                }
            }
        }

        private void setButtonState(string btnstate)
        {
            btnstate = btnstate.ToLower();

            if (btnstate == "stop")
            {
                foreach (var button in ApplicationBar.Buttons)
                {
                    if (((ApplicationBarIconButton)button).Text == "stop")
                    {
                        ((ApplicationBarIconButton)button).IsEnabled = false; // disables the button
                    }

                    if (((ApplicationBarIconButton)button).Text == "start")
                    {
                        ((ApplicationBarIconButton)button).IsEnabled = true; // disables the button
                    }

                    if (((ApplicationBarIconButton)button).Text == "pause")
                    {
                        ((ApplicationBarIconButton)button).IsEnabled = false; // disables the button
                    }
                }
            }

            if (btnstate == "start")
            {
                foreach (var button in ApplicationBar.Buttons)
                {
                    if (((ApplicationBarIconButton)button).Text == "stop")
                    {
                        ((ApplicationBarIconButton)button).IsEnabled = false; // disables the button
                    }

                    if (((ApplicationBarIconButton)button).Text == "start")
                    {
                        ((ApplicationBarIconButton)button).IsEnabled = false; // disables the button
                    }

                    if (((ApplicationBarIconButton)button).Text == "pause")
                    {
                        ((ApplicationBarIconButton)button).IsEnabled = true; // disables the button
                    }
                }
            }

            if (btnstate == "pause")
            {
                foreach (var button in ApplicationBar.Buttons)
                {
                    if (((ApplicationBarIconButton)button).Text == "pause")
                    {
                        ((ApplicationBarIconButton)button).IsEnabled = false; // disables the button
                    }

                    if (((ApplicationBarIconButton)button).Text == "stop")
                    {
                        ((ApplicationBarIconButton)button).IsEnabled = true; // disables the button
                    }

                    if (((ApplicationBarIconButton)button).Text == "start")
                    {
                        ((ApplicationBarIconButton)button).IsEnabled = true; // disables the button
                    }
                }

            }

            App.ButtonState = btnstate;

        }

        void OnTimerTick(Object sender, EventArgs args)
        {

            _seconds += 1;
            _elapsedtime += 1;

            if (_seconds == _startTime)
            {
                playAlert("Complete");
                Reset();
                return;
            }

            if (_elapsedtime == _interval)
            {
                if (_working)
                {
                    _working = false;
                }
                else
                {
                    _working = true;
                }

                setState(_working);
                _elapsedtime = 0;
                
            }

            txtMainTimer.Text = getTime(_seconds);
            txtBlkIntervalTime.Text = (_interval - _elapsedtime).ToString();

            mainGauge.Maximum = _interval;
            mainGauge.Value = _elapsedtime;

            if ((_interval - _elapsedtime) == 5)
            {
                playAlert("FiveSeconds");
            }


        }

        private string getTime(int seconds)
        {

            TimeSpan t = TimeSpan.FromSeconds(_startTime - seconds);
            string readableTime;

            readableTime = string.Format("{0:D2}:{1:D2}",t.Minutes, t.Seconds);

            return readableTime;
        }

        private string getExcercise()
        {
            string exc = "";
            int numOfExc = 0;
            int i = 1;

            List<TPDB.Exercise> wol = new List<TPDB.Exercise>();

            foreach (TPDB.Exercise item in rlExerciseTypes.CheckedItems)
            {
                item.ExerciseId = i;
                wol.Add(item);
                i++;
            }

            if (wol.Count == 0)
            {
                return exc;
            }

            numOfExc = wol.Count;

            Random random = new Random();
            int randomNumber = 0;


            if (wol.Count != 1)
            {
                while (_currentExerciseId == randomNumber)
                {
                    randomNumber = random.Next(1, numOfExc);
                }
            }

            while (_currentExerciseId == randomNumber)
            {
                randomNumber = random.Next(1, numOfExc);
            }

            foreach (TPDB.Exercise ex in wol)
            {
                if (ex.ExerciseId == randomNumber)
                {
                    exc = ex.ExerciseName;
                    break;
                }
            }

            return exc;
        }

        private void playAlert(string alertName)
        {

            string fileName = "Complete";

            switch (alertName)
            {
                case "Push-ups" : fileName = "push_ups.wav";
                    break;
                case "Sit-ups" : fileName = "sit_ups.wav";
                    break;
                case "Hindu Squat" : fileName = "hindu_squats.wav";
                    break;
                case "Jumping Lunges" : fileName = "jumping_lunges.wav";
                    break;
                case "Jump Rope" : fileName = "jump_rope.wav";
                    break;
                case "Box Jumps" : fileName = "box_jump.wav";
                    break;                
                case "Hop Overs" : fileName = "hop_overs.wav";
                    break;                
                case "Mountain Climbers" : fileName = "mountain_climbers.wav";
                    break;    
                case "Handstand Pushups" : fileName = "handstand_pushups.wav";
                    break;                  
                case "Running" : fileName = "running.wav";
                    break;           
                case "Burpees" : fileName = "burpees.wav";
                    break;
                case "FiveSeconds": fileName = "fivesecs.wav";
                    break;
                case "Rest": fileName = "rest.wav";
                    break;
                case "Complete": fileName = "workout_complete.wav";
                    break;   
                default:
                    fileName = "warn.wav";
                    break;
            }



            var soundfile = "Assets/Sounds/" + fileName;
            Stream stream = TitleContainer.OpenStream(soundfile);
            SoundEffect effect = SoundEffect.FromStream(stream);
            FrameworkDispatcher.Update();
            effect.Play();
        }

        private void Reset()
        {
            txtBlkIntervalTime.Text = "";
            _newTimer.Stop();
            txtBlkCurrentExercise.Text = "";
            txtMainTimer.Text = "00:00";
            txtBlkState.Text = "Press Start";
            txtBlkIntervalTime.Text = "00";
            _elapsedtime = 0;
            _seconds = 0;
            _working = true;
            mainGauge.Value = 0;
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            Reset();
        }

        private void btnBuy_Click(object sender, RoutedEventArgs e)
        {
            _marketPlaceDetailTask.Show();
        }

        private void btnRate_Click(object sender, RoutedEventArgs e)
        {
            var rateTask = new MarketplaceReviewTask();
            rateTask.Show();
        }

        private void btnAddExercise_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ExerciseManager.xaml", UriKind.Relative));
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

        private void btnEmail_Click(object sender, RoutedEventArgs e)
        {
            string emailacct = "runwalkpromarc@gmail.com";

            EmailComposeTask emailComposeTask = new EmailComposeTask();

            emailComposeTask.Subject = "Message From Tabata PRO App";
            emailComposeTask.Body = "insert your message here";
            emailComposeTask.To = emailacct;

            emailComposeTask.Show();
        }

    }
}