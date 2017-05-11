using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace speedDials2ccs
{
    class IptcParser
    {

        List<XUser> users;

        public IptcParser()
        {
            this.users = new List<XUser>();
        }

        public List<XUser> Users
        {
            get { return users; }
        }

        public void parse(string fileName)
        {
            XDocument doc = XDocument.Load(fileName);
            var phLists = from l in doc.Root.Elements("PhoneList").Elements("XmlPhoneList").Elements("User") select l;
            foreach (XElement user in phLists)
            {
                parseUser(user);
            }
        }

        private void parseUser(XElement userElement)
        {
            string[] numberElements = { 
                                          "TelephoneNumber", 
                                          "HomePhone", 
                                          "IpPhone", 
                                          "Mobile",
                                          "OtherHomePhone", 
                                          "OtherIpPhone", 
                                          "OtherMobile", 
                                          "OtherTelephone" 
                                      };

            //Create individual contacts for numbers by type
            foreach (string numberElement in numberElements)
            {
                if (userElement.Element(numberElement) == null || userElement.Element(numberElement).Value.Length <= 0)
                    continue;
                string firstName = (string)userElement.Element("FirstName") ?? String.Empty;
                string lastName = (string)userElement.Element("LastName") ?? String.Empty;
                string number = (string)userElement.Element(numberElement) ?? String.Empty;
                if (number == lastName)
                    continue;
                users.Add(
                    new XUser(firstName, lastName, number)
                );

            }
        }
    }
}
