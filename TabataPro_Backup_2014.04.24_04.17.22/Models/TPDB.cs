using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.ComponentModel;
using System.Collections.ObjectModel;


namespace TabataPro.Models
{
    public class TPDB
    {
        public class exerciseDataContext : DataContext
        {
            // Specify the connection string as a static, used in main page and app.xaml.
            public static string DBConnectionString = "Data Source=isostore:/TPDB.sdf";

            // Pass the connection string to the base class.
            public exerciseDataContext(string connectionString)
                : base(connectionString)
            { }

            // Specify a single table for the to-do items.
            public Table<Exercise> Exercises;

        }


        [Table]
        public class Exercise : INotifyPropertyChanged, INotifyPropertyChanging
        {
            // Define ID: private field, public property and database column.
            private int _exerciseId;
            [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
            public int ExerciseId
            {
                get
                {
                    return _exerciseId;
                }
                set
                {
                    if (_exerciseId != value)
                    {
                        NotifyPropertyChanging("ExerciseId");
                        _exerciseId = value;
                        NotifyPropertyChanged("ExerciseId");
                    }
                }
            }

            private string _exerciseName;
            [Column]
            public string ExerciseName
            {
                get
                {
                    return _exerciseName;
                }
                set
                {
                    if (_exerciseName != value)
                    {
                        NotifyPropertyChanging("ExerciseName");
                        _exerciseName = value;
                        NotifyPropertyChanged("ExerciseName");
                    }
                }
            }

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            // Used to notify the page that a data context property changed
            private void NotifyPropertyChanged(string propertyName)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }

            #endregion

            #region INotifyPropertyChanging Members

            public event PropertyChangingEventHandler PropertyChanging;

            // Used to notify the data context that a data context property is about to change
            private void NotifyPropertyChanging(string propertyName)
            {
                if (PropertyChanging != null)
                {
                    PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
                }
            }

            #endregion

        }

    }
}
