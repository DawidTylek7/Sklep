using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
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
        private CollectionViewSource shippingsViewSource;

        public MainWindow()
        {
            InitializeComponent();
            ordersViewSource = (CollectionViewSource)FindResource(nameof(ordersViewSource));
            customersViewSource = (CollectionViewSource)FindResource(nameof(customersViewSource));
            productsViewSource = (CollectionViewSource)FindResource(nameof(productsViewSource));
            shippingsViewSource = (CollectionViewSource)FindResource(nameof(shippingsViewSource));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _shopContext.Database.EnsureCreated();

            _shopContext.Orders.Load();
            _shopContext.Products.Load();
            _shopContext.Customers.Load();
            _shopContext.Shippings.Load();

            ordersViewSource.Source = _shopContext.Orders.Local.ToObservableCollection();
            productsViewSource.Source = _shopContext.Products.Local.ToObservableCollection();
            customersViewSource.Source = _shopContext.Customers.Local.ToObservableCollection();
            shippingsViewSource.Source = _shopContext.Shippings.Local.ToObservableCollection();

            ClearGridFields(OrderForm);
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

            ClearGridFields(CustomerForm);
        }

        private void OnCustomerDelete(object sender, RoutedEventArgs e)
        {
            var id = (((FrameworkElement)sender).DataContext as Customer).CustomerId;
            var customer = _shopContext.Customers.SingleOrDefault(x => x.CustomerId == id);
            _shopContext.Customers.Attach(customer);
            _shopContext.Customers.Remove(customer);
            _shopContext.SaveChanges();
            CustomerGrid.Items.Refresh();

            ClearGridFields(CustomerForm);
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
                var product = _shopContext.Products.SingleOrDefault(x => x.ProductId == int.Parse(txtProductId.Text));
                product.ProductName = txtProductName.Text;
                product.Price = double.Parse(txtPrice.Text);
            }

            _shopContext.SaveChanges();
            ProductGrid.Items.Refresh();

            ClearGridFields(ProductForm);
        }

        private void OnProductDelete(object sender, RoutedEventArgs e)
        {
            var id = (((FrameworkElement)sender).DataContext as Product).ProductId;
            var product = _shopContext.Products.SingleOrDefault(x => x.ProductId == id);
            _shopContext.Products.Attach(product);
            _shopContext.Products.Remove(product);
            _shopContext.SaveChanges();
            ProductGrid.Items.Refresh();

            ClearGridFields(ProductForm);
        }

        private void OnOrderDelete(object sender, RoutedEventArgs e)
        {
            var id = (((FrameworkElement)sender).DataContext as Order).OrderId;
            var Order = _shopContext.Orders.SingleOrDefault(x => x.OrderId == id);
            _shopContext.Orders.Attach(Order);
            _shopContext.Orders.Remove(Order);
            _shopContext.SaveChanges();
            OrderGrid.Items.Refresh();

            ClearGridFields(OrderForm);
        }

        private void OnOrderSave(object sender, RoutedEventArgs e)
        {
            var customer = cbCustomer.SelectedValue as Customer;
            var product = cbProduct.SelectedValue as Product;
            var shipping = cbShipping.SelectedValue as Shipping;

            if (string.IsNullOrEmpty(txtOrderId.Text))
            {
                var order = new Order { Quantity = int.Parse(txtQuantity.Text), Customer = customer, Product = product, Shipping = shipping};
                _shopContext.Orders.Add(order);
            }
            else
            {
                var order = _shopContext.Orders.SingleOrDefault(x => x.OrderId == int.Parse(txtOrderId.Text));
                order.Quantity = int.Parse(txtQuantity.Text);
                order.Customer = customer;
                order.Product = product;
                order.Shipping = shipping;
            }

            _shopContext.SaveChanges();
            OrderGrid.Items.Refresh();

            ClearGridFields(OrderForm);
        }
        
        private void OnShippingSave(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtShippingId.Text))
            {
                var shipping = new Shipping { ShippingName = txtShippingName.Text, ShippingCost = double.Parse(txtShippingCost.Text) };
                _shopContext.Shippings.Add(shipping);
            }
            else
            {
                var shipping = _shopContext.Shippings.SingleOrDefault(x => x.ShippingId == int.Parse(txtShippingId.Text));
                shipping.ShippingName = txtShippingName.Text;
                shipping.ShippingCost = double.Parse(txtShippingCost.Text);
            }

            _shopContext.SaveChanges();
            ShippingGrid.Items.Refresh();

            ClearGridFields(ShippingForm);
        }

        private void OnShippingDelete(object sender, RoutedEventArgs e)
        {
            var id = (((FrameworkElement)sender).DataContext as Shipping).ShippingId;
            var shipping = _shopContext.Shippings.SingleOrDefault(x => x.ShippingId == id);
            _shopContext.Shippings.Attach(shipping);
            _shopContext.Shippings.Remove(shipping);
            _shopContext.SaveChanges();
            ShippingGrid.Items.Refresh();

            ClearGridFields(ShippingForm);
        }

        private void PriceComponentsOnTextChanged(object sender, TextChangedEventArgs e)
        {
            Calculate();
        }

        private void PriceComponentsOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Calculate();
        }

        private void Calculate()
        {
            if (string.IsNullOrEmpty(txtQuantity.Text))
            {
                return;
            }

            if (cbProduct.SelectedIndex == -1)
            {
                return;
            }

            if (cbShipping.SelectedIndex == -1)
            {
                return;
            }

            var product = cbProduct.SelectedValue as Product;
            var shipping = cbShipping.SelectedValue as Shipping;

            txtTotal.Text = ((int.Parse(txtQuantity.Text) * product.Price) + shipping.ShippingCost).ToString();
        }

        private static void ClearGridFields(Grid grid)
        {
            foreach (var ctl in grid.Children)
            {
                if (ctl.GetType() == typeof(TextBox))
                    ((TextBox)ctl).Text = string.Empty;
                if (ctl.GetType() == typeof(ComboBox))
                    ((ComboBox)ctl).SelectedIndex = -1;
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9,]+");
            e.Handled = regex.IsMatch(e.Text);
        }

    }
}
