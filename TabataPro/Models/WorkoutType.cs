using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabataPro.Models
{
    class WorkoutType
    {
        public String WorkoutName { get; set; }
        public bool ItemSelected { get; set; }
        public int ID { get; set; }

        public WorkoutType()
        {

        }

        public List<WorkoutType> InitializaList()
        {
            int ID = 1;
            List<WorkoutType> wot = new List<WorkoutType>();

            string[] exArr = new string[] { "Push-ups", "Sit-ups", "Hindu Squat", "Jumping Lunges"
                                            ,"Jump Rope", "Box Jumps", "Hop Overs","Mountain Climbers"
                                            ,"Handstand Pushups", "Running","Burpees" };
            

            foreach (string ex in exArr)
            {
                WorkoutType wt = new WorkoutType()
                {
                    ID = ID,
                    WorkoutName = ex,
                    ItemSelected = true
                };

                wot.Add(wt);
                ID = ID + 1;
            }

            return wot;
        }


    }
}
