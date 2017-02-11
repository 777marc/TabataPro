using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;
using TabataPro.Models;

namespace TabataPro.DAL
{

    class TPParams
    {
        IsolatedStorageSettings _settings = IsolatedStorageSettings.ApplicationSettings;

        List<WorkoutType> _workoutTypes;
        public List<WorkoutType> WorkoutTypes
        {
            get
            {
                if (_settings.Contains("WorkoutTypes"))
                {
                    this._workoutTypes = (List<WorkoutType>)_settings["WorkoutTypes"];
                }
                else
                {
                    WorkoutType wt = new WorkoutType();
                    this._workoutTypes = wt.InitializaList();
                    _settings.Add("WorkoutTypes", _workoutTypes);
                }
                return _workoutTypes;
            }
            set
            {
                this._workoutTypes = value;
                _settings["WorkoutTypes"] = value;
                _settings.Save();
            }
        }

        int _appRuns;
        public int AppRuns
        {

            get
            {
                if (_settings.Contains("AppRuns"))
                {
                    this._appRuns = (int)_settings["AppRuns"];
                }
                else
                {
                    this._appRuns = 0;
                    _settings.Add("AppRuns", _appRuns);
                }
                return _appRuns;
            }
            set
            {
                if (_settings.Contains("AppRuns"))
                {
                    this._appRuns = value;
                    _settings["AppRuns"] = value;
                }
                else
                {
                    this._appRuns = 0;
                    _settings.Add("AppRuns", _appRuns);
                }
            }
        }


    }
}
