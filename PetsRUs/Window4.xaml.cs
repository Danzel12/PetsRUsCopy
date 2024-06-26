﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;


namespace PetsRUs
{
    public partial class Window4 : Window
    {
        private petsrusDataContext _lsDC;
        private string _orderID;
        private string _customerID;

        public Window4(string orderID, string customerID)
        {
            InitializeComponent();
            _orderID = orderID;
            _customerID = customerID; // Assign customerID to _customerID
            _lsDC = new petsrusDataContext(Properties.Settings.Default.petsrusConnectionString);

            // Generate Payment_ID
            string paymentID = GeneratePaymentID();
            txtPaymentID.Text = paymentID;

            // Set Total Amount
            txtTotalAmount.Text = "5000"; // Fixed price for adopting a pet
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void btnPay_Click(object sender, RoutedEventArgs e)
        {
            string paymentMethod = cmbPaymentMethod.SelectedItem?.ToString();
            decimal totalAmount;
            decimal paymentAmount;

            if (decimal.TryParse(txtTotalAmount.Text, out totalAmount) &&
                decimal.TryParse(txtPaymentAmount.Text, out paymentAmount))
            {
                decimal remainingBalance = paymentAmount - totalAmount;

                // Check if the payment amount is sufficient
                if (remainingBalance >= 0)
                {
                    // Insert into Payment Table
                    Payment newPayment = new Payment
                    {
                        Payment_ID = txtPaymentID.Text,
                        Customer_ID = _customerID, // Use _customerID instead of _orderID
                        Total_Amount = totalAmount,
                        Payment_Amount = paymentAmount,
                        Payment_Change = remainingBalance,
                        Payment_Date = DateTime.Now,
                        Payment_Method = paymentMethod
                    };

                    _lsDC.Payments.InsertOnSubmit(newPayment);
                    _lsDC.SubmitChanges();

                    MessageBox.Show("Payment Successful!");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Payment amount exceeds the total amount!");
                }
            }
            else
            {
                MessageBox.Show("Please enter valid payment amount.");
            }
        }

        private string GeneratePaymentID()
        {
            // Generate a unique Payment_ID
            int count = _lsDC.Payments.Count() + 1;
            return "PID" + count.ToString("D3"); // Format count to have leading zeros if necessary
        }

        private void cmbPaymentMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbPaymentMethod.SelectedItem?.ToString() == "Cash")
            {
                // Show Payment Amount input for Cash payment
                txtPaymentAmount.Visibility = Visibility.Visible;
                lblPaymentAmount.Visibility = Visibility.Visible;
            }
            else
            {
                // Hide Payment Amount input for Card payment
                txtPaymentAmount.Visibility = Visibility.Collapsed;
                lblPaymentAmount.Visibility = Visibility.Collapsed;
            }
        }
    }
}