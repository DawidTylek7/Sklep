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
        private readonly Context _context = new Context();

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
            _context.Database.EnsureCreated();

            // load the entities into EF Core
            _context.Products.Load();
            _context.Customers.Load();

            // bind to the source
            productsViewSource.Source = _context.Products.Local.ToObservableCollection();
            customersViewSource.Source = _context.Customers.Local.ToObservableCollection();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _context.SaveChanges();

            productsDataGrid.Items.Refresh();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _context.Dispose();
            base.OnClosing(e);
        }

        private void OnCustomerSave(object sender, RoutedEventArgs e)
        {
         
            if (string.IsNullOrEmpty(txtCustomerId.Text))
            {
                var customer = new Customer { FirstName = txtFirstName.Text, LastName = txtLastName.Text, City = txtCity.Text, ZipCode = txtZipCode.Text, Street = txtStreet.Text, Number = txtNumber.Text };
                _context.Customers.Add(customer);
            }
            else
            {
                var customer = _context.Customers.SingleOrDefault(x => x.CustomerId == int.Parse(txtCustomerId.Text));
                customer.FirstName = txtFirstName.Text;
                customer.LastName = txtLastName.Text;
                customer.City = txtCity.Text;
                customer.ZipCode = txtZipCode.Text;
                customer.Street = txtStreet.Text;
                customer.Number = txtNumber.Text;
            }
            
            _context.SaveChanges();
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
            var customer = _context.Customers.SingleOrDefault(x => x.CustomerId == id);
            _context.Customers.Attach(customer);
            _context.Customers.Remove(customer);
            _context.SaveChanges();
            CustomerGrid.Items.Refresh();

            foreach (var ctl in CustomerForm.Children)
            {
                if (ctl.GetType() == typeof(TextBox))
                    ((TextBox)ctl).Text = string.Empty;
            }
        }
    }
}
