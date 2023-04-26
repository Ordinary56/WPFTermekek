using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using WPFTermekek.Core;
using WPFTermekek.MVVM.Model;

namespace WPFTermekek.MVVM.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        string _kategoria;
        string _gyarto;
        string _nev;
        //Hardver adatbázisból vesszük ki az adatokat
        //VARCHAR MEZŐKNÉL A KÓDOLÁS LEGYEN UTF-16
        static string Connectionstring = "datasource=127.0.0.1;port=3306;username=root;password=;database=hardver;";
        MySqlConnection _connection = new(Connectionstring);
        ObservableCollection<Termek> _termekek;
        ObservableCollection<Termek> Original;
        public List<string>? Gyartok_Box { get; set; }
        public List<string> Kategoriak_Box { get; set; }
        public ObservableCollection<Termek>? Termekek
        {
            get => _termekek!;
            set
            {
                _termekek = value!;
                OnPropertyChanged();
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
            }
        }

        public RelayCommand FilterCommand { get; }
        public RelayCommand SaveCommand { get; }
        public MainViewModel()
        {
            _termekek = new();
            FilterCommand = new RelayCommand(o => { FilterQuery(); }, o => { return CanFilter(); });
            this.PropertyChanged += queryName;
            SaveCommand = new RelayCommand(o => { Save(); });
            ConnectToDatabase();


        }
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
            Kategoriak_Box = _termekek.Select(x => x.kategoria)
                .Distinct()
                .ToList()!;
            Kategoriak_Box.Insert(0, "-- Nincs megadva --");
            Gyartok_Box = _termekek.Select(x => x.gyarto)
                .Distinct().ToList()!;
            Gyartok_Box.Insert(0, "-- Nincs megadva --");
            _connection.Close();
            command.Dispose();
            reader.Dispose();

            Original = _termekek;
        }

        void FilterQuery()
        {

            var filter = new ObservableCollection<Termek>(_termekek!.Where(x => x.kategoria!.Equals(Kategoria)
            || x.gyarto.Equals(Gyarto)));

            Termekek = filter.Count() > 0 ? filter : Original;

            OnPropertyChanged(nameof(Termekek));

        }
        void Save()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if ((bool)saveFileDialog.ShowDialog()!)
            {
                using (StreamWriter sw = new($"./{saveFileDialog.FileName}"))
                {
                    foreach (Termek termek in _termekek)
                    {
                        sw.WriteLine(termek.ToCSVString());
                    }
                }
            }
            MessageBox.Show($"Sikeresen lementettük az adatokat ({_termekek.Count()} adat eltárolva)",
                "SaveSucces",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        bool CanFilter()
        {
            return !string.IsNullOrEmpty(Kategoria)
                || !string.IsNullOrEmpty(Gyarto);
        }

        void queryName(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Nev)
            {
                if (Nev == "")
                {
                    Termekek = Original;
                }
                else
                {
                    Termekek = new ObservableCollection<Termek>(_termekek.Where(
                        x => x.nev!.Contains(Nev)));
                }

                OnPropertyChanged(nameof(Termekek));
            }
        }
    }
}
