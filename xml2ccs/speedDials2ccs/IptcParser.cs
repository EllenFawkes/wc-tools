using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml;

namespace speedDials2ccs
{
    class TInfo 
    {
        public string IptcButton;
        public string Tab;
        public string Subtab;
        public string PosX;
        public string PosY;
    }

    class IptcParser
    {

        const string IPTC_NAME_REGEX = "^(.+)HotlineButton_Level_(\\d+)_(\\d+)_(\\d+)$";

        private List<Types.Tab> tabs;

        public IptcParser()
        {
            tabs = new List<Types.Tab>();
        }

        public XDocument parse(string speedDialsFile, string phoneListFile)
        {
            XDocument speedDials = XDocument.Load(speedDialsFile);
            XDocument phoneList = XDocument.Load(phoneListFile);
            //XDocument texts = XDocument.Load("textResources.xml");

            var root =
                from s in speedDials.Descendants("HotLineButtons").Descendants("HotLineButton")
                join p in phoneList.Descendants("PhoneList").Descendants("XmlPhoneList").Descendants("User")
                on (string)s.Element("PhoneListUserName")
                equals (string)p.Element("Name")
                select new
                {
                    Name = (string)s.Element("IptcButtonName"),
                    ListName = (string)s.Element("PhoneListName"),
                    ContactName = (string)p.Element("LastName"),
                    Info = extractPropertiesFromName((string)s.Element("IptcButtonName")),
                    Numbers = new {
                        TelephoneNumber = (string)p.Element("TelephoneNumber"),
                        HomePhone = (string)p.Element("HomePhone"),
                        IpPhone = (string)p.Element("IpPhone"),
                        Mobile = (string)p.Element("Mobile"),
                        OtherHomePhone = (string)p.Element("OtherHomePhone"),
                        OtherIpPhone = (string)p.Element("OtherIpPhone"),
                        OtherMobile = (string)p.Element("OtherMobile"),
                        OtherTelephone = (string)p.Element("OtherTelephone")
                    }
                };

            XDocument k = new XDocument(new XElement("tabs"));
            foreach (var r in root)
            {
                if (r.Info != null)
                {
                    //Node Tab (AUT, MB...)
                    XElement tab = enterNode(k.Root, r.Info.Tab.ToLower()); // tabs>AUT
                    XElement subtabs = enterNode(tab, "subtabs"); // tabs>AUT>subtabs
                    XElement subtab = enterNode(subtabs, r.Info.Tab.ToLower() + int.Parse(r.Info.Subtab).ToString("00")); // tabs>AUT>subtabs>AUT1
                    XElement buttons = enterNode(subtab, "buttons"); // tabs>AUT>subtabs>AUT1>buttons
                    XElement button = enterNode(buttons, r.Name); // tabs>AUT>subtabs>AUT1>buttons>BUTTON1

                    subtab.SetElementValue("title", r.Info.Tab + " " + r.Info.Subtab); //Set title for subtab

                    if (r.Numbers.TelephoneNumber == r.ContactName)
                    {
                        button.SetElementValue("style", "BUTTON_CALL_DUMMY");
                    }
                    else
                    {
                        button.SetElementValue("number", r.Numbers.TelephoneNumber);
                        button.SetElementValue("style", "BUTTON_CALL_DEFAULT");
                    }
                    button.SetElementValue("border_color", "91 91 91");
                    button.SetElementValue("enabled", "true");
                    button.SetElementValue("tag", "callButton");
                    button.SetElementValue("span_h", 1);
                    button.SetElementValue("span_w", 1);
                    button.SetElementValue("pos_x", int.Parse(r.Info.PosX) - 1);
                    button.SetElementValue("pos_y", int.Parse(r.Info.PosY) - 1);
                }

            }
            return k;
        }

        private XElement enterNode(XElement parent, string nodeName)
        {
            XElement node = parent.Element(nodeName);
            if (node == null)
            {
                node = new XElement(nodeName);
                parent.Add(node);
            }
            return node;
        }

        private TInfo extractPropertiesFromName(string iptcName)
        {
            if (!Regex.IsMatch(iptcName, IPTC_NAME_REGEX))
                return null;
            Match match = Regex.Match(iptcName, IPTC_NAME_REGEX);
            return new TInfo
            {
                Tab = match.Groups[1].Value,
                Subtab = match.Groups[2].Value,
                PosX = match.Groups[3].Value,
                PosY = match.Groups[4].Value
            };
        }
    }
}
