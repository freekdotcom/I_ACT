using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using SQLite.Net.Attributes;

namespace ACD
{
    /*
     * Class used to represent tips.
     */
    public class Tip : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        private bool enabled;

        [JsonIgnore]
        [DefaultValue(false)]
        public bool Enabled
        {
            set
            {
                if (enabled == value) return;

                enabled = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Enabled"));
            }
            get
            {
                return enabled;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
