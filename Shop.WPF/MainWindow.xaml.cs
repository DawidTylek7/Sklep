using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.EntityFrameworkCore;

namespace Shop.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ShopContext _shopContext = new ShopContext();

        private CollectionViewSource ordersViewSource;
        private CollectionViewSource customersViewSource;
        private CollectionViewSource productsViewSource;

        public MainWindow()
        {
            InitializeComponent();
            ordersViewSource = (CollectionViewSource)FindResource(nameof(ordersViewSource));
            customersViewSource = (CollectionViewSource)FindResource(nameof(customersViewSource));
            productsViewSource = (CollectionViewSource)FindResource(nameof(productsViewSource));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // this is for demo purposes only, to make it easier
            // to get up and running
            _shopContext.Database.EnsureCreated();

            // load the entities into EF Core
            _shopContext.Orders.Load();
            _shopContext.Products.Load();
            _shopContext.Customers.Load();

            // bind to the source
            ordersViewSource.Source = _shopContext.Orders.Local.ToObservableCollection();
            productsViewSource.Source = _shopContext.Products.Local.ToObservableCollection();
            customersViewSource.Source = _shopContext.Customers.Local.ToObservableCollection();


            foreach (var ctl in OrderForm.Children)
            { 
                if (ctl.GetType() == typeof(ComboBox))
                    ((ComboBox)ctl).SelectedIndex = -1;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _shopContext.Dispose();
            base.OnClosing(e);
        }

        private void OnCustomerSave(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtCustomerId.Text))
            {
                var customer = new Customer { FirstName = txtFirstName.Text, LastName = txtLastName.Text, City = txtCity.Text, ZipCode = txtZipCode.Text, Street = txtStreet.Text, Number = txtNumber.Text };
                _shopContext.Customers.Add(customer);
            }
            else
            {
                var customer = _shopContext.Customers.SingleOrDefault(x => x.CustomerId == int.Parse(txtCustomerId.Text));
                customer.FirstName = txtFirstName.Text;
                customer.LastName = txtLastName.Text;
                customer.City = txtCity.Text;
                customer.ZipCode = txtZipCode.Text;
                customer.Street = txtStreet.Text;
                customer.Number = txtNumber.Text;
            }
            
            _shopContext.SaveChanges();
            CustomerGrid.Items.Refresh();

            foreach (var ctl in CustomerForm.Children)
            {
                if (ctl.GetType() == typeof(TextBox))
                    ((TextBox)ctl).Text = string.Empty;
            }
        }

        private void OnCustomerDelete(object sender, RoutedEventArgs e)
        {
            var id = (((FrameworkElement)sender).DataContext as Customer).CustomerId;
            var customer = _shopContext.Customers.SingleOrDefault(x => x.CustomerId == id);
            _shopContext.Customers.Attach(customer);
            _shopContext.Customers.Remove(customer);
            _shopContext.SaveChanges();
            CustomerGrid.Items.Refresh();

            foreach (var ctl in CustomerForm.Children)
            {
                if (ctl.GetType() == typeof(TextBox))
                    ((TextBox)ctl).Text = string.Empty;
            }
        }

        private void OnProductSave(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtProductId.Text))
            {
                var product = new Product { ProductName = txtProductName.Text, Price = double.Parse(txtPrice.Text) };
                _shopContext.Products.Add(product);
            }
            else
            {
                var product = _shopContext.Products.SingleOrDefault(x => x.ProductId == int.Parse(txtCustomerId.Text));
                product.ProductName = txtProductName.Text;
                product.Price = double.Parse(txtPrice.Text);
            }

            _shopContext.SaveChanges();
            ProductGrid.Items.Refresh();

            foreach (var ctl in ProductForm.Children)
            {
                if (ctl.GetType() == typeof(TextBox))
                    ((TextBox)ctl).Text = string.Empty;
            }
        }

        private void OnProductDelete(object sender, RoutedEventArgs e)
        {
            var id = (((FrameworkElement)sender).DataContext as Product).ProductId;
            var customer = _shopContext.Products.SingleOrDefault(x => x.ProductId == id);
            _shopContext.Products.Attach(customer);
            _shopContext.Products.Remove(customer);
            _shopContext.SaveChanges();
            ProductGrid.Items.Refresh();

            foreach (var ctl in ProductForm.Children)
            {
                if (ctl.GetType() == typeof(TextBox))
                    ((TextBox)ctl).Text = string.Empty;
            }
        }

        private void OnOrderDelete(object sender, RoutedEventArgs e)
        {
            var id = (((FrameworkElement)sender).DataContext as Order).OrderId;
            var Order = _shopContext.Orders.SingleOrDefault(x => x.OrderId == id);
            _shopContext.Orders.Attach(Order);
            _shopContext.Orders.Remove(Order);
            _shopContext.SaveChanges();
            OrderGrid.Items.Refresh();

            foreach (var ctl in OrderForm.Children)
            {
                if (ctl.GetType() == typeof(TextBox))
                    ((TextBox)ctl).Text = string.Empty;
                if (ctl.GetType() == typeof(ComboBox))
                    ((ComboBox) ctl).SelectedIndex = -1;
            }
        }

        private void OnOrderSave(object sender, RoutedEventArgs e)
        {
            var customer = cbCustomer.SelectedValue as Customer;
            var product = cbProduct.SelectedValue as Product;

            if (string.IsNullOrEmpty(txtProductId.Text))
            {
                var order = new Order { Quantity = int.Parse(txtQuantity.Text), Customer = customer, Product = product};
                _shopContext.Orders.Add(order);
            }
            else
            {
                var order = _shopContext.Orders.SingleOrDefault(x => x.OrderId == int.Parse(txtOrderId.Text));
                order.Quantity = int.Parse(txtQuantity.Text);
                order.Customer = customer;
                order.Product = product;
            }

            _shopContext.SaveChanges();
            OrderGrid.Items.Refresh();

            foreach (var ctl in OrderForm.Children)
            {
                if (ctl.GetType() == typeof(TextBox))
                    ((TextBox)ctl).Text = string.Empty;
                if (ctl.GetType() == typeof(ComboBox))
                    ((ComboBox)ctl).SelectedIndex = -1;
            }
        }
    }
}
