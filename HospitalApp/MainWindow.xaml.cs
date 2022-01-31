using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HospitalApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ComboSpecialization.ItemsSource = App.DataBase.Specializations.ToList();
            ComboSpecialization.SelectedIndex = 0;

        }

        private void ComboSpecialization_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedSpecialization = ComboSpecialization.SelectedItem as Entities.Specialization;
                if(selectedSpecialization !=null)
            {
                var doctors = App.DataBase.Doctors.ToList().Where(p => p.Specialization == selectedSpecialization);
                ComboDoctor.ItemsSource = doctors;
                ComboDoctor.SelectedItem = 0;
                
            }
        }

        private void ComboDoctor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedDoctor = ComboDoctor.SelectedItem as Entities.Doctor;
            if (selectedDoctor !=null)
            {
                GenerateSchedule(selectedDoctor);
            }
        }

        private void GenerateSchedule(Entities.Doctor selectedDoctor)
        {
            var startDate = DateTime.Parse("11-12-2021");
            var endDate = startDate.AddDays(5);

            var scheduleGenerator = new Utils.ScheduleGenerator(startDate, endDate, selectedDoctor.DoctorSchedules.ToList());

            var headers = scheduleGenerator.GenerateHeader();
            var appointments = scheduleGenerator.GenerateAppoinments(headers);
            LoadSchedule(headers, appointments);
        }

        private void LoadSchedule(List<Entities.ScheduleHeader> headers, List<List<Entities.ScheduleAppoinment>> appoinments)
        {
            DGridSchedule.Columns.Clear();
            for (int i = 0; i < headers.Count(); i++)
            {
                FrameworkElementFactory headerFactory = new FrameworkElementFactory(typeof(UserControls.ScheduleHeaderControl));
                headerFactory.SetValue(DataContextProperty, headers[i]);
                var headerTemplate = new DataTemplate
                {
                    VisualTree = headerFactory
                };

                FrameworkElementFactory cellFactory = new FrameworkElementFactory(typeof(UserControls.ScheduleAppointmentControl));
                cellFactory.SetBinding(DataContextProperty, new Binding($"[{i}]"));

                cellFactory.AddHandler(MouseDownEvent, new MouseButtonEventHandler(BtnAppointment_MouseDown), true);
                var cellTemplate = new DataTemplate
                {
                    VisualTree = cellFactory
                };

                var columnTemplate = new DataGridTemplateColumn
                {
                    HeaderTemplate = headerTemplate,
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                    CellTemplate = cellTemplate
                };

                DGridSchedule.Columns.Add(columnTemplate);
            }
            DGridSchedule.ItemsSource = appoinments;
        }
        private void BtnAppointment_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var currentControl = sender as UserControls.ScheduleAppointmentControl;
            var currentAppointment = currentControl.DataContext as Entities.ScheduleAppoinment;

            if(currentAppointment != null && currentAppointment.ScheduleId > 0 && currentAppointment.AppoinmentType == Entities.AppoinmentType.Free)
            {
                App.DataBase.Appointments.Add(new Entities.Appointment
                {
                    DoctorScheduleId = currentAppointment.ScheduleId,
                    StartTime = currentAppointment.StartTime,
                    EndTime = currentAppointment.EndTime,
                    ClientId = 1
                });

                App.DataBase.SaveChanges();
                MessageBox.Show("Вы записаны на приём!");
                ComboDoctor_SelectionChanged(ComboDoctor, null);
            }
        }
    }
}
