using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Xamarin.Forms;

[assembly: Dependency(typeof(ACD.App.Droid.FileBackedValueFactory))]

namespace ACD.App.Droid
{
    public class FileBacked<T> : ACD.App.FileBacked<T>
    {
        private class FileObserver : Android.OS.FileObserver
        {
            private FileBacked<T> _owner; 

            public FileObserver(string path, FileBacked<T> owner) : base(path, FileObserverEvents.Modify)
            {
                _owner = owner;
            }

            public override void OnEvent(FileObserverEvents e, string path)
            {
                _owner.FileChanged(File.ReadAllText(_owner._path));
            }
        }

        string _path;

        public FileBacked(string file)
        {
            string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            _path = Path.Combine(documentsPath, file);

            var observer = new FileObserver(_path, this);
            observer.StartWatching();

            if (File.Exists(_path)) {
                FileChanged(File.ReadAllText(_path));
            }
        }

        protected override void WriteToFile(string value)
        {
            File.WriteAllText(_path, value);
        }
    }

    public class FileBackedValueFactory : ACD.App.IFileBackedValueFactory
    {
        public ACD.App.FileBacked<T> Create<T>(string file)
        {
            return new FileBacked<T>(file);
        }
    }
}