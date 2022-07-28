namespace FBMS3.Web.ViewModels;

public class AboutViewModel
{
    public string Title { get; set; }

    public string Message { get; set; }

    public DateTime Formed { get; set ;} = DateTime.Now;

    public string FormedString => Formed.ToShortDateString();

    public int Days => DateTime.Now.Subtract(Formed).Days;
}