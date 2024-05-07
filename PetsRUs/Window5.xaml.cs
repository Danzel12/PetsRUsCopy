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

namespace PetsRUs
{
    public partial class Window5 : Window
    {
        private Dictionary<string, dynamic> _cartItems = new Dictionary<string, dynamic>();
        private List<dynamic> _supplies;
        private string _staffID;
        private petsrusDataContext _lsDC;
        //private List<dynamic> _cartItems = new List<dynamic>();

        private void LoadSupplies()
        {
            petSupplyListView.ItemsSource = _supplies;
        }

        public Window5(List<dynamic> supplies, string staffID)
        {
            InitializeComponent();
            _supplies = supplies;
            _staffID = staffID; // Assign the value of staffID to _staffID
            LoadSupplies();
        }

        private void Add_to_cart_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve the selected item from the list view
            dynamic selectedSupply = ((FrameworkElement)sender).DataContext;

            // Create a unique identifier for the item
            string itemId = $"{selectedSupply.Supplies_Name}_{selectedSupply.Supply_Category}_{selectedSupply.Price}";

            // Check if the item already exists in the cart
            if (_cartItems.ContainsKey(itemId))
            {
                // If the item exists, create a new anonymous type with the updated quantity
                var existingItem = _cartItems[itemId];
                var updatedItem = new
                {
                    Supplies_Name = existingItem.Supplies_Name,
                    Supply_Category = existingItem.Supply_Category,
                    Price = existingItem.Price,
                    Quantity = existingItem.Quantity + 1,
                    Supplies_ID = selectedSupply.Supplies_ID // Include Supplies_ID
                };

                // Update the item in the cart
                _cartItems[itemId] = updatedItem;
            }
            else
            {
                // If the item doesn't exist, add it to the cart with a quantity of 1
                dynamic cartItem = new
                {
                    Supplies_Name = selectedSupply.Supplies_Name,
                    Supply_Category = selectedSupply.Supply_Category,
                    Price = selectedSupply.Price,
                    Quantity = 1,
                    Supplies_ID = selectedSupply.Supplies_ID // Include Supplies_ID
                };

                // Add the item to the cart
                _cartItems.Add(itemId, cartItem);
            }
        }



        private void ShowCart_Click(object sender, RoutedEventArgs e)
        {
            petsrusDataContext lsDC = new petsrusDataContext(); // Initialize _lsDC

            // Assuming _staffID and _supplies are already defined somewhere in your code
            Window6 window6 = new Window6(_cartItems, lsDC, _staffID, _supplies); // Pass _lsDC and _supplies to Window6
            window6.Show();
        }
    }
}
