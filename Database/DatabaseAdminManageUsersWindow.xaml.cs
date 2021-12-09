﻿using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace ssi
{
    /// <summary>
    /// Interaktionslogik für DatabaseAdminWindow.xaml
    /// </summary>
    public partial class DatabaseAdminManageUsersWindow : Window
    {
        public DatabaseAdminManageUsersWindow()
        {
            InitializeComponent();

            GetUsers();
        }        

        public void GetUsers(string selectedItem = null)
        {
            UsersBox.Items.Clear();

            List<string> users = DatabaseHandler.GetUsers();

            foreach (string user in users)
            {
                UsersBox.Items.Add(user);
            }
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {            
            DatabaseAdminUserWindow dialog = new DatabaseAdminUserWindow();
            dialog.ShowDialog();

            if (dialog.DialogResult == true)
            {
                DatabaseUser user = new DatabaseUser()
                {
                    Name = dialog.GetName(),
                    Password = dialog.GetPassword(),
                    Fullname = dialog.GetFullName(),
                    Email = dialog.Getemail(),
                    Expertise = dialog.GetExpertise()
 
                };


                if (DatabaseHandler.AddUser(user, dialog.GetIsUserAdmin()))
                {
                    GetUsers();
                }
            }
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersBox.SelectedItem != null)
            {
                string user = (string)UsersBox.SelectedItem;
                MessageBoxResult result = MessageBox.Show("Delete user '" + user + "'?", "Question", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    if (DatabaseHandler.DeleteUser(user))
                    {
                        GetUsers();
                    }
                }
            }
        }

        private void EditUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersBox.SelectedItem != null)
            {
                DatabaseUser blankuser = new DatabaseUser()
                {
                    Name = (string)UsersBox.SelectedItem
                };

                blankuser = DatabaseHandler.GetUserInfo(blankuser);


                DatabaseAdminUserWindow dialog = new DatabaseAdminUserWindow(blankuser.Name,blankuser.Fullname,blankuser.Email,blankuser.Expertise);
                dialog.ShowDialog();

                if (dialog.DialogResult == true)
                {
                    DatabaseUser user = new DatabaseUser()
                    {
                        Name = dialog.GetName(),
                        Fullname = dialog.GetFullName(),
                        Email = dialog.Getemail(),
                        Expertise = dialog.GetExpertise(),
                        Password = dialog.GetPassword(),
                        ln_admin_key = blankuser.ln_admin_key,
                        ln_invoice_key = blankuser.ln_invoice_key,
                        ln_wallet_id = blankuser.ln_wallet_id,
                        ln_user_id = blankuser.ln_user_id
                    };



                    if (user.Password != "")
                    {
                        if (DatabaseHandler.ChangeUserPassword(user))
                        {
                         
                        }
                    }

                    //if user has no wallet
                    if (user.ln_wallet_id == null)
                    {
                        user.ln_wallet_id = "";
                        user.ln_user_id = "";
                        user.ln_invoice_key = "";
                        user.ln_admin_key = "";
                    }
                  

                    DatabaseHandler.ChangeUserCustomData(user);

                    GetUsers();

                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //DatabaseHandler.UpdateDatabaseLocalLists();
        }
    }
}