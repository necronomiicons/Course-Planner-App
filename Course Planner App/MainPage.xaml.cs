using SQLite;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Course_Planner_App
{
    public partial class MainPage : ContentPage
    {
        public static SQLiteConnection database;
        public static ObservableCollection<Class> classList = new ObservableCollection<Class>();
        public static ObservableCollection<Assessment> assessmentList = new ObservableCollection<Assessment>();

        public MainPage()
        {
            InitializeComponent();
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CoursePlannerDatabase.db3");
            database = new SQLiteConnection(dbPath);

            database.DropTable<Class>();
            database.DropTable<Assessment>();
            database.DropTable<Exam>();
            database.CreateTables<Class, Assessment, Exam>();

            foreach (Class c in database.Table<Class>().ToList())
            {
                classList.Add(c);
            }

            foreach(Assessment a in database.Table<Assessment>().ToList())
            {
                assessmentList.Add(a);
            }

            ClassesList.ItemsSource = classList;
            AssessmentsList.ItemsSource = assessmentList;
            SortByPicker.SelectedIndex = 0;
        }

        public static void RefreshClasses()
        {
            classList.Clear();
            foreach (Class c in database.Table<Class>().ToList())
            {
                classList.Add(c);
            }
        }

        public static void RefreshAssessments()
        {
            assessmentList.Clear();
            foreach(Assessment a in database.Table<Assessment>().ToList())
            {
                assessmentList.Add(a);
            }
            foreach(Exam e in database.Table<Exam>().ToList())
            {
                assessmentList.Add(e);
            }
        }

        //Class Buttons
        public async void OnAddClass(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new AddEditClass(false));
        }
        public async void OnEditClass(object sender, EventArgs e)
        {
            if(ClassesList.SelectedItem != null)
            {
                Class classToEdit = (Class)ClassesList.SelectedItem;
                await Navigation.PushModalAsync(new AddEditClass(true, classToEdit));
            }
            else
            {
                await DisplayAlert("Alert", "No item selected to edit", "Ok");
            }
        }
        public async void OnDeleteClass(object sender, EventArgs e)
        {
            if (ClassesList.SelectedItem != null)
            {
                Class selectedClass = (Class)ClassesList.SelectedItem;
                //Only delete class if it has no assignments
                if (selectedClass.activeAssignments == 0)
                {
                    database.Delete((Class)ClassesList.SelectedItem);
                    classList.Remove((Class)ClassesList.SelectedItem);
                }
                else
                {
                    await DisplayAlert("Alert", "Cannot delete classes with associated assignments.", "Ok");
                }
            }
            else
            {
                await DisplayAlert("Alert", "No item selected to delete", "Ok");
            }
            ClassesList.SelectedItem = null;
        }

        //Assessment Buttons
        public async void OnAddAssessment(object sender, EventArgs e)
        {
            if(classList.Count == 0)
            {
                await DisplayAlert("Alert", "Cannot create an assignment when class list is empty", "Ok");
            }
            else
            {
                await Navigation.PushModalAsync(new AddEditAssessment(false));
            }
        }
        public async void OnEditAssessment(object sender, EventArgs e)
        {
            if(AssessmentsList.SelectedItem != null)
            {
                Assessment assessmentToEdit = (Assessment)AssessmentsList.SelectedItem;
                await Navigation.PushModalAsync(new AddEditAssessment(true, assessmentToEdit));
            }
            else
            {
                await DisplayAlert("Alert", "No item selected to edit", "Ok");
            }
        }
        public async void OnDeleteAssessment(object sender, EventArgs e)
        {
            if (AssessmentsList.SelectedItem != null)
            {
                Assessment selectedAssessment = (Assessment)AssessmentsList.SelectedItem;              
                database.Delete(selectedAssessment);
                assessmentList.Remove(selectedAssessment);

                foreach(Class c in classList)
                {
                    if(c.className == selectedAssessment.GetClassName)
                    {
                        c.activeAssignments--;
                        database.Update(c);
                    }
                }
                RefreshClasses();
            }
            else
            {
                await DisplayAlert("Alert", "No item selected to delete", "Ok");
            }
            AssessmentsList.SelectedItem = null;
        }
        public async void OnCompleteAssessment(object sender, EventArgs e)
        {
            if (AssessmentsList.SelectedItem != null)
            {
                Assessment selectedAssessment = (Assessment)AssessmentsList.SelectedItem;
                if(!selectedAssessment.complete)
                {
                    selectedAssessment.Complete();
                    database.Update(selectedAssessment);
                    RefreshAssessments();
                }
                else
                {
                    await DisplayAlert("Alert", "Selected item has already been completed", "Ok");
                }
            }
            else
            {
                await DisplayAlert("Alert", "No item selected to complete", "Ok");
            }
            AssessmentsList.SelectedItem = null;
        }

        //Sort Mode Picker
        public void OnChangeMode(object sender, EventArgs e)
        {

        }

        //Search Button
        public void OnSearch(object sender, EventArgs e)
        {
            string query = SearchText.Text;

            if(query == null)
            {
                return;
            }

            List<Assessment> searchResults = new List<Assessment>();

            foreach(Assessment a in assessmentList)
            {
                if(a.assessmentName.Contains(query))
                {
                    searchResults.Add(a);
                }
            }

            AssessmentsList.ItemsSource = searchResults;
        }

        public void OnSearchTextCleared(object sender, TextChangedEventArgs e)
        {
            if(SearchText.Text == "")
            {
                AssessmentsList.ItemsSource = assessmentList;
            }
        }

        //Report Button
        public async void OnLateReport(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new LateReport());
        }

        
        public void OnSort(object sender, EventArgs e)
        {
            if(SortByPicker.SelectedIndex == 0)//All
            {
                AssessmentsList.ItemsSource = assessmentList;
            }

            if(SortByPicker.SelectedIndex == 1)//Today
            {
                List<Assessment> today = new();
                foreach(Assessment a in assessmentList)
                {
                    if(a.dueDate.Date == DateTime.Now.Date)
                    {
                        today.Add(a);
                    }
                }
                AssessmentsList.ItemsSource = today;
            }

            if (SortByPicker.SelectedIndex == 2)//Week
            {
                List<Assessment> week = new();
                foreach (Assessment a in assessmentList)
                {
                    if (a.dueDate.Date <= DateTime.Now.Date.AddDays(7))
                    {
                        week.Add(a);
                    }
                }
                AssessmentsList.ItemsSource = week;
            }

            if (SortByPicker.SelectedIndex == 3)//Month
            {
                List<Assessment> month = new();
                foreach (Assessment a in assessmentList)
                {
                    if (a.dueDate.Date.Month == DateTime.Now.Date.Month)
                    {
                        month.Add(a);
                    }
                }
                AssessmentsList.ItemsSource = month;
            }
        }
    }

}
