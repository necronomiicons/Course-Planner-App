namespace Course_Planner_App;

public partial class LateReport : ContentPage
{
	public LateReport()
	{
		InitializeComponent();
		SortByPicker.SelectedIndex = 0;
        Sort();
	}

    public void OnSort(object sender, EventArgs e)
    {
        Sort();
    }

	public void Sort()
	{
        List<Assessment> late = new List<Assessment>();

        if (SortByPicker.SelectedIndex == 0)//This month
		{
			foreach(Assessment a in MainPage.database.Table<Assessment>().ToList())
			{
				if(a.complete && a.dueDate.Date < DateTime.Now && a.completionDate.Month == DateTime.Now.Month)
				{
					late.Add(a);
				}
			}
			
		}

		if(SortByPicker.SelectedIndex == 1)//Last 3 months
		{
            foreach (Assessment a in MainPage.database.Table<Assessment>().ToList())
            {
                if (a.complete && a.dueDate.Date < DateTime.Now && a.completionDate >= DateTime.Now.AddMonths(-3))
                {
                    late.Add(a);
                }
            }
        }

		if(SortByPicker.SelectedIndex == 2)//Last 6 months
		{
            foreach (Assessment a in MainPage.database.Table<Assessment>().ToList())
            {
                if (a.complete && a.dueDate.Date < DateTime.Now && a.completionDate >= DateTime.Now.AddMonths(-6))
                {
                    late.Add(a);
                }
            }
        }

		if (SortByPicker.SelectedIndex == 3)//Last 12 months
		{
            foreach (Assessment a in MainPage.database.Table<Assessment>().ToList())
            {
                if (a.complete && a.dueDate.Date < DateTime.Now && a.completionDate >= DateTime.Now.AddYears(-1))
                {
                    late.Add(a);
                }
            }
        }

		if(SortByPicker.SelectedIndex == 4)//All
		{
            foreach (Assessment a in MainPage.database.Table<Assessment>().ToList())
            {
                if (a.complete && a.dueDate < DateTime.Now)
                {
                    late.Add(a);
                }
            }
        }

        LateList.ItemsSource = late;
	}

	public async void OnBack(object sender, EventArgs e)
	{
        await Navigation.PopModalAsync();
    }
}