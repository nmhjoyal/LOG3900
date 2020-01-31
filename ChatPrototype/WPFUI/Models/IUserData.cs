namespace WPFUI.Models
{
    public interface IUserData
    {
        string currentMessage { get; set; }
        string ipAdress { get; set; }
        string userName { get; set; }
    }
}