namespace Course_Planner_App;

public partial class AddEditClass : ContentPage
{
	string safeName;
	bool edit = false;
    Class classToEdit;
	public AddEditClass(bool isEdit, Class classToEdit = null)
	{
		InitializeComponent();

		if(isEdit)
		{
			edit = true;
            safeName = classToEdit.className;
            this.classToEdit = classToEdit;
			LoadEdit();
		}
		else
		{
			LoadNew();
		}
	}


	public void OnSave(object sender, EventArgs e)
	{		
		if(!edit)
		{
			SaveNew();
		}
		else
		{
			SaveEdit();
		}
    }

	async void SaveNew()
	{
        //Exception handling
        if (!Helper.ValidStringInput(ClassNameInput.Text))
        {
            await DisplayAlert("Alert", "Class name cannot be empty", "Ok");
            return;
        }

        if (!Helper.ClassIsUnique(ClassNameInput.Text))
        {
            await DisplayAlert("Alert", "Class name already exists", "Ok");
            return;
        }

        //Create class
        Class newClass = new Class()
        {
            className = ClassNameInput.Text
        };

        //Add to database
        MainPage.database.Insert(newClass);
        MainPage.classList.Add(newClass);
        await Navigation.PopModalAsync();
    }

    async void SaveEdit()
    {
        //Exception handling
        if (!Helper.ValidStringInput(ClassNameInput.Text))
        {
            await DisplayAlert("Alert", "Class name cannot be empty", "Ok");
            return;
        }

        if (!Helper.ClassIsUnique(ClassNameInput.Text) && ClassNameInput.Text != safeName)
        {
            await DisplayAlert("Alert", "Class name already exists", "Ok");
            return;
        }

        //Update class
        classToEdit.className = ClassNameInput.Text;

        //Update database
        MainPage.database.Update(classToEdit);

        //Update listview
        MainPage.classList.Clear();
        foreach (Class c in MainPage.database.Table<Class>().ToList())
        {
            MainPage.classList.Add(c);
        }
        MainPage.RefreshAssessments();

        await Navigation.PopModalAsync();
    }

    public async void OnCancel(object sender, EventArgs e)
	{
        await Navigation.PopModalAsync();
    }

	void LoadEdit()
	{
		ClassName.Text = "Edit Class";
		ClassNameInput.Text = classToEdit.className;
	}

	void LoadNew()
	{
		ClassName.Text = "New Class";
	}
}