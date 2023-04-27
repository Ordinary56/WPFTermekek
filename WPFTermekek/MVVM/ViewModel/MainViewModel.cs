using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using WPFTermekek.Core;
using WPFTermekek.MVVM.Model;

namespace WPFTermekek.MVVM.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        string _kategoria = "-- Nincs megadva --";
        string _gyarto = "-- Nincs megadva --";
        string _nev;

        //Hardver adatbázisból vesszük ki az adatokat
        //VARCHAR MEZŐKNÉL A KÓDOLÁS LEGYEN UTF-16
        static string Connectionstring = "datasource=127.0.0.1;port=3306;username=root;password=;database=hardver;";

        MySqlConnection _connection = new(Connectionstring);

        ObservableCollection<Termek> _termekek;

        ICollectionView _termekekCollection;
        public List<string>? Gyartok_Box { get; set; }

        public List<string> Kategoriak_Box { get; set; }

        public ICollectionView TermekekCollection
        {
            get => _termekekCollection;
            set
            {
                _termekekCollection = value;
                OnPropertyChanged(nameof(TermekekCollection));
            }
        }

        public ObservableCollection<Termek>? Termekek
        {
            get => _termekek!;
            set
            {
                _termekek = value!;
                OnPropertyChanged(nameof(Termekek));
            }
        }

        public string Kategoria
        {
            get => _kategoria;
            set
            {
                _kategoria = value;
                OnPropertyChanged(Kategoria);
            }
        }
        public string Gyarto
        {
            get => _gyarto;
            set
            {
                _gyarto = value;
                OnPropertyChanged(Gyarto);
            }
        }
        public string Nev
        {
            get => _nev;
            set
            {
                _nev = value;
                OnPropertyChanged(Nev);
                TermekekCollection.Filter += Filter;
            }
        }

        public ICommand FilterCommand { get; }
        public ICommand SaveCommand { get; }
        public MainViewModel()
        {
            _termekek = new();
            FilterCommand = new RelayCommand(FilterQuery, CanFilter);
            SaveCommand = new RelayCommand(Save);
            ConnectToDatabase();
            

        }
        //Meghíváskor (program bezárásakor) törli a kapcsolatot
        ~MainViewModel()
        {
            _connection.Dispose();
        }


        void ConnectToDatabase()
        {
            _connection.Open();
            MySqlCommand command = new("SELECT * FROM termékek", _connection);
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                _termekek!.Add(new Termek(
                    reader.GetString("Kategória"),
                    reader.GetString("Gyártó"),
                    reader.GetString("Név"),
                    reader.GetInt32("Ár"),
                    reader.GetInt32("Garidő")
                    ));
            }
            reader.Close();
            Kategoriak_Box = _termekek
                .Select(x => x.kategoria)
                .Distinct()
                .ToList()!;
            Kategoriak_Box.Insert(0, "-- Nincs megadva --");
            Gyartok_Box = _termekek
                .Select(x => x.gyarto)
                .Distinct()
                .ToList()!;
            Gyartok_Box.Insert(0, "-- Nincs megadva --");
            _connection.Close();
            command.Dispose();
            reader.Dispose();
            _termekekCollection = CollectionViewSource.GetDefaultView(Termekek);

        }

        void FilterQuery(object paramter)
        {
            TermekekCollection.GroupDescriptions.Add(new PropertyGroupDescription(Kategoria));
            TermekekCollection.GroupDescriptions.Add(new PropertyGroupDescription(Gyarto));



        }
        void Save(object parameter)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Comma seperated values|*.csv|Text file|*.txt";
            saveFileDialog.FileName = "Save";
            saveFileDialog.DefaultExt = "csv";
            if ((bool)saveFileDialog.ShowDialog()!)
            {
                Stream fileStream = saveFileDialog.OpenFile();
                using (StreamWriter sw = new(fileStream))
                {
                    foreach (Termek termek in TermekekCollection)
                    {
                        sw.WriteLine(termek.ToCSVString());
                    }
                }
            }
            MessageBox.Show($"Sikeresen lementettük az adatokat ({TermekekCollection.Cast<object>().Count()} adat eltárolva)",
                "SaveSucces",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

        }
        bool CanFilter(object parameter)
        {
            return !string.IsNullOrEmpty(Kategoria) &&
                   !string.IsNullOrEmpty(Gyarto);
        }

        bool Filter(object parameter)
        {
            Termek termek = parameter as Termek;

            if (!string.IsNullOrEmpty(Nev))
            {
                return termek.nev.ToLower().Contains(Nev.ToLower());

            }


            return true;

        }
    }
}
