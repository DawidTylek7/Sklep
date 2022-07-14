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

        private CollectionViewSource customersViewSource;
        private CollectionViewSource productsViewSource;

        public MainWindow()
        {
            InitializeComponent();
            customersViewSource = (CollectionViewSource)FindResource(nameof(customersViewSource));
            productsViewSource = (CollectionViewSource)FindResource(nameof(productsViewSource));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // this is for demo purposes only, to make it easier
            // to get up and running
            _shopContext.Database.EnsureCreated();

            // load the entities into EF Core
            _shopContext.Products.Load();
            _shopContext.Customers.Load();

            // bind to the source
            productsViewSource.Source = _shopContext.Products.Local.ToObservableCollection();
            customersViewSource.Source = _shopContext.Customers.Local.ToObservableCollection();
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
    }
}
