﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WPFTermekek.MVVM.ViewModel;

namespace WPFTermekek
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
       
        protected override void OnStartup(StartupEventArgs e)
        {
           

            MainWindow window = new MainWindow() { DataContext = new MainViewModel()};
            window.Show();
            base.OnStartup(e);
        }
    }
}
