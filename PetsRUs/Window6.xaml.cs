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
    public partial class Window6 : Window
    {
        private Dictionary<string, dynamic> _cartItems = new Dictionary<string, dynamic>();
        private petsrusDataContext _lsDC;
        private string _staffID;

        public Window6(Dictionary<string, dynamic> cartItems, petsrusDataContext lsDC, string staffID)
        {
            InitializeComponent();
            _cartItems = cartItems;
            _lsDC = lsDC; // Initialize _lsDC
            _staffID = staffID;
            LoadCartItems();
            CalculateTotalAmount();
        }
        private void LoadCartItems()
        {
            cartListView.ItemsSource = _cartItems.Values.ToList();
        }

        private void CalculateTotalAmount()
        {
            decimal totalAmount = 0;
            foreach (var item in _cartItems.Values)
            {
                totalAmount += item.Price * item.Quantity;
            }
            txtTotalAmount.Text = totalAmount.ToString();
        }

        private void CheckoutButton_Click(object sender, RoutedEventArgs e)
        {
            string customerName = txtCustomerName.Text;
            string paymentMethod = cmbPaymentMethod.SelectedItem?.ToString();
            decimal totalAmount;
            decimal paymentAmount;

            if (decimal.TryParse(txtTotalAmount.Text, out totalAmount) &&
                decimal.TryParse(txtPaymentAmount.Text, out paymentAmount)) // Parse payment amount
            {
                // Validate input
                if (string.IsNullOrEmpty(customerName) || string.IsNullOrEmpty(paymentMethod))
                {
                    MessageBox.Show("Please fill all fields correctly.");
                    return;
                }

                // Calculate remaining balance
                decimal remainingBalance = paymentAmount - totalAmount;

                // Create customer and order records
                string orderID = GenerateOrderID();
                string customerID = GenerateCustomerID();

                if (!string.IsNullOrEmpty(orderID) && !string.IsNullOrEmpty(customerID))
                {
                    try
                    {
                        // Insert into Customers table
                        Customer newCustomer = new Customer
                        {
                            Customer_ID = customerID,
                            Customer_Name = customerName,
                            // Add other customer details as needed
                        };
                        _lsDC.Customers.InsertOnSubmit(newCustomer);

                        // Insert orders for each item in the cart
                        foreach (var item in _cartItems.Values)
                        {
                            Order newOrder = new Order
                            {
                                Order_ID = orderID,
                                Customer_ID = customerID,
                                Staff_ID = _staffID,
                                Order_Desc = "Pet Supply", // Provide a description for the order
                                Order_Date = DateTime.Now,
                                Order_Status = "Complete",
                                Supplies_ID = item.Supplies_ID // Assign Supplies_ID
                            };
                            _lsDC.Orders.InsertOnSubmit(newOrder);
                        }

                        // Insert payment record
                        Payment newPayment = new Payment
                        {
                            Payment_ID = GeneratePaymentID(),
                            Order_ID = orderID,
                            Total_Amount = totalAmount,
                            Payment_Amount = paymentAmount, // Assign payment amount
                            Payment_Change = remainingBalance, // Assign remaining balance as change
                            Payment_Method = paymentMethod,
                            Payment_Date = DateTime.Now, // Set the payment date to the current date and time
                                                         // Add other payment details like Payment_Amount, Payment_Amount, etc.
                        };
                        _lsDC.Payments.InsertOnSubmit(newPayment);

                        // Submit changes to the database
                        _lsDC.SubmitChanges();

                        MessageBox.Show("Checkout successful!");
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Failed to generate Customer_ID or Order_ID.");
                }
            }
            else
            {
                MessageBox.Show("Please enter valid total amount and payment amount.");
            }
        }

        private string GenerateCustomerID()
        {
            int count = _lsDC.Customers.Count() + 1;
            return "CS" + count.ToString("D3"); // Format count to have leading zeros if necessary
        }

        private string GenerateOrderID()
        {
            int count = _lsDC.Orders.Count() + 1;
            return "OID" + count.ToString("D3"); // Format count to have leading zeros if necessary
        }

        private string GeneratePaymentID()
        {
            int count = _lsDC.Payments.Count() + 1;
            return "PID" + count.ToString("D3"); // Format count to have leading zeros if necessary
        }
    }
}