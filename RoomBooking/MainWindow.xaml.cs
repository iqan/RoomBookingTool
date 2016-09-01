using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RoomBooking
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string actionUrl = @"http://localhost:20177";
        public MainWindow()
        {
            InitializeComponent();
            List<string> timeList = new List<string>();
            List<string> rooms = new List<string>();

            DateTime start = new DateTime(2016, 1, 1, 8, 0, 0);
            DateTime end = new DateTime(2016, 1, 1, 22, 0, 0);

            TimeSpan interval = TimeSpan.FromMinutes(15);

            for (DateTime current = start; current <= end; current += interval)
            {
                string stringTime = current.ToString("hh:mm tt", CultureInfo.InvariantCulture);
                timeList.Add(stringTime);
            }

            rooms.Add("2C1");
            rooms.Add("2C2");
            Room.ItemsSource = rooms;
            Room.SelectedIndex = 0;
            StartTime.ItemsSource = timeList;
            EndTime.ItemsSource = timeList;
            StartTime.SelectedIndex = 0;
            EndTime.SelectedIndex = 0;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await BookNow();
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            await GetAll();
        }
        private async Task GetAll()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(actionUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.GetAsync("api/RoomBooking");

                    if (response.IsSuccessStatusCode)
                    {
                        List<BookingModel> bookings = await response.Content.ReadAsAsync<List<BookingModel>>();

                        bookings.Sort((x, y) => x.StartDate.CompareTo(y.StartDate));
                        bookings.RemoveAll(x => x.StartDate < DateTime.Today);
                        Bookings b = new Bookings();
                        b.BookingDataGrid.ItemsSource = bookings;
                        b.Width = 800;
                        b.Height = 600;
                        b.Show();
                    }
                    else
                        MessageBox.Show(response.RequestMessage.ToString(), "Error in request");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occurred");
            }
        }

        private async Task BookNow()
        {
            try
            {
                if (EmpName.Text == "" || EmpId == null || StartDate.ToString() == "" || Subject.Text == "")
                {
                    MessageBox.Show("Fill all the Details", "Invalid Input");
                }
                else
                {
                    DateTime temptime1 = DateTime.Parse(StartTime.SelectedValue.ToString());
                    DateTime temptime2 = DateTime.Parse(EndTime.SelectedValue.ToString());
                    DateTime startDateTemp = DateTime.Parse(StartDate.Text);
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(actionUrl);
                        var content = new FormUrlEncodedContent(new[]
                        {
                            new KeyValuePair<string, string>("RoomNumber", Room.SelectedValue.ToString()),
                            new KeyValuePair<string, string>("StartDate", startDateTemp.ToString("s")),
                            new KeyValuePair<string, string>("StartTime", temptime1.TimeOfDay.ToString()),
                            new KeyValuePair<string, string>("EndTime", temptime2.TimeOfDay.ToString()),
                            new KeyValuePair<string, string>("EmpName", EmpName.Text),
                            new KeyValuePair<string, string>("EmpId", EmpId.Text),
                            new KeyValuePair<string, string>("Subject", Subject.Text)
                        });
                        var result = client.PostAsync("api/RoomBooking/PostBookingNew", content).Result;
                        string resultContent = result.Content.ReadAsStringAsync().Result;
                        MessageBox.Show(resultContent, "Error");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occurred");
            }
        }
    }
}
