using Plugin.LocalNotification;

namespace Course_Planner_App;

public partial class AddEditAssessment : ContentPage
{
	Assessment assessmentToEdit;
	bool isEdit = false;
	List<string> classNames = new();

	public AddEditAssessment(bool isEdit, Assessment assessmentToEdit = null)
	{
		InitializeComponent();

        foreach (Class c in MainPage.classList) 
		{
			classNames.Add(c.className);
		}

		ClassInput.ItemsSource = classNames;

		if(isEdit)
		{
			this.isEdit = true;
			this.assessmentToEdit = assessmentToEdit;
			LoadEdit();
		}
		else
		{
			LoadNew();
		}
	}

    async void SetUpNotifs(Assessment assessment)
    {
        if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
        {
            await LocalNotificationCenter.Current.RequestNotificationPermission();
        }

		var notif = new NotificationRequest
		{
			NotificationId = 100,
			Title = "Assessment Alert",
			Description = "One of your assessments is due soon.",
			Schedule =
			{
				NotifyTime = assessment.dueDate.AddHours(-24)
            }
        };
        await LocalNotificationCenter.Current.Show(notif);
    }

    public void ShowExamTypes(object sender, EventArgs e)
	{
		if (AssessmentTypeInput.SelectedIndex == 1)
		{
			ExamTypeSection.IsVisible = true;
			ExamTypeInput.SelectedIndex = 0;
		}
		else
		{
			ExamTypeSection.IsVisible = false;
		}
	}
	public void OnSave(object sender, EventArgs e)
	{
		if(!isEdit)
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
		if(!Helper.ValidStringInput(AssessmentNameInput.Text))
		{
			await DisplayAlert("Alert", "Assessment name cannot be empty", "Ok");
			return;
		}

		//Check which date to show

		if(AssessmentTypeInput.SelectedIndex == 1)
		{
			Exam newExam = new()
			{
				assessmentName = AssessmentNameInput.Text,
				assessmentType = (AssessmentType)Enum.Parse(typeof(AssessmentType), AssessmentTypeInput.SelectedItem.ToString()),
				examType = (ExamType)Enum.Parse(typeof(ExamType), ExamTypeInput.SelectedItem.ToString()),
				classId = 0,
				enableNotifs = NotifsInput.IsToggled,
				dueDate = DueDateInput.Date,
			};

            if (newExam.enableNotifs)
            {
                SetUpNotifs(newExam);
            }

            foreach (Class c in MainPage.database.Table<Class>().ToList())
            {
                if (c.className == ClassInput.SelectedItem.ToString())
                {
                    newExam.classId = c.classId;
                    c.activeAssignments++;
                    MainPage.database.Update(c);
                    MainPage.RefreshClasses();
                    break;
                }
            }

            //Add to database
            MainPage.database.Insert(newExam);
            MainPage.assessmentList.Add(newExam);
        }
		else
		{
			Assessment newAssessment = new()
			{
				assessmentName = AssessmentNameInput.Text,
				assessmentType = (AssessmentType)Enum.Parse(typeof(AssessmentType), AssessmentTypeInput.SelectedItem.ToString()),
				classId = 0,
				enableNotifs = NotifsInput.IsToggled,
			    dueDate = DueDateInput.Date,			
			};

			if(newAssessment.enableNotifs)
			{
				SetUpNotifs(newAssessment);
			}

            foreach (Class c in MainPage.database.Table<Class>().ToList())
            {
                if (c.className == ClassInput.SelectedItem.ToString())
                {
                    newAssessment.classId = c.classId;
                    c.activeAssignments++;
                    MainPage.database.Update(c);
                    MainPage.RefreshClasses();
                    break;
                }
            }
            //Add to database
            MainPage.database.Insert(newAssessment);
            MainPage.assessmentList.Add(newAssessment);
        }

        await Navigation.PopModalAsync();
    }

	async void SaveEdit()
	{
        if (!Helper.ValidStringInput(AssessmentNameInput.Text))
        {
            await DisplayAlert("Alert", "Assessment name cannot be empty", "Ok");
            return;
        }
        //Remove active assignment from old class
        foreach (Class c in MainPage.database.Table<Class>().ToList())
        {
            if (c.classId == assessmentToEdit.classId)
            {
                c.activeAssignments--;
                MainPage.database.Update(c);
                break;
            }
        }

        assessmentToEdit.assessmentName = AssessmentNameInput.Text;
		assessmentToEdit.assessmentType = (AssessmentType)Enum.Parse(typeof(AssessmentType), AssessmentTypeInput.SelectedItem.ToString());         
		assessmentToEdit.enableNotifs = NotifsInput.IsToggled;
		assessmentToEdit.dueDate = DueDateInput.Date;

		if(assessmentToEdit.enableNotifs)
		{
			SetUpNotifs(assessmentToEdit);
		}

		//Add active assignment to new class
        foreach (Class c in MainPage.database.Table<Class>().ToList())
        {
            if (c.className == ClassInput.SelectedItem.ToString())
            {
                assessmentToEdit.classId = c.classId;
                c.activeAssignments++;
                MainPage.database.Update(c);
                break;
            }
        }

        //Add to database
        MainPage.database.Update(assessmentToEdit);
		MainPage.RefreshAssessments();
		MainPage.RefreshClasses();
        await Navigation.PopModalAsync();
    }

	public async void OnCancel(object send, EventArgs e)
	{
        await Navigation.PopModalAsync();
    }

	public void LoadNew()
	{
		AssessmentName.Text = "New Assessment";
		AssessmentTypeInput.SelectedIndex = 0;
		ClassInput.SelectedIndex = 0;		
	}

	public void LoadEdit()
	{
		AssessmentName.Text = "Edit Assessment";
		AssessmentNameInput.Text = assessmentToEdit.assessmentName;
		NotifsInput.IsToggled = assessmentToEdit.enableNotifs;
		int typeIndex = 0;

		foreach(string type in AssessmentTypeInput.Items)
		{
			if(type == assessmentToEdit.assessmentType.ToString())
			{
				break;
			}
			typeIndex++;
		}
		AssessmentTypeInput.SelectedIndex = typeIndex;
		ClassInput.SelectedItem = Helper.GetClassNameFromId(assessmentToEdit.classId);

		DueDateInput.Date = assessmentToEdit.dueDate;

	}
}