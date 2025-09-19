
namespace Grocery.Core.Models
{
    public partial class Client : Model
    {
        private string _emailAddress;
        private string _password;
        
        public string EmailAddress
        {
            get { return _emailAddress; }
            set { _emailAddress = value; }
        }

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
        public Client(int id, string name, string emailAddress, string password) : base(id, name)
        {
            _emailAddress=emailAddress;
            _password=password;
        }
    }
}
