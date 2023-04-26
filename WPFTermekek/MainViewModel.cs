using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
namespace WPFTermekek
{
    public class MainViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler? PropertyChanged;
        string _kategoria;
        string _gyarto;
        string _nev;
        //Hardver = hw
        static string Connectionstring = "datasource=127.0.0.1;port=3306;username=root;password=;database=hw;";
        static MySqlConnection connection = new(Connectionstring);
        MySqlCommand command = new("SELECT * FROM termékek", connection);
        ObservableCollection<Termek>? _termekek;
        ObservableCollection<string>? _kategoriabox;
        ObservableCollection<string>? _gyartobox;
        public ObservableCollection<string>? Gyartok_Box
        {
            get => _gyartobox;
            set
            {
               
                OnPropertyChanged();

            }
        }
        public ObservableCollection<string> Kategoriak_Box
        {
            get => _kategoriabox;
            set
            {
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Termek>? Termekek
        {
            get => _termekek;
            set
            {
                _termekek = value;
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
        public MainViewModel()
        {
            Termekek = new();
            _kategoriabox = new();
            _gyartobox = new();
            FilterCommand = new RelayCommand(o => { FilterQuery(); }, o => { return CanFilter(); });
            //this.PropertyChanged += 
            ConnectToDatabase();
        }

        public void OnPropertyChanged([CallerMemberName] string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        void ConnectToDatabase()
        {
            connection.Open();
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                _termekek.Add(new Termek(
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetString(3),
                    reader.GetInt32(4),
                    reader.GetInt32(5)
                    ));
            }

            reader.Close();
            command.CommandText = "SELECT DISTINCT Kategória,Gyártó FROM termékek";
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                _kategoriabox.Add(reader.GetString(0));
                _gyartobox.Add(reader.GetString(1));
            }
            _kategoriabox.Insert(0,"-- Nincs megadva --");
            reader.Close();
            connection.Close();
        }

        void FilterQuery()
        {
            Termekek =(ObservableCollection<Termek>)_termekek.Where(x => x.kategoria.Equals(Kategoria));
            
        }
        bool CanFilter()
        {
            return !string.IsNullOrEmpty(Kategoria) || !string.IsNullOrEmpty(Gyarto);
        }
    }
}
