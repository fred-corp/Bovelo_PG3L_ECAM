﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Main
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Button_Production_Click(object sender, RoutedEventArgs e)
        {
            ProductionWindow mnw = new();
            mnw.Owner = Window.GetWindow(this);
            mnw.ShowDialog();
            this.Close();
        }

        private void Button_Sales_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mnw = new();
            mnw.Owner = Window.GetWindow(this);
            mnw.ShowDialog();
            this.Close();
        }
    }
}