﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AxeSoftware.Quest.EditorControls
{
    public partial class CompassEditorControl : UserControl
    {
        public enum CompassEditorMode
        {
            NoSelection,
            NotACompassExit,
            NewCompassExit,
            ExistingCompassExit
        }

        private CompassEditorMode m_mode;

        public event Action<string, string> CreateExit;
        public event Action<string> EditExit;

        public CompassEditorControl()
        {
            InitializeComponent();
        }

        public CompassEditorMode Mode
        {
            get { return m_mode; }
            set
            {
                m_mode = value;

                if (m_mode == CompassEditorMode.NoSelection || m_mode == CompassEditorMode.NotACompassExit)
                {
                    toLabel.Visibility = Visibility.Collapsed;
                    to.Visibility = Visibility.Collapsed;
                    toName.Visibility = Visibility.Collapsed;
                    create.Visibility = Visibility.Collapsed;
                    edit.Visibility = Visibility.Collapsed;
                    chkCorresponding.Visibility = Visibility.Collapsed;
                    corresponding.Visibility = Visibility.Collapsed;
                    createCorresponding.Visibility = Visibility.Collapsed;

                    direction.Text = m_mode == CompassEditorMode.NoSelection ? "No exit selected" : "Selected exit is not a compass direction";
                }
                else if (m_mode == CompassEditorMode.NewCompassExit)
                {
                    toLabel.Visibility = Visibility.Visible;
                    to.Visibility = Visibility.Visible;
                    toName.Visibility = Visibility.Collapsed;
                    create.Visibility = Visibility.Visible;
                    edit.Visibility = Visibility.Collapsed;

                    // depends on where the exit points
                    chkCorresponding.Visibility = Visibility.Visible;
                    corresponding.Visibility = Visibility.Visible;

                    createCorresponding.Visibility = Visibility.Collapsed;
                }
                else if (m_mode == CompassEditorMode.ExistingCompassExit)
                {
                    toLabel.Visibility = Visibility.Visible;
                    to.Visibility = Visibility.Collapsed;
                    toName.Visibility = Visibility.Visible;
                    create.Visibility = Visibility.Collapsed;
                    edit.Visibility = Visibility.Visible;
                    chkCorresponding.Visibility = Visibility.Collapsed;

                    // depends on where the exit points
                    corresponding.Visibility = Visibility.Visible;
                    createCorresponding.Visibility = Visibility.Visible;
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        public string DirectionName { get; set; }
        public string ExitID { get; set; }

        private void edit_Click(object sender, RoutedEventArgs e)
        {
            EditExit(ExitID);
        }

        private void to_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            create.IsEnabled = true;
        }

        private void create_Click(object sender, RoutedEventArgs e)
        {
            CreateExit((string)to.SelectedItem, DirectionName);
        }
    }
}
