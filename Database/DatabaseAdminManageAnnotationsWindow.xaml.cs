﻿using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ssi
{
    /// <summary>
    /// Interaktionslogik für DatabaseAdminWindow.xaml
    /// </summary>
    public partial class DatabaseAdminManageAnnotationsWindow : Window
    {

        GridViewColumnHeader _lastHeaderClicked = null;
        ListSortDirection _lastDirection = ListSortDirection.Ascending;

        public DatabaseAdminManageAnnotationsWindow()
        {
            InitializeComponent();

            GetDatabases(DatabaseHandler.DatabaseName);
        }

        private void Select(ListBox list, string select)
        {
            if (select != null)
            {
                foreach (string item in list.Items)
                {
                    if (item == select)
                    {
                        list.SelectedItem = item;
                    }
                }
            }
        }

        public void GetDatabases(string selectedItem = null)
        {
            DatabaseBox.Items.Clear();

            List<string> databases = DatabaseHandler.GetDatabases();

            foreach (string db in databases)
            {
                if (DatabaseHandler.CheckAuthentication(db) > 2)
                {
                    DatabaseBox.Items.Add(db);
                }
            }

            Select(DatabaseBox, selectedItem);
        }

        private void DataBaseResultsBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DatabaseBox.SelectedItem != null)
            {
                string name = DatabaseBox.SelectedItem.ToString();
                DatabaseHandler.ChangeDatabase(name);
                GetAnnotations();                
            }
        }

        public void GetAnnotations(string selectedItem = null)
        {
           
            if (AnnotationsBox.HasItems)
            {
                AnnotationsBox.ItemsSource = null;
            }

            List<DatabaseAnnotation> items = DatabaseHandler.GetAnnotations();
            
            AnnotationsBox.ItemsSource = items;           
        }                

        private void DeleteAnnotations_Click(object sender, RoutedEventArgs e)
        {
            if (AnnotationsBox.SelectedItem != null)
            {
                var annotations = AnnotationsBox.SelectedItems;
                foreach (DatabaseAnnotation annotation in annotations)
                {
                    DatabaseHandler.DeleteAnnotation(annotation.Id);
                }
                GetAnnotations();
            }            
        }

        private void ChangeFinishedState(ObjectId id, bool state)
        {
            var annos = DatabaseHandler.Database.GetCollection<BsonDocument>(DatabaseDefinitionCollections.Annotations);
            var builder = Builders<BsonDocument>.Filter;

            var filter = builder.Eq("_id", id);
            var update = Builders<BsonDocument>.Update.Set("isFinished", state);
            annos.UpdateOne(filter, update);


        }

        private void ChangeLockedState(ObjectId id, bool state)
        {
            var annos = DatabaseHandler.Database.GetCollection<BsonDocument>(DatabaseDefinitionCollections.Annotations);
            var builder = Builders<BsonDocument>.Filter;

            var filter = builder.Eq("_id", id);
            var update = Builders<BsonDocument>.Update.Set("isLocked", state);
            annos.UpdateOne(filter, update);
        }


        private void IsFinishedCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            DatabaseAnnotation anno = (DatabaseAnnotation)((CheckBox)sender).DataContext;
            ChangeFinishedState(anno.Id, true);
        }

        private void IsFinishedCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            DatabaseAnnotation anno = (DatabaseAnnotation)((CheckBox)sender).DataContext;
            ChangeFinishedState(anno.Id, false);
        }

        private void IsLockedCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            DatabaseAnnotation anno = (DatabaseAnnotation)((CheckBox)sender).DataContext;
            ChangeLockedState(anno.Id, false);
        }

        private void IsLockedCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            DatabaseAnnotation anno = (DatabaseAnnotation)((CheckBox)sender).DataContext;
            ChangeLockedState(anno.Id, true);
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private bool UserFilter(object item)
        {
            if (string.IsNullOrEmpty(searchTextBox.Text))
                return true;
            else
                return (((item as DatabaseAnnotation).Scheme.IndexOf(searchTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                    || (item as DatabaseAnnotation).AnnotatorFullName.IndexOf(searchTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0
                    || (item as DatabaseAnnotation).Annotator.IndexOf(searchTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0
                    || (item as DatabaseAnnotation).Role.IndexOf(searchTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0
                    || (item as DatabaseAnnotation).Session.IndexOf(searchTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (AnnotationsBox.ItemsSource != null)
            {
                System.Windows.Data.CollectionView view = (System.Windows.Data.CollectionView)System.Windows.Data.CollectionViewSource.GetDefaultView(AnnotationsBox.ItemsSource);
                view.Filter = UserFilter;
                System.Windows.Data.CollectionViewSource.GetDefaultView(AnnotationsBox.ItemsSource).Refresh();
            }
        }


        private void SortListView(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked =
             e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction;

            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (headerClicked != _lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (_lastDirection == ListSortDirection.Ascending)
                        {
                            direction = ListSortDirection.Descending;
                        }
                        else
                        {
                            direction = ListSortDirection.Ascending;
                        }
                    }

                    string header = headerClicked.Column.Header as string;
                    ICollectionView dataView = CollectionViewSource.GetDefaultView(((ListView)sender).ItemsSource);

                    dataView.SortDescriptions.Clear();
                    SortDescription sd = new SortDescription(header, direction);
                    dataView.SortDescriptions.Add(sd);
                    dataView.Refresh();


                    if (direction == ListSortDirection.Ascending)
                    {
                        headerClicked.Column.HeaderTemplate =
                          Resources["HeaderTemplateArrowUp"] as DataTemplate;
                    }
                    else
                    {
                        headerClicked.Column.HeaderTemplate =
                          Resources["HeaderTemplateArrowDown"] as DataTemplate;
                    }

                    // Remove arrow from previously sorted header  
                    if (_lastHeaderClicked != null && _lastHeaderClicked != headerClicked)
                    {
                        _lastHeaderClicked.Column.HeaderTemplate = null;
                    }

                    _lastHeaderClicked = headerClicked;
                    _lastDirection = direction;
                }
            }
        }

        private void AnnotationsBox_Click(object sender, RoutedEventArgs e)
        {
            SortListView(sender, e);
        }

    }
}