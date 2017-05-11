using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace speedDials2ccs
{
    class XUser
    {
        private string firstName;
        private string lastName;
        private string number;
        private bool isProtected;

        public XUser(string firstName, string lastName, string number)
        {
            this.firstName = firstName.Replace("\\n", "|");
            this.lastName = lastName.Replace("\\n", "|");
            this.number = number;
            this.isProtected = Regex.IsMatch(number, "^1........");
        }

        public string FirstName
        {
            get { return firstName; }
        }

        public string LastName
        {
            get { return lastName; }
        }

        public string Number
        {
            get { return number; }
        }

        public bool IsProtected
        {
            get { return isProtected; }
        }
    }
}
